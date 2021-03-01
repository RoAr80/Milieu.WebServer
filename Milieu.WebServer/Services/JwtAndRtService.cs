using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Milieu.Models.Account.Models;
using Milieu.Models.Requests;
using Milieu.Models.Responses;
using Milieu.WebServer.Data.Repos;
using Milieu.WebServer.Helpers;
using Milieu.WebServer.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Milieu.WebServer.Services
{
    public class JwtAndRtService : IJwtAndRtService
    {
        private IMilieuRepo _milieuRepo;
        private readonly AppSettings _appSettings;

        public JwtAndRtService(
            IMilieuRepo milieuRepo,
            IOptions<AppSettings> appSettings)
        {
            _milieuRepo = milieuRepo;
            _appSettings = appSettings.Value;
        }

        public AuthenticateApiResponse GetJwtAndRt(AuthenticateApiRequest model, string ipAddress)
        {
            var user = _milieuRepo.GetUser(model.Email);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwt(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
            _milieuRepo.UpdateOrAddRefreshToken(user, refreshToken);

            return new AuthenticateApiResponse(user, jwtToken, refreshToken.Value);
        }

        public AuthenticateApiResponse GetJwtAndRtViaRt(string refreshToken, string ipAddress)
        {
            var user = _milieuRepo.GetUserByToken(refreshToken);

            // return null if no user found with token or refreshtoken expired
            if (user == null || user.RefreshTokens[0].IsExpired) return null;            
           
            RefreshToken newRefreshToken = generateRefreshToken(ipAddress);

            // replace old refresh token with a new one and save
            _milieuRepo.UpdateOrAddRefreshToken(user, newRefreshToken);

            // generate new jwt
            var jwt = generateJwt(user);

            return new AuthenticateApiResponse(user, jwt, newRefreshToken.Value);
        }

        #region Helper Methods


        private string generateJwt(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Value = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        } 
        #endregion

    }
}

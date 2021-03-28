using Milieu.Models.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Milieu.Models.Responses
{
    public class AuthenticateApiResponse
    {
        
        public DefaultApiResponse ApiResponseDefault { get; set; }
        public string Email { get; set; }
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticateApiResponse()
        {

        }
        public AuthenticateApiResponse(User user, string jwt, string refreshToken)
        {
            ApiResponseDefault = new DefaultApiResponse();
            Email = user.Email;
            Jwt = jwt;
            RefreshToken = refreshToken;
        }
    }
}

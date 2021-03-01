using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Milieu.Models.Account.Models;
using Milieu.Models.Account.Requests;
using Milieu.Models.Requests;
using Milieu.Models.Responses;
using Milieu.Models.Routes;
using Milieu.WebServer.HelperMethods;
using Milieu.WebServer.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Milieu.WebServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.AccountController)]
    [Authorize(AuthenticationSchemes = "Identity.Application, Bearer")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private IConfiguration _configuration;
        private IJwtAndRtService _jwtAndRtService;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            IJwtAndRtService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;
            _jwtAndRtService = userService;
        }
        
        [HttpPost(ApiRoutes.Register)]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterApiRequest registerApiRequest)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = registerApiRequest.Email,
                    UserName = registerApiRequest.Email,
                };

                var result = await _userManager.CreateAsync(user, registerApiRequest.Password);
                if (result.Succeeded)
                {                    
                    AuthenticateApiRequest authenticateRequest = new AuthenticateApiRequest(registerApiRequest.Email);                    
                    AuthenticateApiResponse authenticateResponse = _jwtAndRtService.GetJwtAndRt(authenticateRequest, ipAddress());                    
                    await _signInManager.SignInAsync(user, false);

                    return Ok(authenticateResponse);

                }
                else
                {
                    return BadRequest(
                        new AuthenticateApiResponse 
                        { 
                            ApiResponseDefault = new DefaultApiResponse 
                            { 
                                ErrorMessage = result.Errors?.ToList().Select(f => f.Description).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}")
                            } 
                        });
                }
            }
            else
            {
                AuthenticateApiResponse badResponse = new AuthenticateApiResponse();
                badResponse.ApiResponseDefault.ErrorMessage = ModelStateHelperMethods.GetAggregateErrors(ModelState.Values);
                return BadRequest(badResponse);
            }
        }

        [HttpPost(ApiRoutes.Login)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginApiRequest loginApiRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginApiRequest.Email, loginApiRequest.Password, false, false);
                AuthenticateApiRequest authenticateRequest = new AuthenticateApiRequest(loginApiRequest.Email);
                AuthenticateApiResponse authenticateResponse = _jwtAndRtService.GetJwtAndRt(authenticateRequest, ipAddress());

                if (result.Succeeded)                
                    return Ok(authenticateResponse);
                else                
                    return BadRequest(authenticateResponse);                
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpGet("amiauthorized")]
        public IActionResult AmIAuthorized()
        {            
            return Ok(new DefaultApiResponse());
        }

        [HttpPost("getjwtandrtviart")]
        [AllowAnonymous]
        public IActionResult GetJwtAndRtViaRt(AuthenticateApiRequest authenticateRequest)
        {
            AuthenticateApiResponse authenticateResponse = _jwtAndRtService.GetJwtAndRtViaRt(authenticateRequest.RefreshToken, ipAddress());
            if (authenticateResponse != null)
                return Ok(authenticateResponse);
            else                
                return BadRequest(new DefaultApiResponse { ErrorMessage = "Действие обновляющего токена закончилось" });
        }        

        #region Helper Methods

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        #endregion
        
    }
}

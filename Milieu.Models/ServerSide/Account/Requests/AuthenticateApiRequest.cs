using System.ComponentModel.DataAnnotations;

namespace Milieu.Models.Requests
{
    public class AuthenticateApiRequest
    {
        [Required]
        public string Email { get; set; }

        public string RefreshToken { get; set; }

        public AuthenticateApiRequest()
        {

        }

        public AuthenticateApiRequest(string email)
        {
            Email = email;
        }

        public AuthenticateApiRequest(string email, string refreshToken)
        {
            Email = email;
            RefreshToken = refreshToken;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milieu.Models.Routes
{
    public static class ApiRoutes
    {
        public const string AccountController = "api/account";

        public const string Register = "register";

        public const string AccountControllerRegister = AccountController + "/" + Register;

        public const string Login = "login";

        public const string AccountControllerLogin = AccountController + "/" + Login;

        public const string AmIAuthorized = "amiauthorized";

        public const string AccountControllerAmIAuthorized = AccountController + "/" + AmIAuthorized; 

        public const string GetJwtAndRtViaRt = "getjwtandrtviart";

        public const string AccountControllerGetJwtAndRtViaRt = AccountController + "/" + GetJwtAndRtViaRt;


    }
}

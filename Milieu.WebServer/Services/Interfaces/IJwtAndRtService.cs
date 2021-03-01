using Milieu.Models.Account.Models;
using Milieu.Models.Requests;
using Milieu.Models.Responses;
using System.Collections.Generic;

namespace Milieu.WebServer.Services.Interfaces
{
    public interface IJwtAndRtService
    {
        AuthenticateApiResponse GetJwtAndRt(AuthenticateApiRequest model, string ipAddress);
        AuthenticateApiResponse GetJwtAndRtViaRt(string refreshToken, string ipAddress);
    }
}

using Milieu.Models.Account.Models;

namespace Milieu.WebServer.Data.Repos
{
    public interface IMilieuRepo
    {
        User GetUser(string Email);
        User GetUserByToken(string token);
        void UpdateOrAddRefreshToken(User user, RefreshToken newRefreshToken);
    }
}

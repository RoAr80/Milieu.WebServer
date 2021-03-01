using Milieu.Models.Account.Models;
using System.Linq;

namespace Milieu.WebServer.Data.Repos
{
    public class MilieuRepository : IMilieuRepo
    {
        // Так как пока реализовано без fingerprint устройства и всё делается будто у пользователя только одно устройство
        // Поэтому у пользователя может быть только один refresh token
        // И эта логика будет реализована здесь
        private MilieuDbContext _milieuDbContext;

        public MilieuRepository(MilieuDbContext milieuDbContext)
        {
            _milieuDbContext = milieuDbContext;
        }

        public User GetUser(string Email)
        {
            return _milieuDbContext.Users.SingleOrDefault(x => x.Email == Email);             
        }

        public User GetUserByToken(string token)
        {
            return _milieuDbContext.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Value == token));            
        }

        // ToDo: должно быть больше чем один токен, но пока он один поэтому update
        public void UpdateOrAddRefreshToken(User user, RefreshToken newRefreshToken)
        {
            //Просто берём первый элемент
            if (user.RefreshTokens.Any())
                user.RefreshTokens[0] = newRefreshToken;
            else
                user.RefreshTokens.Add(newRefreshToken);

            _milieuDbContext.SaveChanges();
        }


        private void DeleteToken(User user, RefreshToken refreshToken)
        {
            user.RefreshTokens.Remove(refreshToken);
        }

        public void AddRefreshToken(User user, RefreshToken refreshToken)
        {
            if (user.RefreshTokens.Any())
            {
                RefreshToken refreshTokenToDelete = user.RefreshTokens.FirstOrDefault();
                DeleteToken(user, refreshTokenToDelete);
            }            
            user.RefreshTokens.Add(refreshToken);
            _milieuDbContext.SaveChanges();
        }
    }
}

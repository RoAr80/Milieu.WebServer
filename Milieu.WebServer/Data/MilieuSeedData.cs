using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Milieu.Models.Account.Models;

namespace Milieu.WebServer.Data
{
    public class MilieuSeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            MilieuDbContext context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<MilieuDbContext>();

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    UserName = "test",
                    Email = "test",
                });
                context.SaveChanges();
            }
        }
    }
}

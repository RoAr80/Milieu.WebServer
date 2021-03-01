using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Milieu.Models.Account.Models;

namespace Milieu.WebServer.Data
{
    public class MilieuDbContext : IdentityDbContext<User>
    {
        public MilieuDbContext(DbContextOptions<MilieuDbContext> options)
            : base(options) { }
        
    }
}

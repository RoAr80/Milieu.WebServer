using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Milieu.Models;
using Milieu.Models.Account.Models;
using Milieu.WebServer.Data;
using Milieu.WebServer.Data.Repos;
using Milieu.WebServer.Helpers;
using Milieu.WebServer.Services;
using Milieu.WebServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Milieu.WebServer
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<MilieuDbContext>(opts =>
                opts.UseSqlServer(Configuration["ConnectionStrings:MilieuDbConnection"]));

            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<MilieuDbContext>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // ToDo: ƒумаю можно будет убрать
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthentication()
                .AddJwtBearer(opts =>
                {
                    opts.RequireHttpsMetadata = false;
                    opts.SaveToken = true;
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ClockSkew = TimeSpan.Zero
                    };
                    opts.Events = new JwtBearerEvents
                    {
                        //я не знаю что это
                        OnTokenValidated = async ctx =>
                        {
                            var usrmgr = ctx.HttpContext.RequestServices
                                .GetRequiredService<UserManager<User>>();
                            var signinmgr = ctx.HttpContext.RequestServices
                                .GetRequiredService<SignInManager<User>>();

                            string username =
                                ctx.Principal.FindFirst(ClaimTypes.Name)?.Value;
                            User idUser = await usrmgr.FindByNameAsync(username);
                            ctx.Principal =
                                await signinmgr.CreateUserPrincipalAsync(idUser);
                        }
                    };
                });

            services.AddScoped<IJwtAndRtService, JwtAndRtService>();
            services.AddScoped<IMilieuRepo, MilieuRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            MilieuSeedData.EnsurePopulated(app);
        }
    }
}

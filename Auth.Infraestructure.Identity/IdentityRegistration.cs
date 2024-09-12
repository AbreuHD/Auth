using Auth.Core.Application.DTOs.Account;
using Auth.Core.Application.Settings;
using Auth.Infraestructure.Identity.Context;
using Auth.Infraestructure.Identity.Entities;
using Auth.Infraestructure.Identity.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Reflection;
using System.Text;

namespace Auth.Infraestructure.Identity
{
    public static class IdentityRegistration
    {
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseMySql(configuration.GetConnectionString("IdentityConnection"), new MySqlServerVersion(new Version(10, 6, 16)),
                    m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName).SchemaBehavior(MySqlSchemaBehavior.Ignore));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Account";
                option.AccessDeniedPath = "/User/AccessDenied";
            });
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; //patrue
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },
                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponse
                        {
                            Message = "You're Not Authorized",
                            Success = false,
                            Statuscode = 401
                        });
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 404;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponse
                        {
                            Message = "You're Not Authorized to access to this resource",
                            Success = false,
                            Statuscode = 404
                        });
                        return c.Response.WriteAsync(result);
                    }

                };
            });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MailSettings>>().Value);
        }

        public static async Task AddIdentityRolesAsync(this IServiceProvider services)
        {
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DefaultRoles.Seed(userManager, roleManager);
                await DefaultOwner.Seed(userManager, roleManager);
                await DefaultUser.Seed(userManager, roleManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
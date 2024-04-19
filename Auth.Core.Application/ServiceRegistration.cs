using Auth.Core.Application.Interfaces.Services;
using Auth.Core.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Auth.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddAuthServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IUserService, UserService>();
        }
    }
}
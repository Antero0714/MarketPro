using MarketPro.Application.Interfaces.Services;
using MarketPro.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MarketPro.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services)
        {
            services.AddServices();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();
        }
    }
}   

using Marketplace.BLL.Interfaces;
using Marketplace.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.BLL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddScoped<IMarketplaceProbeService, MarketplaceProbeService>();
            return services;
        }
    }
}

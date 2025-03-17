using Microsoft.Extensions.DependencyInjection;
using TWStockLib.Cache;
using TWStockLib.Factory;

namespace TWStockLib.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStockServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IStockMarketFactory, TwseMarketFactory>();
            services.AddScoped<StockMarketService>();
            return services;
        }
    }
} 
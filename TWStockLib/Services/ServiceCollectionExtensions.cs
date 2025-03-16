using Microsoft.Extensions.DependencyInjection;
using TWStockLib.Cache;
using TWStockLib.Factory;

namespace TWStockLib.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStockServices(this IServiceCollection services)
        {
            // 註冊 HttpClient
            services.AddHttpClient();
            
            // 註冊快取服務
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            
            // 註冊工廠
            services.AddSingleton<IStockMarketFactory, TwseMarketFactory>();
            
            // 註冊主服務
            services.AddScoped<StockMarketService>();
            
            return services;
        }
    }
} 
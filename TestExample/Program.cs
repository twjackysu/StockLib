using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using StockLib;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestExample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Stock Test Example Start");
            try
            {
                var config = new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();

                var servicesProvider = BuildDi(config);
                using (servicesProvider as IDisposable)
                {
                    var historyBuilder = servicesProvider.GetRequiredService<HistoryBuilder>();
                    var stockInfoBuilder = servicesProvider.GetRequiredService<StockInfoBuilder>();
                    var listBuilder = servicesProvider.GetRequiredService<StockListBuilderFromWeb>();

                    var tseHistory = await historyBuilder.GetStockHistories("1101", new DateTime(2020, 4, 1), StockType.TSE);
                    var otcHistory = await historyBuilder.GetStockHistories("5015", new DateTime(2000, 11, 1), StockType.OTC);

                    var stockList = await listBuilder.GetAllStockListAsync();
                    var searchStockList = new string[] { "2439", "2330", "2317", "3679", "3548", "4942" };
                    var queries = searchStockList.Select(
                                x => (stockList[x].Type, x)
                            ).ToArray();

                    var stockInfos = await stockInfoBuilder.GetStocksInfo(queries);
                }
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                logger.Info("Stock Test Example End");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }
        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
               .AddTransient<HistoryBuilder>()
               .AddTransient<StockInfoBuilder>()
               .AddTransient<StockListBuilderFromWeb>()
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog(config);
               })
               .BuildServiceProvider();
        }
    }
}

using Microsoft.Extensions.Logging;
using TWStockLib.Cache;
using TWStockLib.Observer;
using TWStockLib.Strategy;
using System;
using System.Net.Http;

namespace TWStockLib.Factory
{
    public class TwseMarketFactory : IStockMarketFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService _cacheService;
        private readonly ILoggerFactory _loggerFactory;

        public TwseMarketFactory(IHttpClientFactory httpClientFactory, ICacheService cacheService, ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
            _loggerFactory = loggerFactory;
        }

        public IDataFetchStrategy CreateDataFetchStrategy()
        {
            return new TwseDataFetchStrategy(
                _httpClientFactory, 
                _cacheService, 
                _loggerFactory.CreateLogger<TwseDataFetchStrategy>());
        }

        public IStockPriceObserver CreatePriceObserver(string observerName)
        {
            return new ConsoleStockPriceObserver(observerName);
        }
    }

    public class ConsoleStockPriceObserver : IStockPriceObserver
    {
        private readonly string _name;

        public ConsoleStockPriceObserver(string name)
        {
            _name = name;
        }

        public void OnPriceChanged(string symbol, decimal newPrice, decimal oldPrice)
        {
            var changePercentage = (newPrice - oldPrice) / oldPrice * 100;
            var direction = newPrice > oldPrice ? "上漲" : "下跌";
            
            Console.WriteLine($"[{_name}] 股票 {symbol} {direction}: 從 {oldPrice} 到 {newPrice} ({changePercentage:F2}%)");
        }
    }
} 
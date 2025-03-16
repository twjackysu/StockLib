using StockLib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockLib.Strategy
{
    public interface IDataFetchStrategy
    {
        Task<StockQuote> FetchRealtimeQuote(string symbol, MarketType marketType);
        Task<IEnumerable<StockHistory>> FetchHistoricalData(string symbol, DateTime startDate, DateTime endDate, MarketType marketType);
        Task<Dictionary<string, StockData>> FetchStockList(MarketType marketType, bool includeWarrant = false);
    }
} 
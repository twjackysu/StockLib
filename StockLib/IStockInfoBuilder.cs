using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockLib
{
    public interface IStockInfoBuilder
    {
        Task<List<StockInfo>> GetStocksInfo(params (StockType type, string stockNo)[] queries);
        Task<List<StockInfo>> GetStocksInfo(Dictionary<string, StockType> queries, DateTime? SpecifiedDate = null);
        Task<List<StockInfo>> GetStockInfo(string stockList);
    }
}

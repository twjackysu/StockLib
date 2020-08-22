using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockLib
{
    public interface IStockInfoBuilder
    {
        /// <summary>
        /// 獲得即時股票資料，用ValueTuple
        /// </summary>
        /// <param name="queries">哪些股票</param>
        /// <returns></returns>
        Task<Dictionary<string, StockInfo>> GetStocksInfo(params (StockType type, string stockNo)[] queries);
        /// <summary>
        /// 獲得即時股票資料，用Dictionary
        /// </summary>
        /// <param name="queries">哪些股票</param>
        /// <param name="SpecifiedDate">指定日期(有時失效)，請不要使用</param>
        /// <returns></returns>
        Task<Dictionary<string, StockInfo>> GetStocksInfo(Dictionary<string, StockType> queries, DateTime? SpecifiedDate = null);
    }
}

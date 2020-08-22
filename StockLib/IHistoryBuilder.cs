using System;
using System.Threading.Tasks;

namespace StockLib
{
    public interface IHistoryBuilder
    {
        /// <summary>
        /// 獲得單一股票的歷史股價，一次獲得一個月的股價
        /// </summary>
        /// <param name="stockNo">股票代號</param>
        /// <param name="dateTime">哪年哪月</param>
        /// <param name="type">TSE or OTC</param>
        /// <returns></returns>
        Task<StockHistory[]> GetStockHistories(string stockNo, DateTime dateTime, StockType type);
    }
}

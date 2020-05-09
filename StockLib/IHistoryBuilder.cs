using System;
using System.Threading.Tasks;

namespace StockLib
{
    public interface IHistoryBuilder
    {
        Task<StockHistory[]> GetStockHistories(string stockNo, DateTime dateTime, StockType type);
    }
}

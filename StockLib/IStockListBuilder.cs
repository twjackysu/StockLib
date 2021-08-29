using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockLib
{
    public interface IStockListBuilder
    {
        /// <summary>
        /// 獲得所有股票清單
        /// </summary>
        /// <param name="includeWarrant">包含權證</param>
        /// <returns></returns>
        Task<Dictionary<string, Stock>> GetAllStockListAsync(bool includeWarrant = false);
        /// <summary>
        /// 獲得所有上櫃股票清單
        /// </summary>
        /// <param name="includeWarrant">包含權證</param>
        /// <returns></returns>
        Task<Dictionary<string, Stock>> GetOTCListAsync(bool includeWarrant = false);
        /// <summary>
        /// 獲得所有上市股票清單
        /// </summary>
        /// <param name="includeWarrant">包含權證</param>
        /// <returns></returns>
        Task<Dictionary<string, Stock>> GetTSEListAsync(bool includeWarrant = false);
    }
}

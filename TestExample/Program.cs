using System;
using StockLib;
using System.Linq;
using System.Threading.Tasks;

namespace TestExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TSEOTCList test = new TSEOTCList();
            var OTCList = test.GetOTCList();
            var searchStockList = new string[] { "2439", "2330", "2317" };
            var queries = searchStockList.Select(
                        x => OTCList.Contains(x) ? (StockType.OTC, x) : (StockType.TSE, x)
                    ).ToArray();
            StockInfoBuilder stockInfoBuilder = new StockInfoBuilder();
            var stockInfos = await stockInfoBuilder.GetStocksInfo(true, queries);
        }
    }
}

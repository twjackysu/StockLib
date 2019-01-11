using System;
using StockLib;
using System.Linq;
using System.Threading.Tasks;

namespace TestExample
{
    class Program
    {
        static void Main(string[] args)
        {
            TSEOTCList test = new TSEOTCList();
            var OTCList = test.GetOTCList();
            var searchStockList = new string[] { "2439", "2408", "3088" };
            var queries = searchStockList.Select(
                        x => OTCList.Any(y => y == x) ? (StockType.OTC, x) : (StockType.TSE, x)
                    ).ToArray();
            StockInfoBuilder stockInfoBuilder = new StockInfoBuilder();
            var stockInfo = stockInfoBuilder.GetStocksInfo(false, queries);

            Task.WaitAll(stockInfo);
            var result = stockInfo.Result;
            Console.ReadKey();
        }
    }
}

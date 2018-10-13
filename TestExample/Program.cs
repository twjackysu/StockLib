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
            var searchStockList = new string[] { "2317", "2330", "3088" };
            StockInfoBuilder stockInfoBuilder = new StockInfoBuilder();
            var stockInfo = stockInfoBuilder.GetStocksInfo(
                searchStockList.Select(
                        x => OTCList.Any(y => y == x) ? new StockQuery(StockType.OTC, x) : new StockQuery(StockType.TSE, x)
                    ).ToArray()
                );
            Task.WaitAll(stockInfo);
            var result = stockInfo.Result;
            Console.ReadKey();
        }
    }
}

using StockLib.Observer;
using StockLib.Strategy;

namespace StockLib.Factory
{
    public interface IStockMarketFactory
    {
        IDataFetchStrategy CreateDataFetchStrategy();
        IStockPriceObserver CreatePriceObserver(string observerName);
    }
} 
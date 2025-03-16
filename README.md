# StockLib
自製的台股類別庫，所有source都是來自網路，因此無法離線搜尋，必須連網，code內有使用範例，這是 .net core的程式，因為我有要跑在linux上的需要，若沒有 .net core sdk的請自行安裝
___

## 新版本使用方法

### 使用依賴注入
```C#
// 在 Startup.cs 或 Program.cs 中註冊服務
services.AddStockServices();
```

### 使用 StockMarketService
```C#
public class YourClass
{
    private readonly StockMarketService _stockMarketService;

    public YourClass(StockMarketService stockMarketService)
    {
        _stockMarketService = stockMarketService;
    }

    public async Task DoSomething()
    {
        // 獲取股票清單
        var allStocks = await _stockMarketService.GetAllStockList();
        var tseStocks = await _stockMarketService.GetStockList(MarketType.TSE);
        var otcStocks = await _stockMarketService.GetStockList(MarketType.OTC);

        // 獲取即時報價
        var quote = await _stockMarketService.GetRealtimeQuote("2330", MarketType.TSE);
        Console.WriteLine($"台積電目前價格: {quote.LastPrice}");

        // 獲取歷史資料
        var history = await _stockMarketService.GetHistoricalData(
            "2330", 
            DateTime.Now.AddMonths(-1), 
            DateTime.Now,
            MarketType.TSE);
        
        foreach (var day in history)
        {
            Console.WriteLine($"{day.Date:yyyy-MM-dd}: 開盤 {day.OpeningPrice}, 收盤 {day.ClosingPrice}");
        }

        // 使用觀察者模式監控價格變化
        var observer = _stockMarketService.CreatePriceObserver("MyObserver");
        _stockMarketService.SubscribePriceChanges("2330", observer);
        
        // 當價格變化時，observer 會收到通知
        
        // 取消訂閱
        _stockMarketService.UnsubscribePriceChanges("2330", observer);
    }
}
```

## 舊版本使用方法

用法可參考我另外一個Repo: [StockJob](https://github.com/TWKuanLun/StockJob) 

使用DI
```C#
new ServiceCollection()
.AddTransient<IHistoryBuilder, HistoryBuilder>()
.AddTransient<IStockInfoBuilder, StockInfoBuilder>()
.AddTransient<IStockListBuilder, StockListBuilderFromWeb>()
```
在任何你的Class內使用
```C#
class YourClass
{
    private readonly IHistoryBuilder historyBuilder;
    private readonly StockListBuilderFromWeb stockListBuilder;
    private readonly IStockInfoBuilder stockInfoBuilder;
    public YourClass(IHistoryBuilder historyBuilder, StockListBuilderFromWeb stockListBuilder, IStockInfoBuilder stockInfoBuilder)
    {
        this.historyBuilder = historyBuilder;
        this.stockListBuilder = stockListBuilder;
        this.stockInfoBuilder = stockInfoBuilder;
    }
}
```
獲得目前上市與上櫃的代碼列表
```C#
//目前所有上市+上櫃公司清單
var allStockList = await stockListBuilder.GetAllStockListAsync();
//目前所有上市公司清單
var tseList = await stockListBuilder.GetTSEListAsync();
//目前所有上櫃公司清單
var otcList = await stockListBuilder.GetOTCListAsync();
```

使用HistoryBuilder來搜尋歷史股價資訊，一次只能查一支股票一個月的資訊(datatime的日期無視)
```C#
var tseHistory = historyBuilder.GetStockHistories("9911", new DateTime(2017, 12, 1), StockType.TSE);
var otcHistory = historyBuilder.GetStockHistories("3088", new DateTime(2017, 12, 1), StockType.OTC);
``` 

StockInfoBuilder可使用ValueTuple(StockType, StockNo)來搜尋股票資訊，可以是多個
```C#
var stockInfo = await stockInfoBuilder.GetStocksInfo((StockType.TSE, "2317"), (StockType.OTC, "5015"));
```

或改Dictionary可以指定日期大約只支援目前日期的前一個禮拜，太久的無法查。(太久遠的請用HistoryBuilder查)
```C#
var stockInfos = await stockInfoBuilder.GetStocksInfo(queries, new DateTime(2020, 5, 6));
```

兩個builder資料範圍有點不太一樣，請自行決定要使用哪個
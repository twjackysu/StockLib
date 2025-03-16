# TWStockLib

TWStockLib 是一個用於獲取台灣股市資料的 .NET 類別庫。它提供了獲取股票清單、歷史數據和即時報價的功能，並支援價格變化的觀察者模式。

## 功能特點

- 獲取上市（TSE）和上櫃（OTC）股票清單
- 獲取股票歷史數據
- 獲取股票即時報價
- 監控股票價格變化（觀察者模式）
- 支援快取機制，減少 API 請求
- 完整的錯誤處理和日誌記錄

## 快速入門

### 步驟 1: 註冊服務

在 `Program.cs` 或 `Startup.cs` 中註冊 TWStockLib 服務：

```csharp
using Microsoft.Extensions.DependencyInjection;
using TWStockLib.Services;
using System.Text;

// 註冊編碼提供者，以支援 950 (繁體中文 Big5) 編碼
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var services = new ServiceCollection();

// 添加 TWStockLib 服務
services.AddStockServices();

var serviceProvider = services.BuildServiceProvider();
```

### 步驟 2: 獲取 StockMarketService

```csharp
var stockMarketService = serviceProvider.GetRequiredService<StockMarketService>();
```

### 步驟 3: 使用服務功能

#### 獲取股票清單

```csharp
// 獲取所有股票
var allStocks = await stockMarketService.GetAllStockList();

// 獲取上市股票
var tseStocks = await stockMarketService.GetStockList(MarketType.TSE);

// 獲取上櫃股票
var otcStocks = await stockMarketService.GetStockList(MarketType.OTC);
```

#### 獲取歷史數據

```csharp
// 獲取上市股票歷史數據
var tseHistory = await stockMarketService.GetHistoricalData(
    "2330",                     // 股票代碼
    new DateTime(2023, 1, 1),   // 開始日期
    new DateTime(2023, 1, 31),  // 結束日期
    MarketType.TSE              // 市場類型
);

// 獲取上櫃股票歷史數據
var otcHistory = await stockMarketService.GetHistoricalData(
    "6510",                     // 股票代碼
    new DateTime(2023, 1, 1),   // 開始日期
    new DateTime(2023, 1, 31),  // 結束日期
    MarketType.OTC              // 市場類型
);

// 使用歷史數據
foreach (var record in tseHistory)
{
    Console.WriteLine($"日期: {record.Date:yyyy-MM-dd}, 開盤: {record.OpeningPrice}, 收盤: {record.ClosingPrice}");
}
```

#### 獲取即時報價

```csharp
// 獲取上市股票即時報價
var tseQuote = await stockMarketService.GetRealtimeQuote("2330", MarketType.TSE);

// 獲取上櫃股票即時報價
var otcQuote = await stockMarketService.GetRealtimeQuote("6510", MarketType.OTC);

// 使用即時報價
if (tseQuote != null)
{
    Console.WriteLine($"股票: {tseQuote.Symbol} {tseQuote.Name}");
    Console.WriteLine($"最新價格: {tseQuote.LastPrice}");
    Console.WriteLine($"最高價: {tseQuote.HighestPrice}");
    Console.WriteLine($"最低價: {tseQuote.LowestPrice}");
    Console.WriteLine($"開盤價: {tseQuote.OpeningPrice}");
    Console.WriteLine($"昨收價: {tseQuote.YesterdayClosingPrice}");
    Console.WriteLine($"成交量: {tseQuote.TotalVolume}");
}
```

#### 使用觀察者模式監控價格變化

```csharp
// 創建觀察者
public class MyPriceObserver : IStockPriceObserver
{
    public void OnPriceChanged(string symbol, decimal newPrice, decimal oldPrice)
    {
        var changePercentage = (newPrice - oldPrice) / oldPrice * 100;
        var direction = newPrice > oldPrice ? "上漲" : "下跌";
        
        Console.WriteLine($"股票 {symbol} {direction}: 從 {oldPrice} 到 {newPrice} ({changePercentage:F2}%)");
    }
}

// 使用內建的觀察者
var observer = stockMarketService.CreatePriceObserver("MyObserver");

// 或使用自定義觀察者
var myObserver = new MyPriceObserver();

// 訂閱價格變化
stockMarketService.SubscribePriceChanges("2330", observer);

// 獲取報價時會自動通知觀察者
var quote = await stockMarketService.GetRealtimeQuote("2330", MarketType.TSE);

// 取消訂閱
stockMarketService.UnsubscribePriceChanges("2330", observer);
```

## 完整範例

```csharp
using Microsoft.Extensions.DependencyInjection;
using TWStockLib.Models;
using TWStockLib.Observer;
using TWStockLib.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLibExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 註冊編碼提供者，以支援 950 (繁體中文 Big5) 編碼
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            // 設置服務
            var services = new ServiceCollection();
            services.AddStockServices();
            var serviceProvider = services.BuildServiceProvider();
            
            // 獲取 StockMarketService
            var stockMarketService = serviceProvider.GetRequiredService<StockMarketService>();
            
            // 創建觀察者
            var observer = stockMarketService.CreatePriceObserver("MyObserver");
            
            try
            {
                // 獲取股票清單
                Console.WriteLine("獲取股票清單...");
                var allStocks = await stockMarketService.GetAllStockList();
                Console.WriteLine($"總共獲取到 {allStocks.Count} 支股票");
                
                // 獲取歷史數據
                Console.WriteLine("獲取歷史數據...");
                var tseHistory = await stockMarketService.GetHistoricalData(
                    "2330", 
                    new DateTime(2023, 1, 1), 
                    new DateTime(2023, 1, 31),
                    MarketType.TSE);
                
                Console.WriteLine($"2330 歷史數據: {tseHistory.Count()} 筆");
                
                // 訂閱價格變化
                stockMarketService.SubscribePriceChanges("2330", observer);
                
                // 獲取即時報價
                Console.WriteLine("獲取即時報價...");
                var quote = await stockMarketService.GetRealtimeQuote("2330", MarketType.TSE);
                
                if (quote != null)
                {
                    Console.WriteLine($"2330 {quote.Name} 目前價格: {quote.LastPrice}");
                }
                else
                {
                    Console.WriteLine("無法獲取 2330 的報價");
                }
                
                // 取消訂閱
                stockMarketService.UnsubscribePriceChanges("2330", observer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤: {ex.Message}");
            }
        }
    }
}
```

## 注意事項

1. 所有資料來源都是從網路獲取，因此需要連接網路才能使用。
2. 歷史數據的獲取是按月為單位，如果指定的日期範圍跨越多個月，會自動獲取所有相關月份的數據。
3. 即時報價數據有 30 秒的快取時間，以避免過多的 API 請求。
4. 股票清單數據有 1 天的快取時間。

## 支援的市場

- TSE (台灣證券交易所，上市)
- OTC (櫃買中心，上櫃)

## 授權

本專案採用 MIT 授權。詳情請參閱 [LICENSE](LICENSE) 文件。
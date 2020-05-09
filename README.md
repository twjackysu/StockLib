# StockLib
自製的台股類別庫，所有source都是來自網路，因此無法離線搜尋，必須連網，code內有使用範例，這是.net core的console程式，因為我有要跑在linux上的需要，若沒有.net core sdk的請自行安裝
___
獲得目前上市與上櫃的代碼列表
```C#
var TSEList = tseOTCListBuilder.GetTSEList();//目前所有上市公司清單
var OTCList = tseOTCListBuilder.GetOTCList();//目前所有上櫃公司清單
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
var stockInfos = await stockInfoBuilder.GetStocksInf(queries, new DateTime(2020, 5, 6));
```

兩個builder資料範圍有點不太一樣，請自行決定要使用哪個
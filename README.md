# StockLib
自製的台股類別庫，所有source都是來自網路，因此無法離線搜尋，必須連網，code內有使用範例
這是.net core的console程式，因為我有要跑在linux上的需要，若沒有.net core sdk的請自行安裝
___
獲得目前上市與上櫃的代碼列表
```C#
var temp = new TSEOTCList();
var TSEList = temp.GetTSEList();//上市
var OTCList = temp.GetOTCList();//上櫃
```
使用new StockQuery(StockType, StockNo)來搜尋股票資訊，可以是多個
```C#
var stockInfoBuilder = new StockInfoBuilder();
var stockInfo = await stockInfoBuilder.GetStocksInfo(new StockQuery(StockType.TSE, "2317"), new StockQuery(StockType.OTC, "5015"));
```

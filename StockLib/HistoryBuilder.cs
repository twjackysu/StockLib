using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StockLib
{
    public class HistoryBuilder : IHistoryBuilder
    {
        private readonly ILogger<HistoryBuilder> logger;
        public HistoryBuilder(ILogger<HistoryBuilder> logger)
        {
            this.logger = logger;
        }
        public async Task<StockHistory[]> GetStockHistories(string stockNo, DateTime dateTime, StockType type)
        {
            var result = new List<StockHistory>();
            try
            {
                switch (type)
                {
                    case StockType.OTC:
                        var tGetData = await GetTPEXData(stockNo, dateTime);
                        var tpex = JsonConvert.DeserializeObject<TPEXAPIModel>(tGetData);
                        var firstTable = tpex.tables.FirstOrDefault();
                        foreach (var data in firstTable.data)
                        {
                            try
                            {
                                var history = new StockHistory(data);
                                result.Add(history);
                            }
                            catch (Exception e)
                            {
                                logger.LogWarning(e, $"new StockHistory Error. GetStockHistories({stockNo}, {dateTime:yyyy-MM}, {type}). Data: {JsonConvert.SerializeObject(data)}");
                            }
                        }
                        break;
                    case StockType.TSE:
                        tGetData = await GetTWSEData(stockNo, dateTime);
                        var twse = JsonConvert.DeserializeObject<TWSEAPIModel>(tGetData);
                        if (twse.data == null)
                        {
                            logger.LogWarning($"No Data. GetStockHistories({stockNo}, {dateTime:yyyy-MM}, {type})");
                        }
                        else
                        {
                            foreach (var data in twse.data)
                            {
                                try
                                {
                                    var history = new StockHistory(data);
                                    result.Add(history);
                                }
                                catch (Exception e)
                                {
                                    logger.LogWarning(e, $"new StockHistory Error. GetStockHistories({stockNo}, {dateTime:yyyy-MM}, {type}). Data: {JsonConvert.SerializeObject(data)}");
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetStockHistories({stockNo}, {dateTime:yyyy-MM}, {type})");
                if(e is HttpRequestException)
                {
                    //IP被鎖
                    throw;
                }
            }
            return result.ToArray();
        }

        private async Task<string> GetTWSEData(string stockNo, DateTime dateTime)
        {
            using (var client = new HttpClient())
            {

                var url = $"https://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&date={dateTime:yyyyMM01}&stockNo={stockNo}";
                return await client.GetStringAsync(url);
            }
        }
        private async Task<string> GetTPEXData(string stockNo, DateTime dateTime)
        {
            using (var client = new HttpClient())
            {
                // TODO: 去https://www.tpex.org.tw/zh-tw/mainboard/trading/info/stock-pricing.html
                // 改成用https://www.tpex.org.tw/www/zh-tw/afterTrading/tradingStock
                var url = $"http://www.tpex.org.tw/www/zh-tw/afterTrading/tradingStock/st43_result.php?l=zh-tw&date={dateTime.ToString("yyyy/MM/dd")}&code={stockNo}";
                return await client.GetStringAsync(url);
            }
        }
    }
    public class TWSEAPIModel
    {
        public string stat { get; set; }
        public string date { get; set; }
        public string title { get; set; }
        public List<string> fields { get; set; }
        public List<List<string>> data { get; set; }
        public List<string> notes { get; set; }
    }
    public class TPEXAPIModel
    {
        public List<Table> tables { get; set; }
        public string date { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool showListPriceNote { get; set; }
        public bool showListPriceLink { get; set; }
        public string stat { get; set; }

        public class Table
        {
            public string title { get; set; }
            public string subtitle { get; set; }
            public string date { get; set; }
            public List<List<string>> data { get; set; }
            public List<string> fields { get; set; }
            public List<string> notes { get; set; }
            public int totalCount { get; set; }
            public List<string> summary { get; set; }
        }
    }
}

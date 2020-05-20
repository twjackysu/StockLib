using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
                        foreach (var data in tpex.aaData)
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
                    throw e;
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
                var dateMonthStr = $"{dateTime.Year - 1911}/{dateTime.Month}";
                var url = $"http://www.tpex.org.tw/web/stock/aftertrading/daily_trading_info/st43_result.php?l=zh-tw&d={dateMonthStr}&stkno={stockNo}";
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
        public string stkNo { get; set; }
        public string stkName { get; set; }
        public bool showListPriceNote { get; set; }
        public bool showListPriceLink { get; set; }
        public string reportDate { get; set; }
        public int iTotalRecords { get; set; }
        public List<List<string>> aaData { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace StockLib
{
    public class HistoryBuilder
    {
        public StockHistory[] GetStockHistories(string stockNo, DateTime dateTime, StockType type)
        {
            List<StockHistory> result = new List<StockHistory>();
            var AMonthAgo = dateTime.AddMonths(-1);
            try
            {
                switch (type)
                {
                    case StockType.OTC:
                        var tGetData = GetTPEXData(stockNo, dateTime);
                        var tGetAMonthAgoData = GetTPEXData(stockNo, AMonthAgo);
                        Task.WaitAll(tGetData, tGetAMonthAgoData);
                        var tpex = JsonConvert.DeserializeObject<TPEXAPIModel>(tGetData.Result);
                        var tpexAMonthAgo = JsonConvert.DeserializeObject<TPEXAPIModel>(tGetAMonthAgoData.Result);
                        foreach (var data in tpex.aaData)
                        {
                            result.Add(new StockHistory(data));
                        }
                        foreach (var data in tpexAMonthAgo.aaData)
                        {
                            result.Add(new StockHistory(data));
                        }
                        break;
                    case StockType.TSE:
                        tGetData = GetTWSEData(stockNo, dateTime);
                        tGetAMonthAgoData = GetTWSEData(stockNo, AMonthAgo);
                        Task.WaitAll(tGetData, tGetAMonthAgoData);
                        var twse = JsonConvert.DeserializeObject<TWSEAPIModel>(tGetData.Result);
                        var twseAMonthAgo = JsonConvert.DeserializeObject<TWSEAPIModel>(tGetAMonthAgoData.Result);
                        foreach (var data in twse.data)
                        {
                            result.Add(new StockHistory(data));
                        }
                        foreach (var data in twseAMonthAgo.data)
                        {
                            result.Add(new StockHistory(data));
                        }
                        break;
                }
            }
            catch (AggregateException e)
            {
                //throw e;
            }
            return result.ToArray();
        }

        private async Task<string> GetTWSEData(string stockNo, DateTime dateTime)
        {
            using (var client = new HttpClient())
            {

                var url = $"http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=json&date={dateTime.ToString("yyyyMM01")}&stockNo={stockNo}";
                return await client.GetStringAsync(url);
            }
        }
        private async Task<string> GetTPEXData(string stockNo, DateTime dateTime)
        {
            using (var client = new HttpClient())
            {
                var dateMonthStr = $"{dateTime.Year - 1911}/{dateTime.Month}";
                var url = $"http://www.tpex.org.tw/web/stock/aftertrading/daily_trading_info/st43_result.php?d={dateMonthStr}&stkno={stockNo}";
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

using Newtonsoft.Json.Linq;
using StockLib.DateTimeExtension;
using StockLib.EnumExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockLib
{
    public class StockInfoBuilder: IStockInfoBuilder
    {
        public async Task<List<StockInfo>> GetStocksInfo(bool needHistory, params StockQuery[] queries)
        {
            if (queries.Length < 1)
                return new List<StockInfo>();
            var keys = new List<string>();
            foreach (var query in queries)
            {
                if (string.IsNullOrEmpty(query.StockNo))
                    continue;
                keys.Add(query.Type.ToKey(query.StockNo));
            }
            var stockList = string.Join("%257c", keys);
            return await GetStockInfo(stockList, needHistory);
        }
        public async Task<List<StockInfo>> GetStocksInfo(bool needHistory, Dictionary<string, StockType> queries)
        {
            if (queries.Count < 1)
                return new List<StockInfo>();
            var keys = new List<string>();
            foreach (var query in queries)
            {
                if (string.IsNullOrEmpty(query.Key))
                    continue;
                keys.Add(query.Value.ToKey(query.Key));
            }
            var stockList = string.Join("%257c", keys);
            return await GetStockInfo(stockList, needHistory);
        }
        private async Task<List<StockInfo>> GetStockInfo(string stockList, bool needHistory)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var json = await httpClient.GetStringAsync($"http://mis.twse.com.tw/stock/api/getStockInfo.jsp?ex_ch={stockList}&json=1&delay=0&_={DateTime.UtcNow.ToJavascriptGetTime()}");
                try
                {
                    var jObj = JObject.Parse(json);
                    var jArray = jObj["msgArray"] as JArray;
                    var result = new List<StockInfo>();
                    foreach (JObject item in jArray)
                    {
                        var stockInfo = new StockInfo();
                        stockInfo.No = item["c"].ToString();
                        stockInfo.Type = item["ex"].ToString() == StockType.OTC.ToString().ToLower() ? StockType.OTC : StockType.TSE;
                        stockInfo.Name = item["n"].ToString();
                        stockInfo.FullName = item["nf"].ToString();
                        stockInfo.LastTradedPrice = Convert.ToSingle(item["z"]);
                        stockInfo.LastVolume = Convert.ToUInt32(item["tv"]);
                        stockInfo.TotalVolume = Convert.ToUInt32(item["v"]);
                        stockInfo.Top5SellPrice = item["a"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToSingle(x)).ToArray();
                        stockInfo.Top5SellVolume = item["f"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToUInt32(x)).ToArray();
                        stockInfo.Top5BuyPrice = item["b"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToSingle(x)).ToArray();
                        stockInfo.Top5BuyVolume = item["g"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToUInt32(x)).ToArray();
                        stockInfo.SyncTime = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToUInt64(item["tlong"]));
                        stockInfo.HighestPrice = Convert.ToSingle(item["h"]);
                        stockInfo.LowestPrice = Convert.ToSingle(item["l"]);
                        stockInfo.OpeningPrice = Convert.ToSingle(item["o"]);
                        stockInfo.YesterdayClosingPrice = Convert.ToSingle(item["y"]);
                        stockInfo.LimitUp = Convert.ToSingle(item["u"]);
                        stockInfo.LimitDown = Convert.ToSingle(item["w"]);
                        if (needHistory)
                        {
                            var historyBuilder = new HistoryBuilder();
                            stockInfo.StockHistory = historyBuilder.GetStockHistories(stockInfo.No, DateTime.UtcNow, stockInfo.Type);
                        }
                        result.Add(stockInfo);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}

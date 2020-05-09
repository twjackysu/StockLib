using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StockLib.DateTimeExtension;
using StockLib.EnumExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockLib
{
    public class StockInfoBuilder : IStockInfoBuilder
    {
        private readonly ILogger<StockInfoBuilder> logger;
        public StockInfoBuilder(ILogger<StockInfoBuilder> logger)
        {
            this.logger = logger;
        }
        public async Task<List<StockInfo>> GetStocksInfo(params (StockType type, string stockNo)[] queries)
        {
            try
            {
                if (queries.Length < 1)
                    return new List<StockInfo>();
                var keys = new List<string>();
                foreach (var query in queries)
                {
                    if (string.IsNullOrEmpty(query.stockNo))
                        continue;
                    keys.Add(query.type.ToKey(query.stockNo));
                }
                var stockList = string.Join("%257c", keys);
                return await GetStockInfo(stockList);
            }
            catch(Exception e)
            {
                var queriesParaStr = $"[({string.Join("),(", queries.Select(x => $"{x.type}, {x.stockNo}"))})]";
                logger.LogError(e, $"Error when GetStockInfo({queriesParaStr})");
                return null;
            }
        }
        public async Task<List<StockInfo>> GetStocksInfo(Dictionary<string, StockType> queries, DateTime? SpecifiedDate = null)
        {
            try
            {
                if (queries.Count < 1)
                    return new List<StockInfo>();
                var keys = new List<string>();
                foreach (var query in queries)
                {
                    if (string.IsNullOrEmpty(query.Key))
                        continue;
                    var key = SpecifiedDate.HasValue ? $"{query.Value.ToKey(query.Key)}_{SpecifiedDate.Value:yyyyMMdd}" : query.Value.ToKey(query.Key);
                    keys.Add(key);
                }
                var stockList = string.Join("%257c", keys);
                return await GetStockInfo(stockList);
            }
            catch (Exception e)
            {
                var queriesParaStr = $"{{{string.Join(",", queries.Select(x => $"{x.Key}: {x.Value}"))}}}";
                var dataParaStr = SpecifiedDate.HasValue ? SpecifiedDate.Value.ToString("yyyyMMdd") : null;
                logger.LogError(e, $"Error when GetStockInfo({queriesParaStr}, {dataParaStr})");
                return null;
            }
        }

        public async Task<List<StockInfo>> GetStockInfo(string stockList)
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
                        stockInfo.LastTradedPrice = item["z"] == null || item["z"].ToString() == "-" ? (float?)null : Convert.ToSingle(item["z"]);
                        stockInfo.LastVolume = item["tv"].ToString() == "-" ? 0 : Convert.ToUInt32(item["tv"]);
                        stockInfo.TotalVolume = Convert.ToUInt32(item["v"]);
                        stockInfo.Top5SellPrice = item["a"] == null || item["a"].ToString() == "-" ? new float[] { } : item["a"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToSingle(x)).ToArray();
                        stockInfo.Top5SellVolume = item["f"] == null || item["f"].ToString() == "-" ? new uint[] { } : item["f"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToUInt32(x)).ToArray();
                        stockInfo.Top5BuyPrice = item["b"] == null || item["b"].ToString() == "-" ? new float[] { } : item["b"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToSingle(x)).ToArray();
                        stockInfo.Top5BuyVolume = item["g"] == null || item["g"].ToString() == "-" ? new uint[] { } : item["g"].ToString().Split('_').Where(x => x.Length > 0).Select(x => Convert.ToUInt32(x)).ToArray();
                        stockInfo.SyncTime = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToUInt64(item["tlong"]));
                        stockInfo.HighestPrice = item["h"] == null || item["h"].ToString() == "-" ? (float?)null : Convert.ToSingle(item["h"]);
                        stockInfo.LowestPrice = item["l"] == null || item["l"].ToString() == "-" ? (float?)null : Convert.ToSingle(item["l"]);
                        stockInfo.OpeningPrice = item["o"] == null || item["o"].ToString() == "-" ? (float?)null : Convert.ToSingle(item["o"]);
                        stockInfo.YesterdayClosingPrice = Convert.ToSingle(item["y"]);
                        stockInfo.LimitUp = Convert.ToSingle(item["u"]);
                        stockInfo.LimitDown = Convert.ToSingle(item["w"]);
                        result.Add(stockInfo);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error when GetStockInfo({stockList})");
                    return null;
                }
            }
        }
    }
}

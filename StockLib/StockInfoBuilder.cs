using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using DotnetSdkUtilities.Extensions.DateTimeExtensions;
using StockLib.EnumExtension;

namespace StockLib
{
    public class StockInfoBuilder : IStockInfoBuilder
    {
        private readonly ILogger<StockInfoBuilder> logger;
        public StockInfoBuilder(ILogger<StockInfoBuilder> logger)
        {
            this.logger = logger;
        }
        public async Task<Dictionary<string, StockInfo>> GetStocksInfo(params (StockType type, string stockNo)[] queries)
        {
            try
            {
                if (queries.Length < 1)
                    return new Dictionary<string, StockInfo>();
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
        public async Task<Dictionary<string, StockInfo>> GetStocksInfo(Dictionary<string, StockType> queries, DateTime? SpecifiedDate = null)
        {
            try
            {
                if (queries.Count < 1)
                    return new Dictionary<string, StockInfo>();
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

        private async Task<Dictionary<string, StockInfo>> GetStockInfo(string stockList)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var json = await httpClient.GetStringAsync($"http://mis.twse.com.tw/stock/api/getStockInfo.jsp?ex_ch={stockList}&json=1&delay=0&_={DateTime.UtcNow.ToEpochMilliseconds()}");
                try
                {
                    var jObj = JObject.Parse(json);
                    var jArray = jObj["msgArray"] as JArray;
                    var result = new Dictionary<string, StockInfo>();
                    if(jArray != null)
                    {
                        foreach (JObject item in jArray)
                        {
                            var stockInfo = new StockInfo
                            {
                                No = GetValue(item, "c"),
                                Type = GetStockType(item, "ex"),
                                Name = GetValue(item, "n"),
                                FullName = GetValue(item, "nf"),
                                LastTradedPrice = GetNullableFloat(item, "z"),
                                LastVolume = GetUInt32(item, "tv"),
                                TotalVolume = GetUInt32(item, "v"),
                                Top5SellPrice = GetFloatArray(item, "a"),
                                Top5SellVolume = GetUInt32Array(item, "f"),
                                Top5BuyPrice = GetFloatArray(item, "b"),
                                Top5BuyVolume = GetUInt32Array(item, "g"),
                                SyncTime = GetDateTime(item, "tlong"),
                                HighestPrice = GetNullableFloat(item, "h"),
                                LowestPrice = GetNullableFloat(item, "l"),
                                OpeningPrice = GetNullableFloat(item, "o"),
                                YesterdayClosingPrice = GetFloat(item, "y"),
                                LimitUp = GetFloat(item, "u"),
                                LimitDown = GetFloat(item, "w")
                            };
                            result.Add(stockInfo.No, stockInfo);
                        }
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

        private string GetValue(JObject item, string key)
        {
            return item[key]?.ToString() ?? string.Empty;
        }

        private StockType GetStockType(JObject item, string key)
        {
            return item[key]?.ToString().ToLower() == StockType.OTC.ToString().ToLower() ? StockType.OTC : StockType.TSE;
        }

        private float? GetNullableFloat(JObject item, string key)
        {
            var token = item[key];
            return token == null || token.ToString() == "-" ? (float?)null : Convert.ToSingle(token);
        }

        private uint GetUInt32(JObject item, string key)
        {
            var token = item[key];
            return token?.ToString() == "-" ? 0 : Convert.ToUInt32(token);
        }

        private float[] GetFloatArray(JObject item, string key)
        {
            var token = item[key];
            return token == null || token.ToString() == "-"
                ? []
                : token.ToString().Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(float.Parse).ToArray();
        }

        private uint[] GetUInt32Array(JObject item, string key)
        {
            var token = item[key];
            return token == null || token.ToString() == "-"
                ? []
                : token.ToString().Split('_').Where(x => !string.IsNullOrEmpty(x)).Select(uint.Parse).ToArray();
        }

        private DateTime GetDateTime(JObject item, string key)
        {
            var token = item[key];
            return new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToUInt64(token));
        }

        private float GetFloat(JObject item, string key)
        {
            var token = item[key];
            return Convert.ToSingle(token);
        }
    }
}

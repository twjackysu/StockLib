using System;

namespace StockLib
{
    public class StockInfo
    {
        public string No { get; set; }
        public StockType Type { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        /// <summary>最新成交價/summary>
        public float? LastTradedPrice { get; set; }
        /// <summary>最新一筆交易成交量/summary>
        public uint LastVolume { get; set; }
        /// <summary>今日累積成交量</summary>
        public uint TotalVolume { get; set; }
        /// <summary>最佳五檔賣出價格</summary>
        public float[] Top5SellPrice { get; set; }
        /// <summary>最佳五檔賣出數量</summary>
        public uint[] Top5SellVolume { get; set; }
        /// <summary>最佳五檔買入價格</summary>
        public float[] Top5BuyPrice { get; set; }
        /// <summary>最佳五檔買入數量</summary>
        public uint[] Top5BuyVolume { get; set; }
        /// <summary>最後Sync時間</summary>
        public DateTime SyncTime { get; set; }
        /// <summary>最高價</summary>
        public float? HighestPrice { get; set; }
        /// <summary>最低價</summary>
        public float? LowestPrice { get; set; }
        /// <summary>開盤價</summary>
        public float? OpeningPrice { get; set; }
        /// <summary>昨日收盤價</summary>
        public float YesterdayClosingPrice { get; set; }
        /// <summary>漲停點</summary>
        public float LimitUp { get; set; }
        /// <summary>跌停點</summary>
        public float LimitDown { get; set; }
    }
}

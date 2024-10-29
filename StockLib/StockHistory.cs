namespace StockLib
{
    public class StockHistory
    {
        public StockHistory(List<string> data)
        {
            var date = data[0].Replace(",", "").Replace("＊", "").Split('/').Select(x => Convert.ToInt32(x)).ToList();

            Date = new DateTime(date[0]+1911, date[1], date[2]);
            TradeVolume = Convert.ToUInt32(data[1].Replace(",", ""));
            TurnOverInValue = Convert.ToDecimal(data[2].Replace(",", ""));
            OpeningPrice = Convert.ToDecimal(data[3].Replace(",", ""));
            HighestPrice = Convert.ToDecimal(data[4].Replace(",", ""));
            LowestPrice = Convert.ToDecimal(data[5].Replace(",", ""));
            ClosingPrice = Convert.ToDecimal(data[6].Replace(",", ""));
            DailyPricing = data[7];
            NumberOfDeals = Convert.ToUInt32(data[8].Replace(",", ""));
        }
        /// <summary>日期</summary>
        public DateTime Date { get; set; }
        /// <summary>成交股數</summary>
        public uint TradeVolume { get; set; }
        /// <summary>成交金額</summary>
        public decimal TurnOverInValue { get; set; }
        /// <summary>開盤價</summary>
        public decimal OpeningPrice { get; set; }
        /// <summary>最高價</summary>
        public decimal HighestPrice { get; set; }
        /// <summary>最低價</summary>
        public decimal LowestPrice { get; set; }
        /// <summary>收盤價</summary>
        public decimal ClosingPrice { get; set; }
        /// <summary>漲跌價差</summary>
        public string DailyPricing { get; set; }
        /// <summary>成交筆數</summary>
        public uint NumberOfDeals { get; set; }
    }
}

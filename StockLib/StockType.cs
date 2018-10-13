namespace StockLib
{
    public enum StockType
    {
        /// <summary>上櫃</summary>
        [StockKey("otc_{StockNo}.tw")]
        OTC,
        /// <summary>上市</summary>
        [StockKey("tse_{StockNo}.tw")]
        TSE,
    }
}

namespace StockLib
{
    public class StockQuery
    {
        public StockQuery(StockType type, string stockNo)
        {
            Type = type;
            StockNo = stockNo;
        }
        public readonly StockType Type;
        public readonly string StockNo;
    }
}

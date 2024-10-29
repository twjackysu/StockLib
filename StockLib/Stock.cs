namespace StockLib
{
    public class Stock
    {
        public required string No { get; set; }
        public required string Name { get; set; }
        public required StockType Type { get; set; }
    }
}

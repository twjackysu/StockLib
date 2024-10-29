namespace StockLib
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StockKeyAttribute : Attribute
    {
        public string Key;
        public StockKeyAttribute(string key)
        {
            Key = key;
        }
    }
}

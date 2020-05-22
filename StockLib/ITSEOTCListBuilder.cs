using System.Collections.Generic;

namespace StockLib
{
    public interface ITSEOTCListBuilder
    {
        Dictionary<string, string> GetOTCList();
        Dictionary<string, string> GetTSEList();
    }
}

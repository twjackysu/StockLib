using System.Collections.Generic;

namespace StockLib
{
    public interface ITSEOTCListBuilder
    {
        HashSet<string> GetOTCList();
        HashSet<string> GetTSEList();
    }
}

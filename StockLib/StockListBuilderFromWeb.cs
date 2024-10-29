using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace StockLib
{
    public class StockListBuilderFromWeb : IStockListBuilder
    {
        private readonly ILogger<StockListBuilderFromWeb> logger;
        private static readonly HttpClient httpClient = new HttpClient();

        public StockListBuilderFromWeb(ILogger<StockListBuilderFromWeb> logger)
        {
            this.logger = logger;
            //.Net Core need Nuget: System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public async Task<Dictionary<string, Stock>> GetAllStockListAsync(bool includeWarrant = false)
        {
            try
            {
                var OTCListTask = GetOTCListAsync(includeWarrant);
                var TSEListTask = GetTSEListAsync(includeWarrant);
                await Task.WhenAll(OTCListTask, TSEListTask);
                var OTCList = await OTCListTask;
                var TSEList = await TSEListTask;
                return OTCList.Concat(TSEList).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetAllStockListAsync({includeWarrant})");
                return null;
            }
        }
        public async Task<Dictionary<string, Stock>> GetOTCListAsync(bool includeWarrant = false)
        {
            try
            {
                var url = "https://isin.twse.com.tw/isin/C_public.jsp?strMode=4";
                var html = await GetHtmlAsync(url);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument doc = parser.ParseDocument(html);
                IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");

                return element.Where(x => includeWarrant || (x.TextContent[x.TextContent.Length - 3] != '購' && x.TextContent[x.TextContent.Length - 3] != '售'))
                    .Select(x => x.TextContent.Split('　'))
                    .ToDictionary(x => x[0], x => new Stock() { No = x[0], Name = x[1], Type = StockType.OTC });
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetOTCListAsync({includeWarrant})");
                return null;
            }
        }
        public async Task<Dictionary<string, Stock>> GetTSEListAsync(bool includeWarrant = false)
        {
            try
            {
                var url = "https://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
                var html = await GetHtmlAsync(url);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument doc = parser.ParseDocument(html);
                IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");
            
                return element.Where(x => includeWarrant || (x.TextContent[x.TextContent.Length-3] != '購' && x.TextContent[x.TextContent.Length - 3] != '售'))
                    .Select(x => x.TextContent.Split('　'))
                    .ToDictionary(x => x[0], x => new Stock() { No = x[0], Name = x[1], Type = StockType.TSE });
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetTSEListAsync({includeWarrant})");
                return null;
            }
        }
        private async Task<string> GetHtmlAsync(string url)
        {
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var raw = await response.Content.ReadAsByteArrayAsync();
                var htmlCode = Encoding.GetEncoding(950).GetString(raw);
                return htmlCode;
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Error when GetHtmlAsync({url})");
                return null;
            }
        }
    }
}

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace StockLib
{
    public class TSEOTCListBuilder : ITSEOTCListBuilder
    {
        private readonly ILogger<TSEOTCListBuilder> logger;
        public TSEOTCListBuilder(ILogger<TSEOTCListBuilder> logger)
        {
            this.logger = logger;
            //.Net Core need Nuget: System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public Dictionary<string, string> GetOTCList()
        {
            try
            {
                var url = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";
                var html = GetHtml(url);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument doc = parser.ParseDocument(html);
                IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");

                return element.Where(x => !(x.TextContent[x.TextContent.Length - 3] == '購' || x.TextContent[x.TextContent.Length - 3] == '售'))
                    .Select(x => x.TextContent.Split('　'))
                    .ToDictionary(x => x[0], x => x[1]);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetOTCList()");
                return null;
            }
        }
        public Dictionary<string, string> GetTSEList()
        {
            try
            {
                var url = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
                var html = GetHtml(url);
                HtmlParser parser = new HtmlParser();
                IHtmlDocument doc = parser.ParseDocument(html);
                IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");
            
                return element.Where(x => !(x.TextContent[x.TextContent.Length-3] == '購' || x.TextContent[x.TextContent.Length - 3] == '售'))
                    .Select(x => x.TextContent.Split('　'))
                    .ToDictionary(x => x[0], x => x[1]);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error when GetTSEList()");
                return null;
            }
        }
        private string GetHtml(string url)
        {
            try
            {
                var myRequest = WebRequest.Create(url);
                myRequest.Method = "GET";
                var myResponse = myRequest.GetResponse();
                var sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(950));//950 = Big5
                var htmlSourceCode = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();
                return htmlSourceCode;
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Error when GetHtml({url})");
                return null;
            }
        }
    }
}

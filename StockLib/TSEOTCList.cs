using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace StockLib
{
    public class TSEOTCList
    {
        public TSEOTCList()
        {
            //.Net Core need Nuget: System.Text.Encoding.CodePages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public string[] GetOTCList()
        {
            var url = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";
            var html = GetHtml(url);
            HtmlParser parser = new HtmlParser();
            IHtmlDocument doc = parser.ParseDocument(html);
            IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");

            return element.Where(x => !(x.TextContent[x.TextContent.Length - 3] == '購' || x.TextContent[x.TextContent.Length - 3] == '售')).Select(x => {
                var splitStr = x.TextContent.Split('　'); return splitStr[0];
            }).ToArray();
        }
        public string[] GetTSEList()
        {
            var url = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
            var html = GetHtml(url);
            HtmlParser parser = new HtmlParser();
            IHtmlDocument doc = parser.ParseDocument(html);
            IEnumerable<IElement> element = doc.QuerySelectorAll("body > table.h4 > tbody > tr > td:nth-child(1)[bgcolor='#FAFAD2']:not([colspan='7'])");
            
            return element.Where(x => !(x.TextContent[x.TextContent.Length-3] == '購' || x.TextContent[x.TextContent.Length - 3] == '售')).Select(x => {
                var splitStr = x.TextContent.Split('　'); return splitStr[0];
            }).ToArray();
        }
        private string GetHtml(string url)
        {
            WebRequest myRequest = WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(950));//950 = Big5
            string htmlSourceCode = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return htmlSourceCode;
        }
    }
}

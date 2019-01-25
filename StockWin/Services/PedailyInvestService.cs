using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StockWin.Model;

namespace StockWin.Services
{
    /// <summary>
    /// 投资界融资事件抓取类
    /// https://zdb.pedaily.cn/inv/
    /// </summary>
    public class PedailyInvestService : BaseService
    {
        private static string domain = "https://zdb.pedaily.cn";
        public override string SiteName { get; } = "投资界";
        public override string PageUrl { get; } = domain + "/inv/p{0}/";// 共搜索到15957条结果

        public override string ItemFilePath{ get; } = 
            Path.Combine(Environment.CurrentDirectory, "InvestedPedaily/")+ "{0}.json";
        public override string PageFilePath { get; } =
            Path.Combine(Environment.CurrentDirectory, "InvestedPedailyPage/") + "{0}.html";
        protected override string CatchedFile { get; } =
            Path.Combine(Environment.CurrentDirectory, "InvestedPedailyInc/") + "{0}.txt";
        protected override Type ModelType { get; } = typeof(InvestEvt);

        public override string PageStartStr { get; } = "共搜索到";

        protected override Regex RegPage { get; } = 
            new Regex(@"<a\s[^>]+href=""/inv/show(\d+)/"">详情</a>", RegexOptions.Compiled);

        static Regex _regexMain = new Regex(@"<div class=""box-fix-l""><div class=""info"">(.*?)</div></div>", RegexOptions.Compiled);
        static Regex _regexTitle = new Regex(@"<h1[^>]*>(.*?)</h1>", RegexOptions.Compiled);
        static Regex _regexData = new Regex(@"<li[^>]*><span[^>]*>.*?</span>(.*?)(?=</?li>)", RegexOptions.Compiled);
        

        protected override async Task<BaseEvt> ParseHtml(string sn, string html)
        {            
            var evt = new InvestEvt();
            evt.Sn = sn;
            evt.Url = $"{domain}/inv/show{sn}/";

            if (await ParseHtml(evt))
            {
                return evt;
            }
            return null;
        }



        /// <summary>
        /// 拉取单个上市详细数据
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        async Task<bool> ParseHtml(InvestEvt evt)
        {
            if (string.IsNullOrEmpty(evt.Url))
            {
                throw new ArgumentException("未赋值Url，无法执行");
            }
            string html;
            try
            {
                html = await Util.GetPage(evt.Url);
            }
            catch (Exception exp)
            {
                Util.Error($"error: {evt.Url}: {exp}");
                return false;
            }
          
            return RealParse(evt, html);
        }

        internal bool RealParse(InvestEvt evt, string html)
        {
            var idx = html.IndexOf("投资事件详情", StringComparison.Ordinal);
            if (idx < 0)
            {
                Util.Error($"no data: {evt.Url}: {html}");
                return false;
            }

            var match = _regexMain.Match(html, idx);
            if (!match.Success)
            {
                Util.Error($"no match data: {evt.Url}: {html}");
                return false;
            }
            var dataStr = match.Value;
            var matTitle = _regexTitle.Match(dataStr);
            if (!matTitle.Success)
            {
                Util.Error($"no match title: {evt.Url}: {dataStr}");
                return false;
            }
            evt.Title = TrimVal(matTitle, 1);

            var matData = _regexData.Match(dataStr);
            if (!matData.Success)
            {
                Util.Error($"no match dataStr: {evt.Url}: {dataStr}");
                return false;
            }
            evt.RecieveEnt = TrimVal(matData, 1);

            matData = matData.NextMatch();
            evt.PayEnt = TrimVal(matData, 1);

            matData = matData.NextMatch();
            evt.Money = TrimVal(matData, 1);

            matData = matData.NextMatch();
            evt.Rounds = TrimVal(matData, 1);

            matData = matData.NextMatch();
            evt.Date = TrimVal(matData, 1).Replace("年", "-").Replace("月", "-").Replace("日", ""); 

            matData = matData.NextMatch();
            evt.Industry = TrimVal(matData, 1);
            return true;
        }
    }
}
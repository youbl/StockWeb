using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using StockWeb.Model;

namespace StockWeb.Services
{
    /// <summary>
    /// 投资界融资事件抓取类
    /// https://zdb.pedaily.cn/inv/
    /// </summary>
    public class PedailyInvestService : BaseService
    {
        private static string domain = "https://zdb.pedaily.cn";
        public override string PageUrl { get; } = domain + "/inv/p{0}/";// 共搜索到15957条结果

        public override string ItemFilePath{ get; } = 
            Path.Combine(Environment.CurrentDirectory, $"InvestedPedaily/")+ "{0}.json";
        public override string PageStartStr { get; } = "共搜索到";

        static string allItemFilename = Path.Combine(Environment.CurrentDirectory, "InvestedPedaily/all.json");
        protected override Regex RegPage { get; } = 
            new Regex(@"<a\s[^>]+href=""/inv/show(\d+)/"">详情</a>", RegexOptions.Compiled);
        protected static Regex _regexMain = new Regex(@"<div class=""box-fix-l""><div class=""info"">(.*?)</div></div>", RegexOptions.Compiled);
        protected static Regex _regexTitle = new Regex(@"<h1[^>]*>(.*?)</h1>", RegexOptions.Compiled);
        protected static Regex _regexData = new Regex(@"<li[^>]*><span[^>]*>.*?</span>((?!</?li>).*?)(?=</?li>)", RegexOptions.Compiled);


        public override async Task<int> SaveAllItems()
        {
            if (File.Exists(allItemFilename))
            {
                var items = GetItemsFromFile(allItemFilename);
                foreach (var item in items)
                {
                    ParseAndSave(item, null);
                }
                return items.Count;
            }
            return await base.SaveAllItems();
        }


        protected static HashSet<string> GetItemsFromFile(string filename)
        {
            var ret = new HashSet<string>();
            using (var sr = new StreamReader(filename, Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine() ?? "";
                    if (line.Length == 0 || line.StartsWith("http"))
                    {
                        continue;
                    }
                    if (!ret.Add(line))
                        Console.WriteLine(line);
                }
            }
            return ret;
        }

        protected override async void ParseAndSave(string sn, string html)
        {
            #region 保存详细SN列表
            var dir = Path.GetDirectoryName(allItemFilename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var sw = new StreamWriter(allItemFilename, true, Encoding.UTF8))
                sw.WriteLine(sn);
            #endregion
            
            var evt = new InvestEvt();
            evt.Sn = sn;
            evt.Url = $"{domain}/inv/show{sn}/";

            var file = string.Format(ItemFilePath, evt.Sn);
            if (!File.Exists(file) && await ParseHtml(evt))
            {
                SaveToFile(file, evt);
                SaveNum++;
            }
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
                await Util.Log($"error: {evt.Url}: {exp}");
                return false;
            }
            var idx = html.IndexOf("投资事件详情", StringComparison.Ordinal);
            if (idx < 0)
            {
                await Util.Log($"no data: {evt.Url}: {html}");
                return false;
            }

            var match = _regexMain.Match(html, idx);
            if (!match.Success)
            {
                await Util.Log($"no match data: {evt.Url}: {html}");
                return false;
            }
            var dataStr = match.Value;
            var matTitle = _regexTitle.Match(dataStr);
            if (!matTitle.Success)
            {
                await Util.Log($"no match title: {evt.Url}: {dataStr}");
                return false;
            }
            evt.Title = matTitle.Groups[1].Value.Trim();

            var matData = _regexData.Match(dataStr);
            if (!matData.Success)
            {
                await Util.Log($"no match dataStr: {evt.Url}: {dataStr}");
                return false;
            }
            evt.RecieveEnt = TrimVal(matData);

            matData = matData.NextMatch();
            evt.PayEnt = TrimVal(matData);

            matData = matData.NextMatch();
            evt.Money = TrimVal(matData);

            matData = matData.NextMatch();
            evt.Rounds = TrimVal(matData);

            matData = matData.NextMatch();
            evt.Date = TrimVal(matData);

            matData = matData.NextMatch();
            evt.Industry = TrimVal(matData);
            return true;
        }


    }
}
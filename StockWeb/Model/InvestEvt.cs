using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using StockWeb.Services;

namespace StockWeb.Model
{
    /// <summary>
    /// 投资事件
    /// </summary>
    public class InvestEvt : BaseEvt
    {
        /// <summary>
        /// 分页地址
        /// </summary>
        public static string PageUrl { get; } = "https://zdb.pedaily.cn/inv/p{0}/";// 共搜索到15957条结果

        #region 属性列表

   
        /// <summary>
        /// 融  资  方
        /// </summary>
        [Description("融资方")]
        public string RecieveEnt { get; set; }
        /// <summary>
        /// 投  资  方
        /// </summary>
        [Description("投资方")]
        public string PayEnt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Description("金额")]
        public string Money { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Description("融资轮次")]
        public string Rounds { get; set; }
        #endregion


        static string allItemFilename = Path.Combine(Environment.CurrentDirectory, $"Invested/all.json");
        static Regex _regPage = new Regex(@"<a\s[^>]+href=""(/inv/show\d+/)"">详情</a>", RegexOptions.Compiled);

        public static async Task<int> SaveAll()
        {
            var dir = Path.GetDirectoryName(allItemFilename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            HashSet<string> items;
            // items = new List<string>() { "/inv/show20027/" };
            if (File.Exists(allItemFilename))
            {
                items = GetItemsFromFile(allItemFilename);
            }
            else
            {
                items = await GetItems(); // 从网络抓取分页数据列表
            }

            var num = 0;
            var ok = 0;
            Parallel.ForEach(items, async url =>
                // foreach (var url in items)
            {
                var item = new InvestEvt();
                item.Sn = RegNum.Match(url).Value;
                var filename = Path.Combine(Environment.CurrentDirectory, $"Invested/{item.Sn}.json");
                if (!File.Exists(filename) && await item.ParseHtml(url))
                {
                    Util.SeriaToFile(filename, item);
                    Interlocked.Increment(ref ok);
                }
                Interlocked.Increment(ref num);
                if (num % 100 == 0)
                {
                    Console.WriteLine($"Total: {num.ToString()}  OK: {ok.ToString()}");
                }
            });
            Console.WriteLine($"Total: {num.ToString()}  OK: {ok.ToString()}");
            return items.Count;
        }

        /// <summary>
        /// 遍历所有分页，拉取全部数据
        /// </summary>
        /// <returns></returns>
        static async Task<HashSet<string>> GetItems()
        {
            var ret = new HashSet<string>();
            var page = 1;
            bool findData;
            using (var sw = new StreamWriter(allItemFilename, false, Encoding.UTF8))
            {
                do
                {
                    findData = false;
                    var url = string.Format(PageUrl, page.ToString());
                    string pageHtml;
                    try
                    {
                        pageHtml = await Util.GetPage(url);
                    }
                    catch (Exception exp)
                    {
                        LogHelper.LogWarning($"error: {url}: {exp}");
                        break;
                    }
                    var startIdx = pageHtml.IndexOf("共搜索到", StringComparison.Ordinal);
                    if (startIdx < 0)
                    {
                        LogHelper.LogWarning($"no page data: {url}: {pageHtml}");
                        break;
                    }
                    var match = _regPage.Match(pageHtml);
                    while (match.Success)
                    {
                        findData = true;
                        var item = match.Groups[1].Value;
                        if (!ret.Add(item))
                        {
                            Console.WriteLine(item);
                        }

                        sw.WriteLine(item);
                        match = match.NextMatch();
                    }
                    sw.WriteLine(url);
                    page++;
                } while (findData);
            }
            return ret;
        }

   
        /// <summary>
        /// 拉取单个上市详细数据
        /// </summary>
        /// <param name="urlttt"></param>
        /// <returns></returns>
        public async Task<bool> ParseHtml(string urlttt = "")
        {
            if (!string.IsNullOrEmpty(urlttt))
            {
                Url = urlttt;
            }
            if (string.IsNullOrEmpty(Url))
            {
                throw new ArgumentException("未赋值Url，无法执行");
            }
            if (!Url.StartsWith("http"))
                Url = "https://zdb.pedaily.cn" + Url;
            string html;
            try
            {
                html = await Util.GetPage(Url);
            }
            catch (Exception exp)
            {
                LogHelper.LogWarning($"error: {Url}: {exp}");
                return false;
            }
            var idx = html.IndexOf("投资事件详情", StringComparison.Ordinal);
            if (idx < 0)
            {
                LogHelper.LogWarning($"no data: {Url}: {html}");
                return false;
            }

            var match = _regexMain.Match(html, idx);
            if (!match.Success)
            {
                LogHelper.LogWarning($"no match data: {Url}: {html}");
                return false;
            }
            var dataStr = match.Value;
            var matTitle = _regexTitle.Match(dataStr);
            if (!matTitle.Success)
            {
                LogHelper.LogWarning($"no match title: {Url}: {dataStr}");
                return false;
            }
            Title = matTitle.Groups[1].Value.Trim();

            var matData = _regexData.Match(dataStr);
            if (!matData.Success)
            {
                LogHelper.LogWarning($"no match dataStr: {Url}: {dataStr}");
                return false;
            }
            RecieveEnt = TrimVal(matData);

            matData = matData.NextMatch();
            PayEnt = TrimVal(matData);

            matData = matData.NextMatch();
            Money = TrimVal(matData);

            matData = matData.NextMatch();
            Rounds = TrimVal(matData);

            matData = matData.NextMatch();
            Date = TrimVal(matData);

            matData = matData.NextMatch();
            Industry = TrimVal(matData);
            return true;
        }


        public static void ReadAndToExcel()
        {
            ReadAndToExcel<InvestEvt>(allItemFilename);
        }
    }
}

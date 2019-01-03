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
    /// 上市事件
    /// </summary>
    public class ListedEvt : BaseEvt
    {
        /// <summary>
        /// 分页地址
        /// </summary>
        public static string PageUrl { get; } = "https://zdb.pedaily.cn/ipo/p{0}/";

        #region 属性列表

        /// <summary>
        /// 公司名称
        /// </summary>
        [Description("公司名称")]
        public string Name { get; set; }

        /// <summary>
        /// 投资方
        /// </summary>
        [Description("投资方")]
        public string InvestName { get; set; }

        /// <summary>
        /// 发行价
        /// </summary>
        [Description("发行价")]
        public string Price { get; set; }
        /// <summary>
        /// 上市地点
        /// </summary>
        [Description("上市地点")]
        public string ListedPlace { get; set; }
        /// <summary>
        /// 发行量
        /// </summary>
        [Description("发行量")]
        public string ListedNum { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        [Description("股票代码")]
        public string Code { get; set; }
        /// <summary>
        /// 是否VC/PE支持
        /// </summary>
        [Description("是否VC/PE支持")]
        public string VcSuport { get; set; }


        #endregion

        static string saveDir = Path.Combine(Environment.CurrentDirectory, "Listed");
        static string allItemFilename = Path.Combine(saveDir, "all.json");

        public static async Task<int> SaveAll()
        {
            var dir = Path.GetDirectoryName(allItemFilename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            HashSet<string> items;
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
            foreach (var url in items)
            {
                var item = new ListedEvt();
                item.Sn = RegNum.Match(url).Value;
                var filename = Path.Combine(dir, $"{item.Sn}.json");
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

            }
            Console.WriteLine($"Total: {num.ToString()}  OK: {ok.ToString()}");
            return items.Count;
        }

        static Regex _regPage = new Regex(@"<a\s[^>]+href=""(/ipo/show\d+/)"">详情</a>", RegexOptions.Compiled);
        /// <summary>
        /// 遍历所有分页，拉取全部上市数据
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
                        await Util.Log($"error: {url}: {exp}");
                        break;
                    }
                    var startIdx = pageHtml.IndexOf("共搜索到", StringComparison.Ordinal);
                    if (startIdx < 0)
                    {
                        await Util.Log($"no page data: {url}: {pageHtml}");
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
                await Util.Log($"error: {Url}: {exp}");
                return false;
            }
            var idx = html.IndexOf("上市事件详情", StringComparison.Ordinal);
            if (idx < 0)
            {
                await Util.Log($"no data: {Url}: {html}");
                return false;
            }

            var match = _regexMain.Match(html, idx);
            if(!match.Success)
            {
                await Util.Log($"no match data: {Url}: {html}");
                return false;
            }
            var dataStr = match.Value;
            var matTitle = _regexTitle.Match(dataStr);
            if (!matTitle.Success)
            {
                await Util.Log($"no match title: {Url}: {dataStr}");
                return false;
            }
            Title = matTitle.Groups[1].Value.Trim();

            var matData = _regexData.Match(dataStr);
            if (!matData.Success)
            {
                await Util.Log($"no match dataStr: {Url}: {dataStr}");
                return false;
            }
            Name = TrimVal(matData);

            matData = matData.NextMatch();
            Industry = TrimVal(matData);

            matData = matData.NextMatch();
            InvestName = TrimVal(matData);

            matData = matData.NextMatch();
            Date = TrimVal(matData);

            matData = matData.NextMatch();
            Price = TrimVal(matData);

            matData = matData.NextMatch();
            ListedPlace = TrimVal(matData);

            matData = matData.NextMatch();
            ListedNum = TrimVal(matData);

            matData = matData.NextMatch();
            Code = TrimVal(matData);

            matData = matData.NextMatch();
            VcSuport = TrimVal(matData);

            return true;
        }

        public static void ReadAndToExcel()
        {
            ReadAndToExcel<ListedEvt>(allItemFilename);
        }


        public static void Rename()
        {
            var dir = @"D:\mine\StockWeb\StockWeb\bin\Release\netcoreapp2.2\Listed";
            var dir2 = @"D:\mine\StockWeb\StockWeb\bin\Release\netcoreapp2.2\ListedNew";
            Parallel.ForEach(Directory.GetFiles(dir), file =>
            {
                if (file.IndexOf("all.json", StringComparison.OrdinalIgnoreCase) >= 0)
                    return;
                var evt = Util.SeriaFromFile<ListedEvt>(file);
                evt.Sn = RegNum.Match(evt.Url).Value;
                var filename = Path.Combine(dir2, $"{evt.Sn}.json");
                Util.SeriaToFile(filename, evt);
            });
            Console.WriteLine("OK");
        }
    }
}

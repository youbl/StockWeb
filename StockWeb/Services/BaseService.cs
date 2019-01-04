using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StockWeb.Services
{
    public abstract class BaseService
    {
        /// <summary>
        /// 分页URL路径
        /// </summary>
        public abstract string PageUrl { get; }
        /// <summary>
        /// 每项的保存路径
        /// </summary>
        public abstract string ItemFilePath { get; }
        /// <summary>
        /// 每分页项的保存路径
        /// </summary>
        public abstract string PageFilePath { get; }
        /// <summary>
        /// 抓取到的分页数据起始字符串
        /// </summary>
        public abstract string PageStartStr { get; }
        protected abstract Regex RegPage { get; }



        protected static Regex _regexVal = new Regex(@"<[^>]+>", RegexOptions.Compiled);
        protected Regex _regexTd = new Regex(@"<td[^>]*>([\s\S]+?)</td>", RegexOptions.Compiled);


        /// <summary>
        /// 遍历所有分页，拉取全部分页数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> SaveAllItems()
        {
            var readNum = 0;
            var saveNum = 0;
            var ret = 0;
            var page = 1;
            bool findData;
            bool isNet;
            do
            {
                findData = false;
                var url = string.Format(PageUrl, page.ToString());
                string pageHtml;
                var pageFile = string.Format(PageFilePath, page.ToString());
                try
                {
                    if (File.Exists(pageFile))
                    {
                        isNet = false;
                        pageHtml = Util.ReadFile(pageFile);
                    }
                    else
                    {
                        isNet = true;
                        pageHtml = await Util.GetPage(url);
                        Util.SaveFile(pageFile, pageHtml);
                    }
                }
                catch (Exception exp)
                {
                    await Util.Log($"error: {url}: {exp}");
                    findData = true;
                    continue;
                }
                var startIdx = pageHtml.IndexOf(PageStartStr, StringComparison.Ordinal);
                if (startIdx < 0)
                {
                    await Util.Log($"no page data: {url}: {pageHtml}");
                    break;
                }
                var match = RegPage.Match(pageHtml, startIdx);
                while (match.Success)
                {
                    findData = true;
                    ret++;

                    var sn = match.Groups[1].Value;
                    var html = (match.Groups.Count > 2) ? match.Groups[2].Value : "";
                    Interlocked.Increment(ref readNum);

                    if (readNum % 100 == 0)
                    {
                        Console.WriteLine($"Total: {readNum.ToString()}  OK: {saveNum.ToString()}");
                    }
                    if (await ParseAndSave(sn, html))
                        Interlocked.Increment(ref saveNum);

                    match = match.NextMatch();
                }
                page++;
                if (findData && isNet)
                {
                    await Task.Delay(5000);
                }
            } while (findData);

            return ret;
        }

        /// <summary>
        /// 解析每项数据，并写入文件
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="html"></param>
        protected abstract Task<bool> ParseAndSave(string sn, string html);

        protected virtual bool SaveToFile(string file, object evt)
        {
            if (!File.Exists(file))
            {
                Util.SeriaToFile(file, evt);
                return true;
            }
            return false;
        }

        protected static string TrimVal(Match matData, int idx = 0)
        {
            var ret = matData.Groups[idx].Value;
            return _regexVal.Replace(ret, "").Replace("&gt;", ">").Trim();
        }

    }
}
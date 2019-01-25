using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using StockWin.Model;

namespace StockWin.Services
{
    public abstract class BaseService
    {
        public abstract string SiteName { get; }
        protected abstract Type ModelType { get; }
        /// <summary>
        /// 分页URL路径
        /// </summary>
        public abstract string PageUrl { get; }

        /// <summary>
        /// 每项数据的保存路径，必须包含 {0} , 用sn替换
        /// </summary>
        public abstract string ItemFilePath { get; }
        /// <summary>
        /// 每次抓取的所有数据sn清单，比如增量数据方便区分，必须包含 {0} , 用抓取启动时间替换
        /// </summary>
        protected abstract string CatchedFile { get; }


        /// <summary>
        /// 每分页项的保存路径
        /// </summary>
        public abstract string PageFilePath { get; }

        /// <summary>
        /// 抓取到的分页数据起始字符串
        /// </summary>
        public abstract string PageStartStr { get; }

        /// <summary>
        /// 获取分页中，每条数据详情的正则
        /// </summary>
        protected abstract Regex RegPage { get; }
        
        protected static Regex RegexVal = new Regex(@"<[^>]+>", RegexOptions.Compiled);

        /// <summary>
        /// 一轮抓取是否完成
        /// </summary>
        public virtual bool Completed { get; private set; } = true;
        /// <summary>
        /// 是否中断抓取
        /// </summary>
        public virtual bool Cancel { get; set; } = false;

        private DateTime lastTime;

        /// <summary>
        /// 前次抓取时间
        /// </summary>
        public virtual DateTime LastCatchTime
        {
            get
            {
                if (lastTime == default(DateTime))
                {
                    var pageDir = Path.GetDirectoryName(PageFilePath);
                    if (Directory.Exists(pageDir))
                    {
                        var file = Directory.GetFiles(pageDir).FirstOrDefault();
                        if (file == null)
                        {
                            lastTime = new DateTime(2019, 1, 13);
                        }
                        else
                        {
                            lastTime = new FileInfo(file).CreationTime;
                        }
                    }

                }
                return lastTime;
            }
            private set => lastTime = value;
        }

        private string TypeName { get; }

        protected BaseService()
        {
            TypeName = GetType().FullName;
        }

        /// <summary>
        /// 遍历所有分页，拉取全部分页数据
        /// </summary>
        /// <param name="incr">false表示抓取全量，true表示抓取增量</param>
        /// <returns></returns>
        public virtual async Task<int> SaveAllItems(bool incr)
        {
            Completed = false;

            var readNum = 0;
            var saveNum = 0;
            var ret = 0;
            var page = 1;
            bool findData;
            try
            {
                if (!CatchedFile.Contains("{0}"))
                {
                    Util.Error(TypeName + " CatchedFile 不含{0}");
                    return 0;
                }

                var strNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                var cacheFile = string.Format(CatchedFile, strNow);
                do
                {
                    if (Cancel)
                    {
                        Util.Log($"已中断：{TypeName}");
                        break;
                    }
                    findData = false;
                    var url = string.Format(PageUrl, page.ToString());
                    string pageHtml;
                    var pageFile = string.Format(PageFilePath, page.ToString());
                    bool isNet;
                    try
                    {
                        // 增量抓取时，不考虑本地缓存，直接抓网页
                        if (incr)
                        {
                            pageFile += strNow;
                        }
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
                        Util.Error($"error: {url}: {exp}");
                        findData = true;
                        continue;
                    }
                    Util.Log($"第:{page.ToString()}页: {TypeName}|Total: {readNum.ToString()}  OK: {saveNum.ToString()}");

                    var startIdx = pageHtml.IndexOf(PageStartStr, StringComparison.Ordinal);
                    if (startIdx < 0)
                    {
                        Util.Error($"no page data: {url}: {pageHtml}");
                        break;
                    }
                    var match = RegPage.Match(pageHtml, startIdx);
                    while (match.Success)
                    {
                        if (Cancel)
                        {
                            Util.Log($"已中断：{TypeName}");
                            break;
                        }
                        findData = true;
                        ret++;

                        var sn = match.Groups[1].Value;
                        var html = (match.Groups.Count > 2) ? match.Groups[2].Value : "";
                        Interlocked.Increment(ref readNum);

                        // 保存详细SN列表
                        Util.AppendFile(cacheFile, sn);
                        var parseRet = await ParseAndSave(sn, html, page);
                        if (parseRet == 1)
                        {
                            Interlocked.Increment(ref saveNum);
                        }
                        else if (incr && parseRet == 0)
                        {
                            findData = false;
                            Util.Log($"数据存在，增量抓取退出 {TypeName}");
                            break; // 抓取增量数据时，文件存在则退出
                        }
                        match = match.NextMatch();
                    }
                    page++;
                    if (findData && isNet)
                    {
                        await Task.Delay(5000);
                    }
                } while (findData);
            }
            finally
            {
                Completed = true;
            }
            LastCatchTime = DateTime.Now;
            Util.Log($"本次完成，{TypeName}|Total: {readNum.ToString()}  OK: {saveNum.ToString()}");
            return ret;
        }

        /// <summary>
        /// 保存单项数据.
        /// 0为已存在不操作；
        /// 1为保存成功；
        /// -1为出错
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="html"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected virtual async Task<int> ParseAndSave(string sn, string html, int page)
        {
            if (!ItemFilePath.Contains("{0}"))
            {
                Util.Error(TypeName + " ItemFilePath 不含{0}");
                return -1;
            }
            var file = string.Format(ItemFilePath, sn);
            if (File.Exists(file))
                return 0;

            var obj = await ParseHtml(sn, html);
            if (obj != null)
            {
                obj.Page = page.ToString();
                Util.SeriaToFile(file, obj);
                return 1;
            }
            Util.Error(TypeName + " 解析出错");
            return -1;
        }

        /// <summary>
        /// 解析每项数据，并写入文件
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="html"></param>
        protected abstract Task<BaseEvt> ParseHtml(string sn, string html);

        ///// <summary>
        ///// 使用当前子类作为泛型方法入参
        ///// </summary>
        ///// <param name="xlsfile"></param>
        ///// <returns></returns>
        //public async Task<bool> ReadAndToExcel(string xlsfile)
        //{
        //    try
        //    {
        //        MethodInfo method = GetType().GetMethod("ReadAndToExcelInter", BindingFlags.Instance | BindingFlags.NonPublic)
        //            .MakeGenericMethod(new Type[] {this.GetType()});
        //        var ret = (Task<bool>) method.Invoke(this, new object[] {xlsfile});
        //        return await ret;
        //    }
        //    catch (Exception exp)
        //    {
        //        Console.WriteLine(exp.ToString());
        //        return false;
        //    }
        //}

        public virtual List<BaseEvt> ReadEvts(string[] keywords)
        {
            if (!ModelType.IsSubclassOf(typeof(BaseEvt)))
            {
                Util.Error($"{ModelType.FullName} 必须是BaseEvt的子类");
                return null;
            }

            var dir = Path.GetDirectoryName(ItemFilePath);
            if (!Directory.Exists(dir))
            {
                Util.Error($"{dir} 目录不存在");
                return null;
            }

            var arr = new List<BaseEvt>();
            var arrFiles = Directory.GetFiles(dir);
            var needFilter = keywords != null && keywords.Length > 0;
            Parallel.ForEach(arrFiles, file =>
            {
                if (file.IndexOf("all.json", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    !file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    return; // continue;
                try
                {
                    if (!(Util.DeSeriaFromFile(ModelType, file) is BaseEvt evt))
                    {
                        Util.Error($"{file} 反序列化为空");
                        return;
                    }
                    if (needFilter && !ContainsKey(keywords, evt))
                    {
                        return;
                    }
                    lock (arr)
                    {
                        arr.Add(evt);
                    }
                }
                catch (Exception exp)
                {
                    Util.Error($"{file} {exp}");
                }
            });
            return arr;
        }

        /// <summary>
        /// 读取指定目录下的所有文件，转成Excel
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="xlsfile"></param>
        /// <param name="showTitle"></param>
        public virtual string ReadAndToExcel(string[] keywords, string xlsfile = null, bool showTitle = true)
        {
            var dir = Path.GetDirectoryName(ItemFilePath) ?? "";
            if (string.IsNullOrEmpty(xlsfile))
            {
                xlsfile = Path.Combine(dir, "all.xlsx");
            }

            Util.Log($"{GetType().FullName} 开始导出");
            var arr = ReadEvts(keywords);
            Util.Log($"{GetType().FullName} 读取到{arr.Count.ToString()}条数据 {xlsfile}");
           
            try
            {
                arr.Sort((x, y) => String.Compare(x.Title, y.Title, StringComparison.Ordinal));
                ExcelHelper.ToExcel(arr, xlsfile, showTitle);
                return xlsfile;
            }
            catch (Exception exp)
            {
                Util.Error($"{dir} {xlsfile} {exp}");
                return null;
            }
        }

        static bool ContainsKey(string[] keywords, BaseEvt evt)
        {
            string key;
            if (evt is InvestEvt ievt)
            {
                key = FindContainsKey(keywords, ievt.RecieveEnt);
            }
            else if (evt is ListedEvt levt)
            {
                key = FindContainsKey(keywords, levt.Name);
            }
            else
            {
                return false;
            }
            if (!string.IsNullOrEmpty(key))
            {
                evt.Keyword = key;
                return true;
            }
            return false;
        }

        static string FindContainsKey(string[] keywords, string input)
        {
            foreach (var keyword in keywords)
            {
                if (input.IndexOf(keyword, StringComparison.Ordinal) >= 0)
                {
                    return keyword;
                }
            }
            return null;
        }

        protected static string TrimVal(Match matData, int idx = 0)
        {
            var ret = matData.Groups[idx].Value;
            return RegexVal.Replace(ret, "").Replace("&gt;", ">").Trim();
        }


        static Regex RegEntQcc = new Regex(@"<a onclick=""addSearchIndex\('([^']+)',\s*\d+\)[^>]+href=""/(firm_[^>""]+)""[^>]*>[\s\S]+?地址：(.+?)\s*</p>", RegexOptions.Compiled);
        /// <summary>
        /// 根据公司名称，去企查查找地址.
        /// 返回 new string[] { entAddr, entUrl }
        /// </summary>
        /// <param name="entName"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static async Task<string[]> GetAddressFromQcc(string entName, string sn)
        {
            var cookie = new CookieContainer();
            var entSearchUrl = $"https://www.qichacha.com/search?key={HttpUtility.UrlEncode(entName)}";
            try
            {
                await Util.GetPage("https://www.qichacha.com/", cookie);
            }
            catch (Exception exp)
            {
                var msg = $"{sn}-{entName} 企查查初始化失败\r\n{exp}";
                Util.Error(msg);
                return null;
            }
            Thread.Sleep(600);
            var html = await Util.GetPage(entSearchUrl, cookie);
            if (html.IndexOf("小查还没找到数据", StringComparison.Ordinal) > 0)
            {
                var msg = $"{sn}-{entName} 企查查搜索失败";
                Util.Error(msg);
                return new string[] { "", "" };
            }
            var idxStart = html.IndexOf("家符合条件的企业", StringComparison.Ordinal);
            if (idxStart < 0)
            {
                var msg = $"{sn}-{entName} 企查查地址未找到\r\n{html}";
                Util.Error(msg);
                return null;
            }
            var match = RegEntQcc.Match(html, idxStart);
            if (!match.Success)
            {
                var msg = $"{sn}-{entName} 企查查没有结果匹配\r\n{html}";
                Util.Error(msg);
                return null;
            }
            if (match.Groups[1].Value != entName)
            {
                var msg = $"{sn}-{entName} 企查查匹配企业名不一致\r\n{html}";
                Util.Error(msg);
                return null;
            }
            var entUrl = match.Groups[2].Value;
            var entAddr = TrimVal(match, 3);
            return new string[] { entAddr, entUrl };
        }



        static Regex RegEntTyc = new Regex(@"<span class=""tt hidden"">" +
            @"{""id"":(\d+),""name"":""([^""]*)"".*?" +
            @"""regLocation"":""([^""]*)""", RegexOptions.Compiled);
        /// <summary>
        /// 根据公司名称，去天眼查找地址.
        /// 返回 new string[] { entAddr, entUrl }
        /// </summary>
        /// <param name="entName"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static async Task<string[]> GetAddressFromTyc(string entName, string sn)
        {
            var cookie = new CookieContainer();
            var entSearchUrl = $"https://www.tianyancha.com/search?key={HttpUtility.UrlEncode(entName)}";
            try
            {
                await Util.GetPage("https://www.tianyancha.com/", cookie);
            }
            catch (Exception exp)
            {
                var msg = $"{sn}-{entName} 天眼查初始化失败\r\n{exp}";
                Util.Error(msg);
                return null;
            }
            Thread.Sleep(600);
            var html = await Util.GetPage(entSearchUrl, cookie);
            if (html.IndexOf("没有找到相关结果", StringComparison.Ordinal) > 0)
            {
                var msg = $"{sn}-{entName} 天眼查搜索失败";
                Util.Error(msg);
                return null;
            }
            // <span>天眼查为你找到</span><span class="tips-num">4</span>家公司<script
            var idxStart = html.IndexOf(">家公司<", StringComparison.Ordinal);
            if (idxStart < 0)
            {
                var msg = $"{sn}-{entName} 天眼查地址未找到\r\n{html}";
                Util.Error(msg);
                return null;
            }
            var match = RegEntTyc.Match(html, idxStart);
            if (!match.Success)
            {
                var msg = $"{sn}-{entName} 天眼查没有结果匹配\r\n{html}";
                Util.Error(msg);
                return null;
            }
            var matName = TrimVal(match, 2);
            if (matName != entName)
            {
                var msg = $"{sn}-{entName} 天眼查匹配企业名不一致\r\n{html}";
                Util.Error(msg);
                return null;
            }
            var entUrl = "https://www.tianyancha.com/company/" + TrimVal(match, 1);
            var entAddr = TrimVal(match, 3);
            return new string[] { entAddr, entUrl };
        }

    }
}
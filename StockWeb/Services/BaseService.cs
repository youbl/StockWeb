using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using StockWeb.Model;

namespace StockWeb.Services
{
    public abstract class BaseService
    {
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
            var readNum = 0;
            var saveNum = 0;
            var ret = 0;
            var page = 1;
            bool findData;
            if (!CatchedFile.Contains("{0}"))
            {
                await Util.Error(TypeName + " CatchedFile 不含{0}");
                return 0;
            }

            var strNow = DateTime.Now.ToString("yyyyMMddHHmmss");
            var cacheFile = string.Format(CatchedFile, strNow);
            do
            {
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
                    await Util.Error($"error: {url}: {exp}");
                    findData = true;
                    continue;
                }
                await Util.Log($"第:{page.ToString()}页: {TypeName}|Total: {readNum.ToString()}  OK: {saveNum.ToString()}");

                var startIdx = pageHtml.IndexOf(PageStartStr, StringComparison.Ordinal);
                if (startIdx < 0)
                {
                    await Util.Error($"no page data: {url}: {pageHtml}");
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
                    
                    // 保存详细SN列表
                    Util.AppendFile(cacheFile, sn);
                    var parseRet = await ParseAndSave(sn, html);
                    if (parseRet == 1)
                    {
                        Interlocked.Increment(ref saveNum);
                    }
                    else if(incr && parseRet == 0)
                    {
                        findData = false;
                        await Util.Log($"数据存在，增量抓取退出 {TypeName}");
                        break;// 抓取增量数据时，文件存在则退出
                    }
                    match = match.NextMatch();
                }
                page++;
                if (findData && isNet)
                {
                    await Task.Delay(5000);
                }
            } while (findData);

            await Util.Log($"本次完成，{TypeName}|Total: {readNum.ToString()}  OK: {saveNum.ToString()}");
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
        /// <returns></returns>
        protected virtual async Task<int> ParseAndSave(string sn, string html)
        {
            if (!ItemFilePath.Contains("{0}"))
            {
                await Util.Error(TypeName + " ItemFilePath 不含{0}");
                return -1;
            }
            var file = string.Format(ItemFilePath, sn);
            if (File.Exists(file))
                return 0;

            var obj = await ParseHtml(sn, html);
            if (obj != null)
            {
                Util.SeriaToFile(file, obj);
                return 1;
            }
            await Util.Error(TypeName + " 解析出错");
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

        /// <summary>
        /// 读取指定目录下的所有文件，转成Excel
        /// </summary>
        /// <param name="xlsfile"></param>
        public virtual async Task<bool> ReadAndToExcel(string xlsfile = null)
        {
            //var type = typeof(InvestEvt);
            //Console.WriteLine(type.FullName);
            //var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (var prop in props)
            //{
            //    Console.WriteLine(prop.Name);
            //    Console.WriteLine(ExcelHelper.GetPropDesc(prop));
            //}
            //return false;

            if (!ModelType.IsSubclassOf(typeof(BaseEvt)))
            {
                await Util.Error($"{ModelType.FullName} 必须是BaseEvt的子类");
                return false;
            }

            var dir = Path.GetDirectoryName(ItemFilePath);
            if (!Directory.Exists(dir))
            {
                await Util.Error($"{dir} 目录不存在");
                return false;
            }

            if (string.IsNullOrEmpty(xlsfile))
            {
                xlsfile = Path.Combine(dir, "all.xlsx");
            }

            var arr = new List<BaseEvt>();
            var arrFiles = Directory.GetFiles(dir);
            await Util.Log($"{dir} 文件个数:{arrFiles.Length.ToString()}");
            //foreach (var file in arrFiles)
            Parallel.ForEach(arrFiles, async file =>
            {
                if (file.IndexOf("all.json", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    !file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    return; // continue;
                try
                {
                    var evt = Util.DeSeriaFromFile(ModelType, file) as BaseEvt;
                    if (evt == null)
                    {
                        await Util.Error($"{file} 反序列化为空");
                        return;
                    }
                    arr.Add(evt);
                }
                catch (Exception exp)
                {
                    await Util.Error($"{file} {exp}");
                }
            });
           
            await Util.Log($"{dir} 对象个数:{arr.Count.ToString()} {xlsfile}");
            try
            {
                ExcelHelper.ToExcel(arr, xlsfile);
                return true;
            }
            catch (Exception exp)
            {
                await Util.Error($"{dir} {xlsfile} {exp}");
                return false;
            }
        }

        protected static string TrimVal(Match matData, int idx = 0)
        {
            var ret = matData.Groups[idx].Value;
            return RegexVal.Replace(ret, "").Replace("&gt;", ">").Trim();
        }


        static Regex RegEnt = new Regex(@"<a onclick=""addSearchIndex\('([^']+)',\s*\d+\)[^>]+href=""/(firm_[^>""]+)""[^>]*>[\s\S]+?地址：(.+?)\s*</p>", RegexOptions.Compiled);
        /// <summary>
        /// 根据公司名称，去企查查找地址.
        /// 返回 new string[] { entAddr, entUrl }
        /// </summary>
        /// <param name="entName"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static async Task<string[]> GetAddress(string entName, string sn)
        {
            var cookie = new CookieContainer();
            var entSearchUrl = $"https://www.qichacha.com/search?key={HttpUtility.UrlEncode(entName)}";
            await Util.GetPage("https://www.qichacha.com/", cookie);
            Thread.Sleep(600);
            var html = await Util.GetPage(entSearchUrl, cookie);
            var idxStart = html.IndexOf("家符合条件的企业", StringComparison.Ordinal);
            if (idxStart < 0)
            {
                var msg = $"{sn}-{entName} 地址未找到\r\n{html}";
                await Util.Error(msg);
                return null;
            }
            var match = RegEnt.Match(html, idxStart);
            if (!match.Success)
            {
                var msg = $"{sn}-{entName} 没有结果匹配\r\n{html}";
                await Util.Error(msg);
                return null;
            }
            if (match.Groups[1].Value != entName)
            {
                var msg = $"{sn}-{entName} 匹配企业名不一致\r\n{html}";
                await Util.Error(msg);
                return null;
            }
            var entUrl = match.Groups[2].Value;
            var entAddr = TrimVal(match, 3);
            return new string[] { entAddr, entUrl };
        }

    }
}
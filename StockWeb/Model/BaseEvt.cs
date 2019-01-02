using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StockWeb.Services;

namespace StockWeb.Model
{
    /// <summary>
    /// 基类事件
    /// </summary>
    public abstract class BaseEvt
    {
        #region 属性列表
        /// <summary>
        /// 详情地址
        /// </summary>
        [Description("详情地址")]
        public string Url { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号")]
        public string Sn { get; set; }
        /// <summary>
        /// 事件标题
        /// </summary>
        [Description("标题")]
        public string Title { get; set; }

        /// <summary>
        /// 融资时间
        /// </summary>
        [Description("融资时间")]
        public string Date { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        [Description("行业")]
        public string Industry { get; set; }

        #endregion


        protected static Regex RegNum = new Regex(@"\d+", RegexOptions.Compiled);
        protected static Regex _regexMain = new Regex(@"<div class=""box-fix-l""><div class=""info"">(.*?)</div></div>", RegexOptions.Compiled);
        protected static Regex _regexTitle = new Regex(@"<h1[^>]*>(.*?)</h1>", RegexOptions.Compiled);
        protected static Regex _regexData = new Regex(@"<li[^>]*><span[^>]*>.*?</span>((?!</?li>).*?)(?=</?li>)", RegexOptions.Compiled);
        protected static Regex _regexVal = new Regex(@"<[^>]+>", RegexOptions.Compiled);
        

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
                    if(!ret.Add(line))
                        Console.WriteLine(line);
                }
            }
            return ret;
        }

        protected static string TrimVal(Match matData)
        {
            var ret = matData.Groups[1].Value.Trim();
            return _regexVal.Replace(ret, "").Replace("&gt;", ">");
        }



        protected static void ReadAndToExcel<T>(string allItemFilename)
        {
            var dir = Path.GetDirectoryName(allItemFilename);
            var arr = new List<T>();
            Parallel.ForEach(Directory.GetFiles(dir), file =>
            {
                if (file.IndexOf("all.json", StringComparison.OrdinalIgnoreCase) >= 0)
                    return;
                try
                {
                    var evt = Util.SeriaFromFile<T>(file);
                    if (evt == null)
                    {
                        Console.WriteLine($"{file}  反序列化为空");
                        return;
                    }
                    arr.Add(evt);
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"{file}  {exp}");
                }
            });
            var xlsfile = Path.Combine(dir, "all.xlsx");
            Console.WriteLine($"{arr.Count.ToString()} {xlsfile}");
            try
            {
                ExcelHelper.ToExcel(arr, xlsfile);
            }
            catch (Exception exp)
            {
                Console.WriteLine($"{xlsfile}  {exp}");
            }
        }
    }
}

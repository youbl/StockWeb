using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockWin.Model;

namespace StockWin.Services
{
    public static class TaskService
    {
        internal static Dictionary<string, BaseService> ArrSites = new Dictionary<string, BaseService>()
        {
            {"创业邦", new StockWin.Services.CyzoneInvestService() },    // http://www.cyzone.cn
            {"投资界", new StockWin.Services.PedailyInvestService()},   // https://zdb.pedaily.cn
        };

        private static bool Catching;

        internal static void StartCatch(int hour, string siteName = null)
        {
            Catching = true;
            var sleepMs = hour * 3600000;
            ThreadPool.UnsafeQueueUserWorkItem(state => {
                while (Catching)
                {
                    ChgCatchStatus(StartOneTask, siteName);
                    Thread.Sleep(sleepMs);
                }
            }, null);
        }

        internal static int StopCatch(string siteName = null)
        {
            Catching = false;
            return ChgCatchStatus(StopOneTask, siteName);
        }

        delegate bool DoTask(string siteName);
        private static int ChgCatchStatus(DoTask method, string siteName = null)
        {
            var ret = 0;
            if (string.IsNullOrEmpty(siteName) || siteName == "全部")
            {
                foreach (var pair in ArrSites)
                {
                    if (method(pair.Key))
                    {
                        ret++;
                    }
                }
            }
            else
            {
                if (method(siteName))
                {
                    ret++;
                }
            }
            return ret;
        }

        /// <summary>
        /// 启动抓取任务，如果任务已经启动过，不再启动
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        static bool StartOneTask(string siteName)
            //static async Task<bool> StartOneTask(string siteName)
        {
            BaseService service = GetSelectedService(siteName);
            if (service == null)
                return false;

            if (service.Completed)
            {
                Task.Factory.StartNew(async () =>
                {
                    await service.SaveAllItems(true);
                    // MessageBox.Show("siteName抓取完成");
                });

                // await task; // await 会阻塞
                return true;
            }
            return false;
        }

        static bool StopOneTask(string siteName)
        {
            BaseService service = GetSelectedService(siteName);
            if (service == null)
                return false;
            service.Cancel = true;
            return true;
        }

        static BaseService GetSelectedService(string siteName)
        {
            if (ArrSites.TryGetValue(siteName, out var ret))
            {
                return ret;
            }
            return null;
            // throw new Exception("未找到匹配的抓取服务");
        }

        internal static void ExportExcel(string[] keywords, bool single, bool showTitle)
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            string file = "";
            if (single)
            {
                var arr = new List<BaseEvt>();
                foreach (var pair in ArrSites)
                {
                    arr.AddRange(pair.Value.ReadEvts(keywords));
                }
                file = Path.Combine(Environment.CurrentDirectory, $"{now}.xlsx");
                arr.Sort((x, y) => String.Compare(x.Title, y.Title, StringComparison.Ordinal));
                ExcelHelper.ToExcel(arr, file, showTitle);
            }
            else
            {
                foreach (var pair in ArrSites)
                {
                    file = Path.Combine(Environment.CurrentDirectory, $"{pair.Key}{now}.xlsx");
                    pair.Value.ReadAndToExcel(keywords, file, showTitle);
                }
            }
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                Process.Start("explorer", "/select," + file);
            }
            else
            {
                MessageBox.Show("导出失败。");
            }
            //Console.WriteLine("Begin xls");
            //new CyzoneInvestService().ReadAndToExcel();
            //new PedailyInvestService().ReadAndToExcel();
            //Console.WriteLine("OK");
        }


        internal static string CountTaskStatus()
        {
            var ret = new StringBuilder("上次完成时间：");
            var num = 0;
            foreach (var pair in ArrSites)
            {
                ret.AppendFormat("{0}:{1};", pair.Value.SiteName, pair.Value.LastCatchTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (!pair.Value.Completed)
                {
                    num++;
                }
            }
            ret.Insert(0, $"抓取中任务数:{num.ToString()}个　    ");
            return ret.ToString();
        }
    }
}
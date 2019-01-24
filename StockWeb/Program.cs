using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using StockWeb.Model;
using StockWeb.Services;

namespace StockWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Factory.StartNew(Run);
            // Run();

            //FinishAddr();
            Console.ReadLine();

            // 启动Web监听
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        static async Task Run()
        {
            // 导出Excel的代码
            //Console.WriteLine("Begin xls");
            //new CyzoneInvestService().ReadAndToExcel();
            //new PedailyInvestService().ReadAndToExcel();
            //Console.WriteLine("OK");
            //Console.Read();


            //// 全量抓取的代码
            //var task1 = Task.Factory.StartNew(async () => {
            //    var retF1 = await new CyzoneInvestService().SaveAllItems(false);
            //    Util.Log($"CyzoneInvestService全量抓取完成:{retF1.ToString()}");
            //});
            //var task2 = Task.Factory.StartNew(async () => {
            //    var retF2 = await new PedailyInvestService().SaveAllItems(false);
            //    Util.Log($"PedailyInvestService全量抓取完成:{retF2.ToString()}");
            //});

            // 增量抓取的代码
            var waitMs = 3600 * 1000 * 24; // 一天
            while (true)
            {
                // 开启增量抓取
                var ret1 = await new CyzoneInvestService().SaveAllItems(true);
                var ret2 = await new PedailyInvestService().SaveAllItems(true);
                Util.Log($"一轮增量抓取完成:{ret1.ToString()}|{ret2.ToString()}");
                await Task.Delay(waitMs);
            }


            // 上市事件采集
            //await Task.Factory.StartNew(async ()=>
            //{
            //    var ret = await ListedEvt.SaveAll();
            //    Console.WriteLine(ret);
            //});
            // ReSharper disable once FunctionNeverReturns
        }

        static async Task FinishAddr()
        {
            var isnet = false;
            var num = 0;
            var fail = 0;
            var dir = @"E:\mysource\StockWeb\StockWeb\bin\Release\netcoreapp2.2\InvestedCyzone";
            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                //}
                //Parallel.ForEach(files, async file =>
                //{
                if (num > 0 && isnet)
                    Thread.Sleep(5000);
                isnet = false;
                if (num % 100 == 0)
                {
                    Console.WriteLine(num.ToString() + " " + fail.ToString());
                }
                Interlocked.Increment(ref num);

                if (file.IndexOf("all.json", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    !file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    fail++;
                    continue;
                }
                try
                {
                    var evt = Util.DeSeriaFromFile<InvestEvt>(file);
                    if (evt == null)
                    {
                        Util.Error($"{file} 反序列化为空");
                        fail++;
                        continue;
                    }
                    var newfile = Path.Combine(dir + "new", evt.Sn + ".json");
                    var newfileNo = newfile + "no";
                    if (File.Exists(newfile) || File.Exists(newfileNo))
                    {
                        fail++;
                        continue;
                    }
                    isnet = true;
                    var addrArr = await BaseService.GetAddressFromQcc(evt.RecieveEnt, evt.Sn);
                    // var addrArr = await BaseService.GetAddressFromTyc(evt.RecieveEnt, evt.Sn);
                    if (addrArr == null)
                    {
                        Util.SeriaToFile(newfileNo, evt);
                        // File.Copy(file, newfileNo);
                        fail++;
                        continue;
                    }
                    //evt.Address = addrArr[0];
                    //evt.UrlEnt = addrArr[1];
                    Util.SeriaToFile(newfile, evt);
                }
                catch (Exception exp)
                {
                    Util.Error($"{file} {exp}");
                }
            }
            Console.WriteLine("OK " + num.ToString() + " " + fail.ToString());

        }
    }
}

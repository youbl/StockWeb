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
            // Task.Factory.StartNew(Run);
            // Run();

            FinishAddr();
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
            //await new CyzoneInvestService().ReadAndToExcel();
            //await new PedailyInvestService().ReadAndToExcel();
            //Console.WriteLine("OK");
            //Console.Read();

            var waitMs = 3600 * 1000 * 24; // 一天
            while (true)
            {
                // 开启增量抓取
                var ret1 = await new CyzoneInvestService().SaveAllItems(true);
                var ret2 = await new PedailyInvestService().SaveAllItems(true);
                await Util.Log($"一轮增量抓取完成:{ret1.ToString()}|{ret2.ToString()}");
                await Task.Delay(waitMs);
            }

            // ListedEvt.ReadAndToExcel();
            // InvestEvt.ReadAndToExcel();
            // Console.WriteLine("OK");

            //ListedEvt.Rename();
            //return;

            // 上市事件采集
            //await Task.Factory.StartNew(async ()=>
            //{
            //    var ret = await ListedEvt.SaveAll();
            //    Console.WriteLine(ret);
            //});

            // 投资事件采集
            //await Task.Factory.StartNew(async () =>
            //{
            //    var ret = await InvestEvt.SaveAll();
            //    Console.WriteLine(ret);
            //});
            // ReSharper disable once FunctionNeverReturns
        }

        static async Task FinishAddr()
        {
            var num = 0;
            var fail = 0;
            var dir = @"D:\mine\StockWeb-master\StockWeb\bin\Release\netcoreapp2.2\aaaa";
            var files = Directory.GetFiles(dir);
            await Util.GetPage("https://www.qichacha.com/");
            foreach (var file in files)
            {
            //}
            //Parallel.ForEach(files, async file =>
            //{
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
                        await Util.Error($"{file} 反序列化为空");
                        fail++;
                        continue;
                    }
                    var addrArr = await BaseService.GetAddress(evt.RecieveEnt, evt.Sn);
                    if (addrArr == null)
                    {
                        fail++;
                        continue;
                    }
                    evt.Address = addrArr[0];
                    evt.UrlEnt = addrArr[1];
                    var newfile = Path.Combine(dir + "new", evt.Sn + ".json");
                    Util.SeriaToFile(newfile, evt);
                    Interlocked.Increment(ref num);

                    Thread.Sleep(5000);
                    if (num % 100 == 0)
                    {
                        Console.WriteLine(num.ToString() + " " + fail.ToString());
                    }
                }
                catch (Exception exp)
                {
                    await Util.Error($"{file} {exp}");
                }
            }
            Console.WriteLine("OK " + num.ToString() + " " + fail.ToString());

        }
    }
}

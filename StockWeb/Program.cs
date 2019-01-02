using System;
using System.Collections.Generic;
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
            Run();
            Console.ReadLine();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        static async Task Run()
        {
            ListedEvt.ReadAndToExcel();
            // InvestEvt.ReadAndToExcel();
            Console.WriteLine("OK");

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
        }
    }
}

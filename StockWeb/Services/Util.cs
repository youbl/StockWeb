using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockWeb.Services
{
    public static class Util
    {
        private static string LogFile = Path.Combine(Environment.CurrentDirectory, "log.txt");
        private static readonly CookieContainer Cookie = new CookieContainer();
        /// <summary>
        /// 读取网页内容返回
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetPage(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.CookieContainer = Cookie;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            request.Referer = "https://zdb.pedaily.cn/inv/";
            using (var response = await request.GetResponseAsync())
                // ReSharper disable once AssignNullToNotNullAttribute
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                return stream.ReadToEnd();
            }
        }


        public static void SaveFile(string file, string str)
        {
            CreateDir(file);
            using (var stream = new StreamWriter(file, false, Encoding.UTF8))
            {
                stream.Write(str);
            }
        }
        public static string ReadFile(string file)
        {
            using (var stream = new StreamReader(file, Encoding.UTF8))
            {
                return stream.ReadToEnd();
            }
        }

        public static void SeriaToFile(string filename, object obj)
        {
            CreateDir(filename);
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var stream = new StreamWriter(filename, false, Encoding.UTF8))
                //using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                serializer.Serialize(stream, obj);
            }
        }


        public static T SeriaFromFile<T>(string filename)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (var file = new StreamReader(filename, Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                return serializer.Deserialize<T>(reader);
            }

        }

        static void CreateDir(string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public static async Task Log(string msg)
        {
            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + msg;
            Console.WriteLine(msg);
            using (var sw = new StreamWriter(LogFile, true, Encoding.UTF8))
            {
                await sw.WriteAsync(msg);
            }
        }
    }
}

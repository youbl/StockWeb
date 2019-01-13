using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockWin.Services
{
    public static class Util
    {
        private static string LogFile = Path.Combine(Environment.CurrentDirectory, "loginfo.txt");
        private static string ErrorFile = Path.Combine(Environment.CurrentDirectory, "logerr.txt");
        private static readonly CookieContainer Cookie = new CookieContainer();

        static readonly object lockObj = new object();
        /// <summary>
        /// 读取网页内容返回
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static async Task<string> GetPage(string url, CookieContainer cookie = null)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.CookieContainer = cookie ?? Cookie;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            request.Referer = url;
            var ret = new StringBuilder();
            using (var response = await request.GetResponseAsync())
                // ReSharper disable once AssignNullToNotNullAttribute
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                ret.AppendFormat("{0}\r\n===\r\n", request.Headers);
                ret.AppendFormat("{0}\r\n===\r\n", response.Headers);
                ret.AppendFormat("{0}\r\n", stream.ReadToEnd());
            }
            return ret.ToString();
        }

        public static void AppendFile(string file, string str)
        {
            CreateDir(file);
            using (var stream = new StreamWriter(file, true, Encoding.UTF8))
            {
                stream.WriteLine(str);
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



        /// <summary>
        /// 从文件反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static object DeSeriaFromFile(Type type, string filename)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (var file = new StreamReader(filename, Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                return serializer.Deserialize(reader, type);
                //return serializer.Deserialize<T>(reader);
            }

        }

        /// <summary>
        /// 从文件反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T DeSeriaFromFile<T>(string filename)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (var file = new StreamReader(filename, Encoding.UTF8))
            using (var reader = new JsonTextReader(file))
            {
                return serializer.Deserialize<T>(reader);
            }

        }

        /// <summary>
        /// 目录不存在时创建
        /// </summary>
        /// <param name="filename"></param>
        public static void CreateDir(string filename)
        {
            var dir = Path.GetDirectoryName(filename) ?? "";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static void Log(string msg)
        {
            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + msg + "\r\n";
            Console.WriteLine(msg);

            lock (lockObj)
            {
                using (var sw = new StreamWriter(LogFile, true, Encoding.UTF8))
                {
                    sw.WriteLine(msg);
                }
            }
        }

        /// <summary>
        /// 同步记录错误日志.
        /// await 不能进行lock,
        /// 如果要异步写文件并加lock，只能用队列
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static void Error(string msg)
        {
            msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + msg + "\r\n";
            Console.WriteLine(msg);

            lock (lockObj)
            {
                using (var sw = new StreamWriter(ErrorFile, true, Encoding.UTF8))
                {
                    sw.WriteLine(msg);
                    //await sw.WriteLineAsync(msg);
                }
            }
        }
    }
}

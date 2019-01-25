using System;
using System.Reflection;
using System.Windows.Forms;
using StockWin.Model;
using StockWin.Services;

namespace StockWin
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var html = Services.Util.ReadFile("d:/1.txt");
            //var aa = new InvestEvt();
            //new PedailyInvestService().RealParse(aa, html);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // += new ResolveEventHandler(AssemblyResolve_ResxFile);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.Run(new MainForm());
        }

        #region 从exe嵌入资源里加载dll的方法, Web项目不要使用

        private static Assembly exeAss = Assembly.GetEntryAssembly();
        private static System.Resources.ResourceManager resManager;
        private static System.Resources.ResourceManager ResManager
        {
            get
            {
                if (resManager == null)
                {
                    var entryNamespace = exeAss.GetTypes()[0].Namespace;
                    resManager = new System.Resources.ResourceManager(entryNamespace + ".Properties.Resources",
                        Assembly.GetExecutingAssembly());
                }
                return resManager;
            }
        }

        /// <summary>
        /// 从嵌入的资源文件里加载dll,调用方法：在Main函数里执行：
        /// AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // System.Reflection.RuntimeAssembly exeAss;  exeAss.GetManifestResourceNames()
            var idx = args.Name.IndexOf(',');
            string dllName = idx > 0 ? args.Name.Substring(0, idx) : args.Name.Replace(".dll", "");
            if (dllName.EndsWith(".resources"))
            {
                return null;
            }

            // 读取那些 生成操作为“嵌入的资源”的数据
            string resourceName = $"{exeAss.GetTypes()[0].Namespace}.{dllName}.dll";
            using (var stream = exeAss.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    var len = (int)stream.Length;
                    var assemblyData = new byte[len];
                    if (stream.Read(assemblyData, 0, len) == len)
                    {
                        return Assembly.Load(assemblyData);
                    }
                }
            }

            // 读取那些 嵌入在“Resources.resx”里的数据
            dllName = dllName.Replace(".", "_");
            try
            {
                var bytes = (byte[])ResManager.GetObject(dllName);
                if (bytes == null)
                {
                    return null;
                }
                return Assembly.Load(bytes);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }
}

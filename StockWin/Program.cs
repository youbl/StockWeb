using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DependentFiles.LoadResourceDll();       // 载入资源dll文件

            Application.Run(new MainForm());
        }
    }

    /// <summary>
    /// QRTool.DependentFiles.LoadResourceDll();       // 载入资源dll文件
    /// </summary>
    class DependentFiles
    {
        /// <summary>
        /// 载入资源文件中附带的所有dll文件
        /// </summary>
        public static void LoadResourceDll()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");
            dllName = dllName.Replace(".", "_");
            if (dllName.EndsWith("_resources")) return null;
            string Namespace = Assembly.GetEntryAssembly().GetTypes()[0].Namespace;
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());
            byte[] bytes = (byte[])rm.GetObject(dllName);
            return System.Reflection.Assembly.Load(bytes);
        }

    }
}

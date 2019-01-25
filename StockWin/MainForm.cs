using System;
using System.Configuration;
using System.Windows.Forms;
using StockWin.Services;

namespace StockWin
{
    public partial class MainForm : Form
    {        

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lstSites.Items.Add("全部");
            lstSites.SelectedIndex = 0;
            foreach (var pair in TaskService.ArrSites)
            {
                lstSites.Items.Add(pair.Key);
            }
            var autoStart = ConfigurationManager.AppSettings["AutoStart"] ?? "true";
            if(autoStart.Equals("true", StringComparison.OrdinalIgnoreCase))
                chkStartCatch.Checked = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var begin = DateTime.Now;
            var keywords = txtKeyWords.Text.Split(new char[] {' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            TaskService.ExportExcel(keywords, chkSingle.Checked, chkTitle.Checked);
            ShowTime(begin);
            //Console.WriteLine("Begin xls");
            //new CyzoneInvestService().ReadAndToExcel();
            //new PedailyInvestService().ReadAndToExcel();
            //Console.WriteLine("OK");
        }

        private void chkStartCatch_CheckedChanged(object sender, EventArgs e)
        {
            // 全量抓取代码：
            // new PedailyInvestService().SaveAllItems(false);
            // new CyzoneInvestService().SaveAllItems(false);
            // return;
            var chk = chkStartCatch;
            if (chk.Checked)
            {
                if (!int.TryParse(txtCatchHour.Text.Trim(), out var hour))
                {
                    return;
                }
                TaskService.StartCatch(hour);
            }
            else
            {
                TaskService.StopCatch();
            }
        }

        void ShowTime(DateTime begin, string msg = null)
        {
            var time = (DateTime.Now - begin).TotalSeconds;
            msg = msg ?? "操作完成";
            labTime.Text = $"{msg} 耗时：{time.ToString("N2")}秒";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var ret = TaskService.CountTaskStatus();
            labTaskNum.Text = ret;
        }
    }
}

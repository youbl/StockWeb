using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockWeb.Model
{
    public class ReadyEnt
    {
        //序号	企业名称	注册地	主营业务	保荐机构	通过发审日期/申报IPO日期/备案日期	辅导工作进度	备注
        public string Sn { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Business { get; set; }
        public string Org { get; set; }
        public string Date { get; set; }
        public string Process { get; set; }
        public string Memo { get; set; }
    }
}

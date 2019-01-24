namespace StockWeb.Model
{
    public class ListedEnt
    {
        // 序号	证券代码	证券简称	办公地址	公司电话	董 秘	上市日期
        public string Sn { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Secretary { get; set; }
        public string Date { get; set; }
    }
}

using System;
using System.ComponentModel;

namespace StockWin.Model
{
    /// <summary>
    /// 基类事件
    /// </summary>
    public abstract class BaseEvt
    {
        #region 属性列表

        /// <summary>
        /// 事件标题
        /// </summary>
        [Description("标题")]
        public string Title { get; set; }

        /// <summary>
        /// 融资时间
        /// </summary>
        [Description("融资时间")]
        public string Date { get; set; }

        /// <summary>
        /// 融资时间
        /// </summary>
        [Description("融资年")]
        public string DateYear
        {
            get
            {
                if (string.IsNullOrEmpty(Date) || Date.Length < 4)
                {
                    return "";
                }
                return Date.Substring(0, 4);
            }
        }

        private string _indus;

        /// <summary>
        /// 行业
        /// </summary>
        [Description("行业")]
        public string Industry
        {
            get { return _indus; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var arr = value.Split(new char[] {'\r', '\n', '>'}, StringSplitOptions.RemoveEmptyEntries);
                    Ind1 = arr[0].Trim();
                    if(arr.Length > 1) Ind2 = arr[1].Trim();
                    if (arr.Length > 2) Ind3 = arr[2].Trim();
                }
                _indus = value;
            }
        }
        
        [Description("一级行业")]
        public string Ind1 { get; set; }


        [Description("二级行业")]
        public string Ind2 { get; set; }


        [Description("三级行业")]
        public string Ind3 { get; set; }


        //[Description("省份")]
        //public string Province
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(Address) || Address.Length < 2)
        //            return "";
        //        return Address.Substring(0, 2);
        //    }
        //}

        ///// <summary>
        ///// 地址
        ///// </summary>
        //[Description("地址")]
        //public string Address { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Description("序号")]
        public string Sn { get; set; }

        /// <summary>
        /// 融资详情地址
        /// </summary>
        [Description("融资详情")]
        public string Url { get; set; }


        ///// <summary>
        ///// 企查查详情地址
        ///// </summary>
        //[Description("企查查详情")]
        //public string UrlEnt { get; set; }

        /// <summary>
        /// 所属分页
        /// </summary>
        [Description("所属分页")]
        public string Page { get; set; }


        /// <summary>
        /// 采集时间
        /// </summary>
        [Description("采集时间")]
        public DateTime CatchTime { get; set; } = DateTime.Now;

        #endregion
    }
}

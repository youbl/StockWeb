using System.ComponentModel;

namespace StockWeb.Model
{
    /// <summary>
    /// 上市事件
    /// </summary>
    public class ListedEvt : BaseEvt
    {
        #region 属性列表

        /// <summary>
        /// 公司名称
        /// </summary>
        [Description("公司名称")]
        public string Name { get; set; }

        /// <summary>
        /// 投资方
        /// </summary>
        [Description("投资方")]
        public string InvestName { get; set; }

        /// <summary>
        /// 发行价
        /// </summary>
        [Description("发行价")]
        public string Price { get; set; }
        /// <summary>
        /// 上市地点
        /// </summary>
        [Description("上市地点")]
        public string ListedPlace { get; set; }
        /// <summary>
        /// 发行量
        /// </summary>
        [Description("发行量")]
        public string ListedNum { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        [Description("股票代码")]
        public string Code { get; set; }
        /// <summary>
        /// 是否VC/PE支持
        /// </summary>
        [Description("是否VC/PE支持")]
        public string VcSuport { get; set; }


        #endregion
        
    }
}

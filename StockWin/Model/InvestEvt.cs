using System.ComponentModel;

namespace StockWin.Model
{
    /// <summary>
    /// 投资事件
    /// </summary>
    public class InvestEvt : BaseEvt
    {
        #region 属性列表

   
        /// <summary>
        /// 融  资  方
        /// </summary>
        [Description("融资方")]
        public string RecieveEnt { get; set; }
        /// <summary>
        /// 投  资  方
        /// </summary>
        [Description("投资方")]
        public string PayEnt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Description("金额")]
        public string Money { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Description("融资轮次")]
        public string Rounds { get; set; }
        #endregion
        
    }
}

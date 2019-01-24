using System;
using System.ComponentModel;

namespace StockWeb.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class EvtAttribute : Attribute
    {
       
        public EvtAttribute(bool show)
        {
            //Sort = sort;
            Show = show;
        }

        //public int Sort { get; set; }
        public bool Show { get; set; }
    }
}

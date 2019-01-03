using System;
using System.IO;
using System.Text.RegularExpressions;
using StockWeb.Model;

namespace StockWeb.Services
{
    public class CyzoneInvestService : BaseService
    {
        private static string domain = "http://www.cyzone.cn";

        public override string PageUrl { get; } = domain + "/event/list-0-{0}-0-0-0-0/0"; // 1464页

        public override string ItemFilePath { get; } =
            Path.Combine(Environment.CurrentDirectory, $"InvestedCyzone/") + "{0}.json";

        public override string PageStartStr { get; } = "<h2>投资事件</h2>";

        protected override Regex RegPage { get; } =
            new Regex(@"<tr\s[^>]*data-url=""//www.cyzone.cn/event/(\d+).html"">([\s\S]*?)</tr>",
                RegexOptions.Compiled);

        protected override void ParseAndSave(string sn, string html)
        {
            /*
<tr class="table-plate3" data-url="//www.cyzone.cn/event/489229.html">
<td class="tp1">
<a href="//www.cyzone.cn/company/292562.html"><img src="//oss.cyzone.cn/2015/0601/20150601023926587.png?x-oss-process=image/resize,w_140,h_140,limit_0"></a>
</td>
<td class="tp2">
<span class="tp2_tit"><a href="//www.cyzone.cn/company/292562.html" target="_blank">云迹科技</a></span><br>
<span class="tp2_com">北京云迹科技有限公司</span>
</td>
<td class="tp-mean">
<div class="money">未公开</div>
</td>
<td>战略投资</td>
<td class="tp3" title="292562">
<a href="//www.cyzone.cn/capital/201245.html" target="_blank">携程</a><br></td>
<td><a href="//www.cyzone.cn/event/list-3551-1-0-0-0-0/">人工智能</a><br>
</td>
<td>2019-01-03</td>
<td><a href="//www.cyzone.cn/event/489229.html" class="show-detail" target="_blank" rel="nofollow">详情</a></td>
</tr>             
             */
            var evt = new InvestEvt();
            evt.Sn = sn;
            evt.Url = $"{domain}/event/{sn}.html";

            // 第一个是图片
            var match = _regexTd.Match(html);

            match = match.NextMatch();
            var titleAndEnt = TrimVal(match).Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            evt.Title = titleAndEnt[0];
            evt.RecieveEnt = titleAndEnt[titleAndEnt.Length - 1];

            match = match.NextMatch();
            evt.Money = TrimVal(match);

            match = match.NextMatch();
            evt.Rounds = TrimVal(match);

            match = match.NextMatch();
            evt.PayEnt = TrimVal(match);

            match = match.NextMatch();
            evt.Industry = TrimVal(match);

            match = match.NextMatch();
            evt.Date = TrimVal(match);

            var file = string.Format(ItemFilePath, evt.Sn);
            SaveToFile(file, evt);
            SaveNum++;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core.Report.PlugIn;
using Kingdee.BOS.Core.Report.PlugIn.Args;

namespace Kingdee.Samples.Report.Normal
{
    [Description("格式化报表数据行表单插件")]
    [HotUpdate]
    public class FormatRowEdit : AbstractSysReportPlugIn
    {
        public override void OnFormatRowConditions(ReportFormatConditionArgs args)
        {
            base.OnFormatRowConditions(args);
            //金额等于或超过10000的数据行为浅蓝色
            if (args.DataRow.ColumnContains("FALLAMOUNT") && Convert.ToDecimal(args.DataRow["FALLAMOUNT"]) >= 10000m)
            {
                var format = new BOS.Core.Metadata.FormatCondition();
                format.BackColor = ColorTranslator.ToHtml(Color.LightBlue);
                format.ApplayRow = true;
                args.FormatConditions.Add(format);
            }
        }
    }
}


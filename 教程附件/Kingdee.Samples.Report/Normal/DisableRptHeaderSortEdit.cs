using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core.Report.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.ControlModel;

namespace Kingdee.Samples.Report.Normal
{
    [Description("禁用报表列头排序功能")]
    [HotUpdate]
    public class DisableRptHeaderSortEdit : AbstractSysReportPlugIn
    {
        public override void AfterBindData(EventArgs e)
        {
            base.AfterBindData(e);
            EntryGrid listGrid = View.GetControl<EntryGrid>("FList");
            listGrid.SetCustomPropertyValue("AllowSorting", false);
        }
    }
}

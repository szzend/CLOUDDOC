using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace Zy.KDTest.PlugIn
{
    [HotUpdate]
    [Description("字段值更新时，插件进行相关处理")]
    public class DemoZy003:AbstractDynamicFormPlugIn
    {
        public override void DataChanged(DataChangedEventArgs e)
        {
            base.DataChanged(e);

            switch (e.Field.Key.ToLower())
            {
                case "f_pae_creator":
                    this.View.ShowMessage(string.Format("修改前的值是：{0},修改后的值是：{1}", e.OldValue, e.NewValue));
                    break;
            }
        }

        public override void BeforeUpdateValue(BeforeUpdateValueEventArgs e)
        {
            base.BeforeUpdateValue(e);

            switch (e.Key.ToLower())
            {
                case "f_pae_quantity":
                    if (Convert.ToDecimal(e.Value) > 10)
                    {
                        this.View.ShowErrMessage("数量不能超10");
                        e.Cancel = true;
                    }
                    break;
            }
        }
    }
}

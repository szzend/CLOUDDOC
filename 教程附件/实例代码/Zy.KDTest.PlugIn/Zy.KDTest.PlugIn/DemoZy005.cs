using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.Core.Metadata.EntityElement;
using Kingdee.BOS.Core.DynamicForm.PlugIn.ControlModel;

namespace Zy.KDTest.PlugIn
{
    [HotUpdate]
    [Description("干预菜单点击事件")]
    public class DemoZy005:AbstractDynamicFormPlugIn
    {
        public override void BarItemClick(BarItemClickEventArgs e)
        {
            base.BarItemClick(e);

            switch (e.BarItemKey.ToLower())
            {
                case "tbsplitsave":
                case "tbsave":
                    object objCreator = this.Model.GetValue("F_PAE_Creator");
                    if (objCreator.IsNullOrEmptyOrWhiteSpace())
                    {
                        this.View.ShowErrMessage("对不起，创建人不能为空");
                        e.Cancel = true;
                    }
                    
                    break;
            }
        }
    }
}

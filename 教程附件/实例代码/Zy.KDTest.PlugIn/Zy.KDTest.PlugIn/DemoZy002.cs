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
    [Description("插件控制菜单以及字段可见性和可用性")]
    public class DemoZy002:AbstractDynamicFormPlugIn
    {
        public override void ButtonClick(ButtonClickEventArgs e)
        {
            base.ButtonClick(e);

            switch (e.Key.ToLower())
            {
                case "f_pae_disablebaritem":
                    //禁用主菜单
                    this.View.GetMainBarItem("tbButtonTest").Enabled = false;
                    this.View.GetControl("F_PAE_OrderAmount").Enabled = false;
                    break;
                case "f_pae_enablebaritem":
                    //启用主菜单
                    this.View.GetMainBarItem("tbButtonTest").Enabled = true;
                     this.View.GetControl("F_PAE_OrderAmount").Enabled = true;
                    break;
                case "f_pae_hidebaritem":
                    //隐藏主菜单
                    this.View.GetMainBarItem("tbButtonTest").Visible = false;
                    break;
                case "f_pae_showbaritem":
                    //显示主菜单
                    this.View.GetMainBarItem("tbButtonTest").Visible = true;
                    break;
            }
        }
    }
}

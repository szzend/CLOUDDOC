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
using Kingdee.BOS.Core.DynamicForm;

namespace Zy.KDTest.PlugIn
{
    [HotUpdate]
    [Description("插件调用操作中的保存操作")]
    public class DemoZy006:AbstractDynamicFormPlugIn
    {
        public override void ButtonClick(ButtonClickEventArgs e)
        {
            base.ButtonClick(e);

            switch (e.Key.ToLower())
            {
                case "f_pae_callsave":


                    this.View.InvokeFormOperation(FormOperationEnum.Save);

                    break;
            }
        }
    }
}

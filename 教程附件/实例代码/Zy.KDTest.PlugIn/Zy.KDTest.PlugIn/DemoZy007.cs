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
    [Description("动态调用动态表单，以及传递数据")]
    public class DemoZy007:AbstractDynamicFormPlugIn
    {
        public override void ButtonClick(ButtonClickEventArgs e)
        {
            base.ButtonClick(e);

            switch (e.Key.ToLower())
            {
                case "f_pae_ok":
                    this.View.ReturnToParentWindow(this.Model.DataObject);
                    this.View.Close();
                    break;
                case "F_PAE_Cancel":
                    this.View.Close();
                    break;
            }
        }

        private DynamicObject selectItem;

        public override void OnInitialize(InitializeEventArgs e)
        {
            base.OnInitialize(e);

            if (this.View.ParentFormView != null)
            {
                selectItem = this.View.OpenParameter.GetCustomParameter("SelectItem") as DynamicObject;
            }
        }

        public override void AfterCreateNewData(EventArgs e)
        {
            base.AfterCreateNewData(e);

            if (selectItem != null)
            {
                this.Model.DataObject["F_PAE_ItemName"] = selectItem["F_PAE_ItemName"].ToString();
                this.Model.DataObject["F_PAE_Price"] = selectItem["F_PAE_Price"].ToString();
                this.Model.DataObject["F_PAE_Quantity"] = selectItem["F_PAE_Quantity"].ToString();
            }
            
        }

        public override void AfterBindData(EventArgs e)
        {
            base.AfterBindData(e);
        }

        public override void EntityRowDoubleClick(EntityRowClickEventArgs e)
        {
            base.EntityRowDoubleClick(e);

            if (e.Key.EqualsIgnoreCase("FEntity"))
            {
                DynamicObject dyo;
                int selectedRow = 0;
                this.Model.TryGetEntryCurrentRow("FEntity", out dyo, out selectedRow);
                if (dyo != null)
                {
                    if (dyo["F_PAE_ItemName"].IsNullOrEmptyOrWhiteSpace())
                    {
                        this.View.ShowErrMessage("商品名称不能为空");
                        return;
                    }

                    DynamicFormShowParameter param = new DynamicFormShowParameter();
                    param.FormId = "PAE_ZyDemoBase2";
                    param.CustomComplexParams.Add("SelectItem", dyo);

                    this.View.ShowForm(param, (result) =>
                     {
                         if (result != null)
                         {
                             DynamicObject resultDyo = result.ReturnData as DynamicObject;
                             if (resultDyo == null)
                                 return;
                             this.Model.SetValue("F_PAE_ItemName", resultDyo["F_PAE_ItemName"]);
                             this.Model.SetValue("F_PAE_Price", resultDyo["F_PAE_Price"]);
                             this.Model.SetValue("F_PAE_Quantity", resultDyo["F_PAE_Quantity"]);
                         }
                     });
                }
            }
        }
    }
}

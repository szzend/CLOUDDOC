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
    [Description("控件字段的取值和赋值常用方法")]
    public class DemoZy004:AbstractDynamicFormPlugIn
    {
        public override void ButtonClick(ButtonClickEventArgs e)
        {
            base.ButtonClick(e);

            switch (e.Key.ToLower())
            {
                case "f_pae_getvalue":

                    ShowGetValue();

                    break;
                case "f_pae_setvalue":

                    ShowSetValue();

                    break;
            }
        }

        private void ShowGetValue()
        {
            string message = "";

            //获取单据体文本字段 
            object billNo = this.Model.GetValue("FBillNo");
            //object billNo = this.Model.DataObject["FBillNo"];
            //var billNoField = this.View.BillBusinessInfo.GetField("FBillNo");
            //object billNo = billNoField.DynamicProperty.GetValue(this.Model.DataObject);
            if (billNo != null)
            {
                message += "[FBillNo:" + billNo.ToString() + "] ";
            }

            //获取基础资料字段
            DynamicObject baseCoin= this.Model.GetValue("F_PAE_BaseCoin") as DynamicObject;
            if (baseCoin != null)
            {
                message += "[F_PAE_BaseCoin:" + baseCoin["Name"] + "] ";
            }

            //获取单据体中的所有行数据
            Entity fentity = this.View.BillBusinessInfo.GetEntity("FEntity");
            DynamicObjectCollection entityObjs= this.Model.GetEntityDataObject(fentity);
            for(int i = 0; i < entityObjs.Count; i++)
            {
                message += "[row "+(i+1)+":"+entityObjs[i]["F_PAE_ItemName"]+"]";
            }

            //获取单据体选中行的数据
            int[] selectedRows = this.View.GetControl<EntryGrid>("FEntity").GetSelectedRows();
            DynamicObjectCollection entityObjs2= this.Model.GetEntityDataObject(fentity);
            foreach(var row in selectedRows)
            {
                message += "[selected row "+(row+1)+":"+entityObjs[row]["F_PAE_ItemName"]+"]";
            }

            //快速获取单据体中某个字段的值（如果行不存在时，会中断）
            object itemName = this.Model.GetValue("F_PAE_ItemName", 0);
            if (itemName != null)
            {
                message += "[quick row 1:"+itemName.ToString()+"]";
            }

            this.View.ShowMessage(message);

        }

        private void ShowSetValue()
        {
            //设置单据头字段的值
            this.Model.SetValue("FBillNo", "test setValue");

            //设置基础资料的值（根据内码）
            this.Model.SetValue("F_PAE_BaseCoin", "1");

            //设置基础资料的值（根据编码）
            this.Model.SetItemValueByNumber("F_PAE_BaseCoin", "PRE001", -1);


            //设置单据体中字段的值
            this.Model.SetValue("F_PAE_Quantity", 5, 0);
            this.Model.SetValue("F_PAE_Price", 20, 0);


            //this.Model.DataObject赋值与this.Model.SetValue的差异
            //this.Model.DataObject["F_PAE_Creator"] = "小熊";
            //this.Model.SetValue("F_PAE_Creator", "小熊");
        }

    }
}

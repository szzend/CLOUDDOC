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
    [Description("我的第一个单据插件")]
    public class DemoZy001:AbstractDynamicFormPlugIn
    {
        public override void PreOpenForm(PreOpenFormEventArgs e)
        {
            base.PreOpenForm(e);
        }

        public override void BeforeUpdateValue(BeforeUpdateValueEventArgs e)
        {
            base.BeforeUpdateValue(e);

            if (e.Key.EqualsIgnoreCase("FBILLNO"))
            {
                this.View.GetMainBarItem("tbButtonTest").Enabled = false;
                this.View.GetControl("F_PAE_OrderAmount").Enabled = false;
                this.Model.SetValue("F_PAE_OrderAmount", 100);
            }
        }

        public override void OnInitializeService(InitializeServiceEventArgs e)
        {
            base.OnInitializeService(e);
        }

        public override void OnSetBusinessInfo(SetBusinessInfoArgs e)
        {
            base.OnSetBusinessInfo(e);
        }

        public override void OnSetLayoutInfo(SetLayoutInfoArgs e)
        {
            base.OnSetLayoutInfo(e);
        }

        public override void OnCreateDataBinder(CreateDataBinderArgs e)
        {
            base.OnCreateDataBinder(e);
        }

        public override void OnInitialize(InitializeEventArgs e)
        {
            base.OnInitialize(e);
        }

        public override void CreateNewData(BizDataEventArgs e)
        {
            base.CreateNewData(e);
        }

        public override void BeforeCreateNewEntryRow(BeforeCreateNewEntryEventArgs e)
        {
            base.BeforeCreateNewEntryRow(e);
        }

        public override void AfterCreateNewEntryRow(CreateNewEntryEventArgs e)
        {
            base.AfterCreateNewEntryRow(e);
        }

        public override void AfterCreateModelData(EventArgs e)
        {
            base.AfterCreateModelData(e);
        }

        public override void AfterCreateNewData(EventArgs e)
        {
            base.AfterCreateNewData(e);
        }

        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

         public override void BeforeBindData(EventArgs e)
        {
            base.BeforeBindData(e);
        }

        public override void AfterBindData(EventArgs e)
        {
            base.AfterBindData(e);

            //this.View.ShowMessage("大家好，这是我的第一个单据插件");

            //this.View.GetControl("F_PAE_OrderAmount").Enabled = false;

            //this.Model.SetValue("F_PAE_OrderAmount", 100);

            
        }

       
    }
}

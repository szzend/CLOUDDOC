using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.SqlBuilder;
using Kingdee.BOS.Orm.DataEntity;
using Kingdee.BOS.ServiceHelper;
using Kingdee.BOS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Zy.KDTest.PlugIn
{
    [HotUpdate]
    [Description("获取单据数据的常用方法")]
    public class DemoZy008:AbstractDynamicFormPlugIn
    {
        public override void AfterBindData(EventArgs e)
        {
            base.AfterBindData(e);

            ReadData();

        }

        private void ReadData()
        {
            var formMetaData = FormMetaDataCache.GetCachedFormMetaData(this.Context, "PAE_ZyDemoBase3");
            var dynamicObjType = formMetaData.BusinessInfo.GetDynamicObjectType();
            string result = "";

            //根据内码来查询单据数据
            //DynamicObject dynamicObj= BusinessDataServiceHelper.LoadSingle(this.Context, "100001", dynamicObjType);
            //if (dynamicObj != null)
            //{
            //    this.View.ShowMessage(dynamicObj["BILLNO"].ToString());
            //}

            //根据内码查询多条数据
            //DynamicObject[] dynamicObjs = BusinessDataServiceHelper.Load(this.Context, new object[] { "100001","100002" }, dynamicObjType);
            //foreach (var obj in dynamicObjs)
            //{
            //    result += obj["BILLNO"].ToString() + ",";
            //}
            //this.View.ShowMessage(result);

            //根据自定义的查询条件来查询单据数据

            //QueryBuilderParemeter query = new QueryBuilderParemeter();
            //query.FormId = "PAE_ZyDemoBase3";
            //query.FilterClauseWihtKey = "FBillNo='F001'";
            //query.BusinessInfo = formMetaData.BusinessInfo;
            ////query.SelectItems.Add(new Kingdee.BOS.Core.Metadata.SelectorItemInfo("BillNo"));
            //DynamicObject[] dynamicObjs = BusinessDataServiceHelper.Load(this.Context, dynamicObjType, query);
            //foreach (var obj in dynamicObjs)
            //{
            //    result += obj["BILLNO"].ToString() + ",";
            //}
            //this.View.ShowMessage(result);

            //自定义查询条件以及查询列
            //List<SelectorItemInfo> selectItems = new List<SelectorItemInfo>();
            //selectItems.Add(new SelectorItemInfo("FBillNo"));
            //selectItems.Add(new SelectorItemInfo("F_PAE_Base"));
            //OQLFilter oFilter = new OQLFilter();
            //oFilter.Add(new OQLFilterHeadEntityItem() { FilterString = "FBillNo='F001'" });
            //oFilter.Add(new OQLFilterHeadEntityItem() { EntityKey="FEntity" ,FilterString="F_PAE_Base=110958" });
            //oFilter.Add(new OQLFilterHeadEntityItem() { EntityKey="FEntity" ,FilterString="F_PAE_Base.FNumber='CH4441'" });
            //DynamicObject[] dynamicObjs = BusinessDataServiceHelper.Load(this.Context,"PAE_ZyDemoBase3", selectItems, oFilter);
            //foreach (var obj in dynamicObjs)
            //{
            //    result += obj["BILLNO"].ToString() + ",";
            //}
            //this.View.ShowMessage(result);

        }

    }
}

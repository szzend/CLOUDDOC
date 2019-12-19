using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.Core.Metadata.FieldElement;
using Kingdee.BOS.Core.Report.PivotReport;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;
using System.Data;

namespace Kingdee.Samples.Report.Cross
{
    [Description("部门物料采购透视表")]
    [HotUpdate]
    public class CrossReport : SysReportBaseService
    {
        
        public override void Initialize()
        {
            base.ReportProperty.IsGroupSummary = true;
            InitSettingInfo();
            base.Initialize();
        }
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {   
            InsertTempTable(Context, tableName);            
        }

        private void InitSettingInfo()
        {
            SettingInfo = new PivotReportSettingInfo();
            SettingField supplier = PivotReportSettingInfo.CreateColumnSettingField(new TextField()
            {
                Key = "FSupplier",
                FieldName = "FSupplier",
                Name = new LocaleValue("供应商")
            }, 0);

            SettingInfo.ColTitleFields.Add(supplier);
            SettingInfo.SelectedFields.Add(supplier);

            SettingField department = PivotReportSettingInfo.CreateColumnSettingField(new TextField()
            {
                Key = "FDepartment",
                FieldName = "FDepartment",
                Name = new LocaleValue("部门")
            }, 0);

            SettingInfo.RowTitleFields.Add(department);
            SettingInfo.SelectedFields.Add(department);

            SettingField material = PivotReportSettingInfo.CreateColumnSettingField(new TextField()
            {
                Key = "FMaterial",
                FieldName = "FMaterial",
                Name = new LocaleValue("物料")
            }, 1);

            SettingInfo.RowTitleFields.Add(material);
            SettingInfo.SelectedFields.Add(material);


            SettingField qty = PivotReportSettingInfo.CreateColumnSettingField(new DecimalField()
            {
                Key = "FQty",
                FieldName = "FQty",
                Name = new LocaleValue("数量"),
            }, 0);

            qty.SumType = 1;
            SettingInfo.AggregateFields.Add(qty);
            SettingInfo.SelectedFields.Add(qty);

            SettingField amount = PivotReportSettingInfo.CreateColumnSettingField(new DecimalField()
            {
                Key = "FAmount",
                FieldName = "FAmount",
                Name = new LocaleValue("金额")
            }, 1);

            amount.SumType = 1;
            SettingInfo.AggregateFields.Add(amount);
            SettingInfo.SelectedFields.Add(amount);
        }

        private void InsertTempTable(Context ctx, string tempTable)
        {
            KSQL_SEQ = string.Format(KSQL_SEQ, "t.FDepartment,t.FMaterial");
            var sql = string.Format(@"
SELECT 
t.*,
{0}
INTO {1}
FROM(
        SELECT dl.FNAME AS FDepartment,ml.FNAME AS FMaterial,sl.FNAME AS FSupplier,SUM(poe.FQTY) AS FQty,SUM(poef.FAMOUNT) AS FAmount
        FROM T_BD_DEPARTMENT d
        INNER JOIN T_BD_DEPARTMENT_L dl ON (d.FDEPTID=dl.FDEPTID AND dl.FLOCALEID=2052)
        LEFT JOIN T_PUR_POORDER po ON (d.FDEPTID=po.FPURCHASEDEPTID)
        LEFT JOIN T_PUR_POORDERENTRY poe ON (po.FID=poe.FID)
        LEFT JOIN T_BD_MATERIAL_L ml ON (poe.FMATERIALID=ml.FMATERIALID)
        LEFT JOIN T_BD_SUPPLIER_L sl ON (po.FSUPPLIERID=sl.FSUPPLIERID)
        LEFT JOIN T_PUR_POORDERENTRY_F poef ON (poe.FENTRYID=poef.FENTRYID)
        GROUP BY dl.FNAME,ml.FNAME,sl.FNAME
    )t", KSQL_SEQ, tempTable);

            DBUtils.Execute(Context, sql);
        }              
    }
}

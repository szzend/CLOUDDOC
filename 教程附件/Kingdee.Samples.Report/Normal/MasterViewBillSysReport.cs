﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;

namespace Kingdee.Samples.Report.Normal
{
    [Description("主账表-查看单据")]
    [HotUpdate]
    public class MasterViewBillSysReport : SysReportBaseService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // 简单账表类型：基本简单账表
            ReportProperty.ReportType = ReportType.REPORTTYPE_NORMAL;       
            // 标识报表的列通过插件定义
            ReportProperty.IsUIDesignerColumns = false;

            //联查单据内码的字段
            ReportProperty.BillKeyFieldName = "FID";           
            //联查单据的表单标识
            ReportProperty.FormIdFieldName = "FFORMID";
        }

        /// <summary>
        /// 向报表临时表，插入报表数据
        /// </summary>
        /// <param name="filter">过滤信息</param>
        /// <param name="tableName">临时表名</param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            base.BuilderReportSqlAndTempTable(filter, tableName);
            KSQL_SEQ = string.Format(KSQL_SEQ, "t0.FBILLNO");
            string sql = string.Format(
                @"
SELECT  t0.FID ,       
        t0.FBILLNO ,
        t0.FDATE ,
        'PUR_PurchaseOrder' AS FFORMID,
        {0}
INTO {1}
FROM    T_PUR_POORDER t0       
WHERE   1 = 1 ", KSQL_SEQ, tableName);

            DBUtils.Execute(Context, sql);
        }

        /// <summary>
        /// 设置报表列
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportHeader GetReportHeaders(IRptParams filter)
        {          
            ReportHeader header = new ReportHeader();            
            var billNo = header.AddChild("FBillNo", new LocaleValue("单据编号"));
            billNo.ColIndex = 0;
            var date = header.AddChild("FDATE", new LocaleValue("日期"));
            date.ColIndex = 1;           
            return header;
        } 
    }
}

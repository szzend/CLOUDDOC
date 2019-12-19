using System;
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
    [Description("明细账表")]
    [HotUpdate]
    public class DetailSysReport : SysReportBaseService
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
        (CASE WHEN t0.FDOCUMENTSTATUS='A' THEN '创建' 
             WHEN t0.FDOCUMENTSTATUS='B' THEN '审核中' 
             WHEN t0.FDOCUMENTSTATUS='C' THEN '已审核' 
             WHEN t0.FDOCUMENTSTATUS='D' THEN '重新审核' 
             ELSE t0.FDOCUMENTSTATUS END) AS FDOCUMENTSTATUS,
        {0}
INTO {1}
FROM    T_PUR_POORDER t0       
WHERE   1 = 1 ", KSQL_SEQ, tableName);

            if (!filter.IsRefresh)
            {
                var dicFilter = filter.GetParentReportCurrentRow();
                if (dicFilter != null && dicFilter.ContainsKey("FBILLNO"))
                {
                    var billNo = Convert.ToString(dicFilter["FBILLNO"]);
                    if (billNo.IsNullOrEmptyOrWhiteSpace() == false)
                    {
                        sql = sql + string.Format(" AND t0.FBILLNO='{0}'", billNo);
                    }
                }
            }

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
            var date = header.AddChild("FDOCUMENTSTATUS", new LocaleValue("单据状态"));
            date.ColIndex = 1;           
            return header;
        } 
    }
}

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
    [Description("基本简单账表-UI列")]
    [HotUpdate]
    public class SysReportUIColumns : SysReportBaseService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // 简单账表类型：基本简单账表
            ReportProperty.ReportType = ReportType.REPORTTYPE_NORMAL;
            // 标识报表支持分组汇总
            ReportProperty.IsGroupSummary = true;
            // 标识报表的列通过插件定义
            ReportProperty.IsUIDesignerColumns = true;
        }

        /// <summary>
        /// 向报表临时表，插入报表数据
        /// </summary>
        /// <param name="filter">过滤信息</param>
        /// <param name="tableName">临时表名</param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            base.BuilderReportSqlAndTempTable(filter, tableName);

            //默认排序字段：需要从filter中取用户设置的排序字段
            //KSQL_SEQ: ROW_NUMBER() OVER(ORDER BY  {0} ) FIDENTITYID
            if (filter.FilterParameter.SortString.IsNullOrEmptyOrWhiteSpace())
            {
                KSQL_SEQ = string.Format(KSQL_SEQ, " t1.FMATERIALID asc");
            }
            else
            {

                KSQL_SEQ = string.Format(KSQL_SEQ, filter.FilterParameter.SortString);
            }

            string sql = string.Format(
                @"
SELECT  t0.FID ,
        t1.FENTRYID ,
        t1.FMATERIALID ,
        t1M_L.FNAME AS FMATERIALNAME ,
        t1.FQTY ,        
        t1U_L.FNAME AS FUNITNAME ,
        t1f.FTAXPRICE ,
        t1f.FALLAMOUNT ,     
        t1f.FTAXRATE ,
        t0.FBILLNO ,
        t0.FDATE ,
        t0.FDOCUMENTSTATUS , 
        {0}
INTO {1}
FROM    T_PUR_POORDER t0
        INNER JOIN T_PUR_POORDERENTRY t1 ON ( t0.FID = t1.FID )      
        INNER JOIN T_PUR_POORDERENTRY_F t1F ON ( t1.FENTRYID = t1f.FENTRYID )
        LEFT JOIN T_BD_MATERIAL_L t1M_L on (t1.FMATERIALID = t1m_l.FMATERIALID and t1M_L.FLOCALEID = 2052)
        LEFT JOIN T_BD_UNIT t1U ON ( t1f.FPRICEUNITID = t1u.FUNITID )
        LEFT JOIN T_BD_UNIT_L t1U_L ON ( t1U.FUNITID = t1U_L.FUNITID AND t1U_L.FLOCALEID = 2052 )
WHERE   1 = 1 ", KSQL_SEQ, tableName);

            //添加快捷过滤
            if (filter.FilterParameter.CustomFilter != null 
                && filter.FilterParameter.CustomFilter.Contains("FMATERIALID"))
            {
                var material = filter.FilterParameter.CustomFilter["FMATERIALID"] as DynamicObject;
                if (material!= null)
                {
                    var quickStr = string.Format("t1.FMATERIALID='{0}'", material[0]);
                    sql += string.Format("AND {0}", quickStr);
                }
            }

            //添加条件过滤
            string conditionStr = filter.FilterParameter.FilterString;
            if (!conditionStr.IsNullOrEmptyOrWhiteSpace())
            {
                sql += string.Format("AND {0}", conditionStr);
            }

            DBUtils.Execute(Context, sql);
        }        

        /// <summary>
        /// 设置报表标题
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportTitles GetReportTitles(IRptParams filter)
        {
            var result = base.GetReportTitles(filter);
            if (result == null)
            {
                result = new ReportTitles();
            }

            if (filter.FilterParameter.CustomFilter != null
                && filter.FilterParameter.CustomFilter.Contains("FMATERIALID"))
            {
                var material = filter.FilterParameter.CustomFilter["FMATERIALID"] as DynamicObject;
                if (material != null && material.Contains("NAME"))
                {
                    result.AddTitle("FMATERIAL", Convert.ToString(material["Name"]));
                }
                else
                {
                    result.AddTitle("FMATERIAL", "");
                }
            }
            
            return result;
        }

        /// <summary>
        /// 设置报表合计列
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override List<SummaryField> GetSummaryColumnInfo(IRptParams filter)
        {
            var result = base.GetSummaryColumnInfo(filter);
            result.Add(new SummaryField("FQTY", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            result.Add(new SummaryField("FALLAMOUNT", Kingdee.BOS.Core.Enums.BOSEnums.Enu_SummaryType.SUM));
            return result;
        }
    }
}

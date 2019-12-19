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
    [Description("基本简单账表,包含过滤页面信息")]
    [HotUpdate]
    public class SysReport : SysReportBaseService
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
            ReportProperty.IsUIDesignerColumns = false;            
            //列显示内容代替列定义
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FMATERIALID", "FMATERIALNAME");
            // 设置精度控制
            List<DecimalControlField> list = new List<DecimalControlField>();           
            list.Add(new DecimalControlField("FUNITPRECISION", "FQTY"));  // 数量            
            list.Add(new DecimalControlField("FPRICEDIGITS", "FTAXPRICE"));// 单价            
            list.Add(new DecimalControlField("FAMOUNTDIGITS", "FALLAMOUNT")); // 金额            
            ReportProperty.DecimalControlFieldList = list;
            //标识报表是否调用BuilderReportSqlAndTempTable接口创建临时表
            IsCreateTempTableByPlugin = true;
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
        t1m_l.FNAME AS FMATERIALNAME ,
        t1.FQTY ,  
        t1u.FPRECISION AS FUNITPRECISION ,
        t1u_l.FNAME AS FUNITNAME ,
        t1f.FTAXPRICE ,
        ISNULL(t20.FPRICEDIGITS,4) AS FPRICEDIGITS ,
        t1f.FALLAMOUNT ,     
        ISNULL(t20.FAMOUNTDIGITS,2) AS FAMOUNTDIGITS ,
        t1f.FTAXRATE ,
        t0.FBILLNO ,
        t0.FDATE ,
        {0}
INTO {1}
FROM    T_PUR_POORDER t0
        INNER JOIN T_PUR_POORDERENTRY t1 ON ( t0.FID = t1.FID )      
        INNER JOIN T_PUR_POORDERENTRY_F t1f ON ( t1.FENTRYID = t1f.FENTRYID )
        INNER JOIN T_PUR_POORDERFIN t2 on (t0.FID = t2.FID)
        LEFT JOIN T_BD_MATERIAL_L t1m_l ON (t1.FMATERIALID = t1m_l.FMATERIALID and t1m_l.FLOCALEID = 2052)
        LEFT JOIN T_BD_UNIT t1u ON ( t1f.FPRICEUNITID = t1u.FUNITID )
        LEFT JOIN T_BD_UNIT_L t1u_l ON ( t1u.FUNITID = t1u_l.FUNITID AND t1u_l.FLOCALEID = 2052 )        
        LEFT JOIN T_BD_CURRENCY t20 ON (t2.FLOCALCURRID = t20.FCURRENCYID)  
WHERE   1 = 1 ", KSQL_SEQ, tableName);

            //添加快捷过滤
            if (filter.FilterParameter.CustomFilter != null 
                && filter.FilterParameter.CustomFilter.Contains("FMATERIAL"))
            {
                var material = filter.FilterParameter.CustomFilter["FMATERIAL"] as DynamicObject;
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

        #region IsCreateTempTableByPlugin为false时调用
        protected override string BuilderSelectFieldSQL(IRptParams filter)
        {
            return @"
SELECT  t0.FID ,
        t1.FENTRYID ,
        t1.FMATERIALID ,
        t1m_l.FNAME AS FMATERIALNAME ,
        t1.FQTY ,  
        t1u.FPRECISION AS FUNITPRECISION ,
        t1u_l.FNAME AS FUNITNAME ,
        t1f.FTAXPRICE ,
        ISNULL(t20.FPRICEDIGITS,4) AS FPRICEDIGITS ,
        t1f.FALLAMOUNT ,     
        ISNULL(t20.FAMOUNTDIGITS,2) AS FAMOUNTDIGITS ,
        t1f.FTAXRATE ,
        t0.FBILLNO ,
        t0.FDATE";
        }

        protected override string BuilderFromWhereSQL(IRptParams filter)
        {
            var sql = @"
FROM    T_PUR_POORDER t0
        INNER JOIN T_PUR_POORDERENTRY t1 ON ( t0.FID = t1.FID )      
        INNER JOIN T_PUR_POORDERENTRY_F t1f ON ( t1.FENTRYID = t1f.FENTRYID )
        INNER JOIN T_PUR_POORDERFIN t2 on (t0.FID = t2.FID)
        LEFT JOIN T_BD_MATERIAL_L t1m_l ON (t1.FMATERIALID = t1m_l.FMATERIALID and t1m_l.FLOCALEID = 2052)
        LEFT JOIN T_BD_UNIT t1u ON ( t1f.FPRICEUNITID = t1u.FUNITID )
        LEFT JOIN T_BD_UNIT_L t1u_l ON ( t1u.FUNITID = t1u_l.FUNITID AND t1u_l.FLOCALEID = 2052 )        
        LEFT JOIN T_BD_CURRENCY t20 ON (t2.FLOCALCURRID = t20.FCURRENCYID)  
WHERE   1 = 1 ";
            //添加快捷过滤
            if (filter.FilterParameter.CustomFilter != null
                && filter.FilterParameter.CustomFilter.Contains("FMATERIAL"))
            {
                var material = filter.FilterParameter.CustomFilter["FMATERIAL"] as DynamicObject;
                if (material != null)
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

            return sql;
        }

        protected override string BuilderTempTableOrderBySQL(IRptParams filter)
        {            
            return "t1.FMATERIALID asc";
        }

        #endregion

        /// <summary>
        /// 设置报表列
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportHeader GetReportHeaders(IRptParams filter)
        {          
            ReportHeader header = new ReportHeader();
            var material = header.AddChild("FMATERIALID", new LocaleValue("物料"));
            material.ColIndex = 0;
            var qty = header.AddChild("FQTY", new LocaleValue("数量"), SqlStorageType.SqlDecimal);
            qty.ColIndex = 1;
            var unit = header.AddChild("FUNITNAME", new LocaleValue("单位"));
            unit.ColIndex = 2;
            var price = header.AddChild("FTAXPRICE", new LocaleValue("含税价"), SqlStorageType.SqlDecimal);
            price.ColIndex = 3;
            var amount = header.AddChild("FALLAMOUNT", new LocaleValue("价税合计"), SqlStorageType.SqlDecimal);
            amount.ColIndex = 4;
            var taxRate = header.AddChild("FTAXRATE", new LocaleValue("税率"), SqlStorageType.SqlDecimal);
            taxRate.ColIndex = 5;
            var billNo = header.AddChild("FBillNo", new LocaleValue("单据编号"));
            billNo.ColIndex = 6;
            var date = header.AddChild("FDATE", new LocaleValue("日期"));
            date.ColIndex = 7;           
            return header;
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
                && filter.FilterParameter.CustomFilter.Contains("FMATERIAL"))
            {
                var material = filter.FilterParameter.CustomFilter["FMATERIAL"] as DynamicObject;
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

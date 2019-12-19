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
using System.Data;

namespace Kingdee.Samples.Report.Move
{
    [Description("分页账表")]
    [HotUpdate]
    public class MoveReport : SysReportBaseService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // 简单账表类型：基本简单账表
            ReportProperty.ReportType = ReportType.REPORTTYPE_MOVE;
            // 标识报表支持分组汇总
            ReportProperty.IsGroupSummary = true;
            // 标识报表的列通过插件定义
            ReportProperty.IsUIDesignerColumns = false;
            //列显示内容代替列定义
            ReportProperty.DspInsteadColumnsInfo.DefaultDspInsteadColumns.Add("FMATERIALID", "FMATERIALNAME");
            
        }

        /// <summary>
        /// 向报表临时表，插入报表数据
        /// </summary>
        /// <param name="filter">过滤信息</param>
        /// <param name="tableName">临时表名</param>
        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            base.BuilderReportSqlAndTempTable(filter, tableName);

            KSQL_SEQ = string.Format(KSQL_SEQ, " t1.FMATERIALID asc");

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
        sl.FNAME AS FSUPPLIERNAME , 
        {0}
INTO {1}
FROM    T_PUR_POORDER t0
        INNER JOIN T_PUR_POORDERENTRY t1 ON ( t0.FID = t1.FID )      
        INNER JOIN T_PUR_POORDERENTRY_F t1F ON ( t1.FENTRYID = t1f.FENTRYID )
        LEFT JOIN T_BD_MATERIAL_L t1M_L on (t1.FMATERIALID = t1m_l.FMATERIALID and t1M_L.FLOCALEID = 2052)
        LEFT JOIN T_BD_UNIT t1U ON ( t1f.FPRICEUNITID = t1u.FUNITID )
        LEFT JOIN T_BD_UNIT_L t1U_L ON ( t1U.FUNITID = t1U_L.FUNITID AND t1U_L.FLOCALEID = 2052 )
        LEFT JOIN T_BD_SUPPLIER_L sl ON (t0.FSUPPLIERID=sl.FSUPPLIERID AND sl.FLOCALEID = 2052 )
WHERE   1 = 1 ", KSQL_SEQ, tableName);

            if (CacheDataList != null && CacheDataList.ContainsKey(filter.CurrentPosition))
            {
                var dr = CacheDataList[filter.CurrentPosition];
                if (dr != null)
                {
                    sql = sql + string.Format(" AND t0.FSUPPLIERID={0}", Convert.ToInt32(dr["FSUPPLIERID"]));
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
            var supply = header.AddChild("FSUPPLIERNAME", new LocaleValue("供应商"));
            supply.ColIndex = 8;
            return header;
        }

        public override DataTable GetList(IRptParams filter)
        {           
            var dt = new DataTable();
            dt.Columns.Add("FSUPPLIERID");
            dt.Columns.Add("FSUPPLIERNAME");

            var sql = "SELECT FSUPPLIERID,FNAME AS FSUPPLIERNAME FROM T_BD_SUPPLIER_L WHERE FLOCALEID=2052 ORDER BY FSUPPLIERID";
            using (var rd = DBUtils.ExecuteReader(Context, sql))
            {
                while (rd.Read())
                {
                    var row = dt.NewRow();
                    row["FSUPPLIERID"] = rd["FSUPPLIERID"];
                    row["FSUPPLIERNAME"] = rd["FSUPPLIERNAME"];
                    dt.Rows.Add(row);
                }

                rd.Close();
            }

            return dt;
        }

        /// <summary>
        /// 设置报表标题
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override ReportTitles GetReportTitles(IRptParams filter)
        {
            var titles = base.GetReportTitles(filter);
            if (titles == null)
            {
                titles = new ReportTitles();
            }

            if (CacheDataList != null && CacheDataList.ContainsKey(filter.CurrentPosition))
            {
                var dr = CacheDataList[filter.CurrentPosition];
                if (dr != null)
                {
                    titles.AddTitle("FSupplier", Convert.ToString(dr["FSUPPLIERNAME"]));
                }
            }

            return titles;
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

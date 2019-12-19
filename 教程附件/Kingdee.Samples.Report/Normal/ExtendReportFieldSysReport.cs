using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Kingdee.BOS;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.Contracts;
using Kingdee.BOS.Contracts.Report;
using Kingdee.BOS.App.Data;
using Kingdee.BOS.Orm.DataEntity;

namespace Kingdee.Samples.Report.Normal
{
    [Description("报表扩展字段")]
    [HotUpdate]
    public class ExtendReportFieldSysReport : SysReport
    {
        //临时表名
        private string tmpTbName = string.Empty;

        public override void BuilderReportSqlAndTempTable(IRptParams filter, string tableName)
        {
            //创建临时表用于存放基类产生的数据
            IDBService dbSrv = ServiceFactory.GetDBService(Context);
            tmpTbName = dbSrv.CreateTemporaryTableName(Context);
            //调用基类方法并将数据存入临时表
            base.BuilderReportSqlAndTempTable(filter, tmpTbName);

            var sql = string.Format(@"
SELECT t.*, cl.FName AS FCurrencyName INTO {0}
FROM {1} t 
INNER JOIN T_PUR_POORDERFIN t2 on (t.FID = t2.FID)
LEFT JOIN T_BD_CURRENCY_L cl on t2.FLOCALCURRID=cl.FCURRENCYID and cl.FLOCALEID=2052", tableName, tmpTbName);

            DBUtils.Execute(Context, sql);
        }

        public override void CloseReport()
        {
            base.CloseReport();
            if (tmpTbName.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }
            //删除自定义临时表
            IDBService dbSrv = ServiceFactory.GetDBService(Context);
            dbSrv.DeleteTemporaryTableName(Context, new string[] { tmpTbName });
        }

        public override ReportHeader GetReportHeaders(IRptParams filter)
        {
            var header = base.GetReportHeaders(filter);
            var currency = header.AddChild("FCurrencyName", new LocaleValue("币别"), SqlStorageType.Sqlnvarchar);
            currency.ColIndex = 10;
            return header;
        }
    }
}

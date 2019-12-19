using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using Kingdee.BOS.Util;
using Kingdee.BOS.Core.Report;
using Kingdee.BOS.Core.Report.PlugIn;
using Kingdee.BOS.Core.Report.PlugIn.Args;
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;

namespace Kingdee.Samples.Report.Normal
{
    [Description("查看单据表单插件")]
    [HotUpdate]
    public class ShowBillEdit : AbstractSysReportPlugIn
    {
        /// <summary>
        /// 方案一，用户双击单元格：读取焦点单元格记录的单据内码，打开对应单据
        /// </summary>
        /// <param name="e"></param>
        public override void CellDbClick(CellEventArgs e)
        {
            base.CellDbClick(e);
            e.Cancel = true;

            if (e.Header.FieldName.EqualsIgnoreCase("FMATERIAL"))
            {
                var materialId = GetValue(e.CellRowIndex, "FMATERIAL");
                if (materialId.IsNullOrEmptyOrWhiteSpace() == false)
                {
                    ShowForm("BD_MATERIAL", materialId);
                }
            }
        }

        /// <summary>
        /// 方案二，用户点击带链接的单元格：读取此单元格记录的单据编号，打开对应单据
        /// </summary>
        /// <param name="e"></param>
        public override void EntryButtonCellClick(EntryButtonCellClickEventArgs e)
        {      
            if (e.FieldKey.EqualsIgnoreCase("FID"))
            {
                var billFID = GetValue(e.Row - 1, "FID");
                if (billFID.IsNullOrEmptyOrWhiteSpace() == false)
                {
                    ShowForm("PUR_PurchaseOrder", billFID);
                }
            }            
        }

        private string GetValue(int row, string fieldKey)
        {           
            DataTable dt = ((ISysReportModel)this.View.Model).DataSource;
            if (row >= 0 && row <= dt.Rows.Count)
            {               
                return Convert.ToString(dt.Rows[row][fieldKey]);              
            }

            return string.Empty;
        }

        /// <summary>
        /// 打开单据界面，显示指定的单据
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="pkValue"></param>
        private void ShowForm(string formId, string pkValue)
        {          
            BillShowParameter showParameter = new BillShowParameter();
            showParameter.FormId = formId;
            showParameter.OpenStyle.ShowType = ShowType.Floating;   
            showParameter.Status = OperationStatus.EDIT;
            showParameter.PKey = pkValue;
            View.ShowForm(showParameter);
        }
    }
}

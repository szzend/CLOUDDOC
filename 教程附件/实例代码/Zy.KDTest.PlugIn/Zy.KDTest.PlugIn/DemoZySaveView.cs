using Kingdee.BOS.Core;
using Kingdee.BOS.Core.Bill;
using Kingdee.BOS.Core.DynamicForm;
using Kingdee.BOS.Core.DynamicForm.PlugIn;
using Kingdee.BOS.Core.DynamicForm.PlugIn.Args;
using Kingdee.BOS.Core.Interaction;
using Kingdee.BOS.Core.Metadata;
using Kingdee.BOS.Core.Metadata.FormElement;
using Kingdee.BOS.Core.SqlBuilder;
using Kingdee.BOS.Orm;
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
    [Description("插件创建单据并保存的实现")]
    public class DemoZySaveView:AbstractDynamicFormPlugIn
    {

        public override void ButtonClick(ButtonClickEventArgs e)
        {
            base.ButtonClick(e);

            if (e.Key.EqualsIgnoreCase("F_PAE_SaveView"))
            {
                SaveBillInfo();
            }
        }


        private void SaveBillInfo()
        {
             IBillView billView = this.CreateBillView();
            ((IBillViewService)billView).LoadData();
            this.FillFormPropertys(billView);
            OperateOption saveOption = OperateOption.Create();
            this.SaveFormData(billView, saveOption);
        }

        private IBillView CreateBillView()
        {
             //读取单据的元数据
            FormMetadata meta = MetaDataServiceHelper.Load(this.Context, "PAE_ZyDemoBase3") as FormMetadata;
            Form form = meta.BusinessInfo.GetForm();
            // 创建用于引入数据的单据view
            Type type = Type.GetType("Kingdee.BOS.Web.Import.ImportBillView,Kingdee.BOS.Web");
            var billView = (IDynamicFormViewService)Activator.CreateInstance(type);
            // 开始初始化billView：
            // 创建视图加载参数对象，指定各种参数，如FormId, 视图(LayoutId)等
            BillOpenParameter openParam = CreateOpenParameter(meta);
            // 动态领域模型服务提供类，通过此类，构建MVC实例
            var provider = form.GetFormServiceProvider();
            billView.Initialize(openParam, provider);
            return billView as IBillView;
        }

        private BillOpenParameter CreateOpenParameter(FormMetadata meta)
        {
            Form form = meta.BusinessInfo.GetForm();
            // 指定FormId, LayoutId
            BillOpenParameter openParam = new BillOpenParameter(form.Id, meta.GetLayoutInfo().Id);
            // 数据库上下文
            openParam.Context = this.Context;
            // 本单据模型使用的MVC框架
            openParam.ServiceName = form.FormServiceName;
            // 随机产生一个不重复的PageId，作为视图的标识
            openParam.PageId = Guid.NewGuid().ToString();
            // 元数据
            openParam.FormMetaData = meta;
            // 界面状态：新增 (修改、查看)
            openParam.Status = OperationStatus.ADDNEW;
            // 单据主键：本案例演示新建物料，不需要设置主键
            openParam.PkValue = null;
            // 界面创建目的：普通无特殊目的 （为工作流、为下推、为复制等）
            openParam.CreateFrom = CreateFrom.Default;
            // 基础资料分组维度：基础资料允许添加多个分组字段，每个分组字段会有一个分组维度
            // 具体分组维度Id，请参阅 form.FormGroups 属性
            openParam.GroupId = "";
            // 基础资料分组：如果需要为新建的基础资料指定所在分组，请设置此属性
            openParam.ParentId = 0;
            // 单据类型
            openParam.DefaultBillTypeId = "";
            // 业务流程
            openParam.DefaultBusinessFlowId = "";
            // 主业务组织改变时，不用弹出提示界面
            openParam.SetCustomParameter("ShowConfirmDialogWhenChangeOrg", false);
            // 插件
            List<AbstractDynamicFormPlugIn> plugs = form.CreateFormPlugIns();
            openParam.SetCustomParameter(FormConst.PlugIns, plugs);
            PreOpenFormEventArgs args = new PreOpenFormEventArgs(this.Context, openParam);
            foreach (var plug in plugs)
            {// 触发插件PreOpenForm事件，供插件确认是否允许打开界面
                plug.PreOpenForm(args);
            }
            if (args.Cancel == true)
            {// 插件不允许打开界面
                // 本案例不理会插件的诉求，继续....
            }
            // 返回
            return openParam;
        }

        private void SaveFormData(IBillView billView, OperateOption saveOption)
        {
            // 调用保存操作
            IOperationResult saveResult = BusinessDataServiceHelper.Save(
                        this.Context,
                        billView.BillBusinessInfo,
                        billView.Model.DataObject,
                        saveOption,
                        "Save");
            // 显示处理结果
            if (saveResult == null)
            {
                this.View.ShowErrMessage("未知原因导致保存失败！");
                return;
            }
            else if (saveResult.IsSuccess == true)
            {// 保存成功，直接显示
                this.View.ShowOperateResult(saveResult.OperateResult);
                return;
            }
            else if (saveResult.InteractionContext != null
                    && saveResult.InteractionContext.Option.GetInteractionFlag().Count > 0)
            {// 保存失败，需要用户确认问题
                InteractionUtil.DoInteraction(this.View, saveResult, saveOption,
                    new Action<FormResult, IInteractionResult, OperateOption>((
                        formResult, opResult, option) =>
                    {
                        // 用户确认完毕，重新调用保存处理
                        this.SaveFormData(billView, option);
                    }));
            }
            // 保存失败，显示错误信息
            if (saveResult.IsShowMessage)
            {
                saveResult.MergeValidateErrors();
                this.View.ShowOperateResult(saveResult.OperateResult);
                return;
            }
        }

        private void FillFormPropertys(IBillView billView)
        {

            billView.Model.SetValue("FBillNo", DateTime.Now.ToString("yyyyMMddHHmmssss"), 0);
            billView.Model.SetValue("F_PAE_Text", "普通", 0);
            billView.Model.SetItemValueByNumber("F_PAE_Base1", "PRE001", 0);
            billView.Model.SetItemValueByNumber("F_PAE_Base", "CH4441", 0);
            billView.Model.SetValue("F_PAE_Text1", "普通", 0);
        }

    }
}

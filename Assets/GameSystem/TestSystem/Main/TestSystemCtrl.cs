using GameSystem.MVCTemplate;
using Tool.UI;
using UnityEngine;

namespace GameSystem.TestSystem.Main
{
    public class TestSystemCtrl : BaseCtrl
    {
        public TestSystemCtrl() : base(new TestSystemModel(),
            UIManager.GetInstance().OpenPanelSync("TestSystemView", EuiLayer.Mid).GetComponent<BaseView>(),
            EuiLayer.Mid)
        {
        }

        /// <summary>
        /// 处理部分view的业务
        /// </summary>
        protected override void InitListener()
        {
            Debug.Log("InitListener");
        }

        /// <summary>
        /// 展示主要view
        /// </summary>
        public void OnShowView() => OnOpen("TestSystemView", EuiLayer.Mid);

        /// <summary>
        /// 关闭视图
        /// </summary>
        public void OnHideView() => OnClose();

        /// <summary>
        /// view展示完毕回调函数
        /// </summary>
        protected override void OnCompleteShow()
        {
            Debug.Log("展示view完毕");
        }

        /// <summary>
        /// view关闭完毕回调函数
        /// </summary>
        protected override void OnCompleteClose()
        {
            Debug.Log("关闭view完毕");
        }

        public TestSystemModel GetModel() => Model as TestSystemModel;
        public TestSystemView GetView() => View as TestSystemView;
    }
}
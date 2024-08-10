using GameSystem.MVCTemplate;
using Tool.UI;
using UnityEngine;

namespace GameSystem.CounterSystem.Main
{
    public class CounterSystemCtrl : BaseCtrl
    {
        public CounterSystemCtrl() : base(new CounterSystemModel(),
            UIManager.GetInstance().OpenPanelSync("CounterSystemView", EuiLayer.Mid).GetComponent<BaseView>(),
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
        public void OnShowView() => OnOpen("CounterSystemView", EuiLayer.Mid);

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

        public CounterSystemModel GetModel() => Model as CounterSystemModel;
        public CounterSystemView GetView() => View as CounterSystemView;
    }
}
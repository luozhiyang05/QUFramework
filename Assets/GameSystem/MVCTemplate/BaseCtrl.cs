using Framework;
using Tool.UI;

namespace GameSystem.MVCTemplate
{
    /// <summary>
    /// 功能：
    /// 连通model和view
    /// 将view层的某些业务放入ctrl
    /// 控制view的打开和关闭
    /// </summary>
    public abstract class BaseCtrl : ICanGetSystem
    {
        protected BaseModel Model;
        protected BaseView View;
        private EuiLayer _euiLayer;

        protected BaseCtrl(BaseModel module, BaseView view, EuiLayer euiLayer)
        {
            Model = module;
            View = view;
            View.SetModel(Model);
            _euiLayer = euiLayer;
            InitListener();
            OnCompleteShow();
        }

        protected abstract void InitListener();

        protected void OnOpen(string viewName, EuiLayer euiLayer)
        {
            View = UIManager.GetInstance().OpenPanelSync(viewName, euiLayer).GetComponent<BaseView>();
            OnCompleteShow();
        }

        protected void OnClose()
        {
            UIManager.GetInstance().ClosePanel(_euiLayer);
            OnCompleteClose();
        }

        protected abstract void OnCompleteShow();
        protected abstract void OnCompleteClose();

        public IMgr Ins => Global.GetInstance();
    }
}
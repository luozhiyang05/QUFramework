using Framework;
using GameSystem.TestSystem.Main;

namespace GameSystem.TestSystem
{
    public interface ITestSystemModule : IModule
    {
        public void ShowView();
        public void HideView();
    }

    public class TestSystemModule : AbsModule, ITestSystemModule
    {
        private TestSystemCtrl _ctrl;

        protected override void OnInit()
        {
        }

        public void ShowView()
        {
            if (_ctrl == null) _ctrl = new TestSystemCtrl();
            else _ctrl.OnShowView();
        }

        public void HideView()
        {
            _ctrl.OnHideView();
        }
    }
}
using Framework;
using GameSystem.CounterSystem.Main;

namespace GameSystem.CounterSystem
{
    public interface ICounterSystemModule : IModule
    {
        public void ShowView();
        public void HideView();
    }

    public class CounterSystemModule : AbsModule, ICounterSystemModule
    {
        private CounterSystemCtrl _ctrl;

        protected override void OnInit()
        {
        }

        public void ShowView()
        {
            if (_ctrl == null) _ctrl = new CounterSystemCtrl();
            else _ctrl.OnShowView();
        }

        public void HideView()
        {
            _ctrl.OnHideView();
        }
    }
}
using GameSystem.CounterSystem;
namespace Framework
{
    public class Global : FrameworkMgr<Global>
    {
        protected override void OnInitModule()
        {
			this.RegisterModule<ICounterSystemModule>(new CounterSystemModule());
        }
    }
}
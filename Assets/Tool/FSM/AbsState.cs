namespace Tool.FSM
{
    public abstract class AbsState : IState
    {
        protected FSM _fsm; 
        FSM IState.Fsm => _fsm;
        public float Time => _time;
        public float FrameCount => _frameCount;
        public float FixedFrameCount => _fixedFrameCount;
        
        private float _enterTime = 0;
        private float _time;
        private float _frameCount;
        private float _fixedFrameCount;
        

        public virtual void OnEnter(FSM fsm)
        {
            _fsm = fsm;
            _time = 0;
            _frameCount = 0;
            _fixedFrameCount = 0;
            _enterTime = UnityEngine.Time.time;
        }

        public virtual void OnUpdate()
        {
            _time = UnityEngine.Time.time - _enterTime;
            _frameCount++;
        }

        public virtual void OnFixedUpdate()
        {
            _fixedFrameCount++;
        }

        public virtual void OnExit()
        {
        }

        public abstract void CheckCondition();

    }
}
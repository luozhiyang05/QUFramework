namespace Tool.FSM
{
    public interface IState
    {
        public Tool.FSM.FSM Fsm { get; }
        public float Time { get; }
        public float FrameCount { get; }
        public float FixedFrameCount { get; }
        void OnEnter(Tool.FSM.FSM fsm);
        void OnUpdate();
        void OnFixedUpdate();
        void OnExit();
        void CheckCondition();
    }
}
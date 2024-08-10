using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tool.FSM
{
    public abstract class FSM : MonoBehaviour
    {
        protected IState CurrentState;
        protected IState LastState;
        public string currentStateName;
        public string lastStateName;
        private readonly Dictionary<Enum, IState> _statesDic = new Dictionary<Enum, IState>();
        
        protected void StartState(Enum stateEnum)
        {
            if (_statesDic.TryGetValue(stateEnum, out var value))
                SwitchState(stateEnum);
            else throw new Exception("找不到该状态脚本");
            lastStateName = stateEnum.ToString();
        }

        protected void AddState(Enum stateEnum, IState state) => _statesDic[stateEnum] = state;

        public void SwitchState(Enum stateEnum)
        {
            //执行状态退出逻辑
            CurrentState?.OnExit();
            //记录上一个状态信息
            LastState = CurrentState;
            CurrentState = _statesDic[stateEnum];
            //记录上一个状态名称
            lastStateName = currentStateName;
            currentStateName = stateEnum.ToString();
            //执行状态进入逻辑
            CurrentState?.OnEnter(this);
        }
    }
}
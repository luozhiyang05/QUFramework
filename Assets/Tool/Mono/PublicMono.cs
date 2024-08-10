using System;
using UnityEngine;

namespace Tool.Mono
{
    public class PublicMono : MonoBehaviour
    {
        private Action _updateEvents;
        private Action _fixedUpdateEvents;

        public void OnRegisterUpdate(Action action) => _updateEvents += action;
        public void OnRegisterFixedUpdate(Action action) => _fixedUpdateEvents += action;
        public void OnUnRegisterUpdate(Action action) => _updateEvents -= action;
        public void OnUnRegisterFixedUpdate(Action action) => _fixedUpdateEvents -= action;
        
        private void Update()
        {
            _updateEvents?.Invoke();
        }

        private void FixedUpdate()
        {
            _fixedUpdateEvents?.Invoke();
        }
    }
}
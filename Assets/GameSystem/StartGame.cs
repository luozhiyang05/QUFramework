using System;
using Framework;
using GameSystem.CounterSystem;
using UnityEngine;

namespace GameSystem
{
    public class StartGame : MonoBehaviour,ICanGetSystem
    {
        private void Start()
        {
           
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var testSystem = this.GetSystem<ICounterSystemModule>();
                testSystem.ShowView();
            }
        }

        public IMgr Ins => Global.GetInstance();
    }
}
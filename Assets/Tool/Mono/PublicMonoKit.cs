using System;
using Tool.Single;
using Tool.Utilities;
using UnityEngine;

namespace Tool.Mono
{
    public class PublicMonoKit : Singleton<PublicMonoKit>
    {
        private GameObject _publicMonoGo;
        private PublicMono _publicMono;

        protected override void OnInit()
        {
            //生成计时器
            _publicMonoGo = new GameObject("PublicMono", typeof(PublicMono));
            _publicMono = _publicMonoGo.GetComponent<PublicMono>();
            //销毁保护
            GameObject.DontDestroyOnLoad(_publicMonoGo);
        }

        public PublicMono GetPublicMono() => _publicMono;

        /// <summary>
        /// 添加Update事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        /// <param name="durationTime"></param>
        public void OnRegisterUpdate(Action action)
        {
            _publicMono.OnRegisterUpdate(action);
        }
        
        /// <summary>
        /// 添加FixedUpdate事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        /// <param name="durationTime"></param>
        public void OnRegisterFixedUpdate(Action action)
        {
            _publicMono.OnRegisterFixedUpdate(action);
        }
        

        /// <summary>
        /// 消除Update事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        public void OnUnRegisterUpdate(Action action)
        {
            _publicMono.OnUnRegisterUpdate(action);
        }
        
        /// <summary>
        /// 消除FixedUpdate事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        public void OnUnRegisterFixedUpdate(Action action)
        {
            _publicMono.OnUnRegisterFixedUpdate(action);
        }
        
        
    }
}
using System.Collections;
using Tool.Single;
using UnityEngine;

namespace Tool.Mono
{
     public class WaitSeconds
     {
         public float Duration;
         public WaitSeconds(float time) => Duration = time;
     }


    public class CoroutineKit : Singleton<CoroutineKit>
    {
        private CoroutineManager _coroutineManager;
        protected override void OnInit()
        {
            var gameObject = new GameObject("CoroutineManager");
            _coroutineManager = gameObject.AddComponent<CoroutineManager>();
            Object.DontDestroyOnLoad(gameObject);
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _coroutineManager.StartCoroutine(coroutine);
            Debug.LogWarning(coroutine.ToString());
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            _coroutineManager.StopCoroutine(coroutine);
        }
        
    }
}
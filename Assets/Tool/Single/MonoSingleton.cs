using UnityEngine;

namespace Tool.Single
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T GetInstance()
        {
            if (_instance != null) return _instance;
            var obj = new GameObject(typeof(T).ToString());
            _instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
            return _instance;
        }
    }
}
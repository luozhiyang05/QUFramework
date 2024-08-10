using System;

namespace Tool.Single
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        //懒汉式
        private static T _instance;

        public static T GetInstance()
        {
            if (_instance != null) return _instance;
            _instance = Activator.CreateInstance<T>();
            _instance.OnInit();
            return _instance;
        }
        protected abstract void OnInit();
    }
}
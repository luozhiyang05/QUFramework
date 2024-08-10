using System;
using System.Collections;
using Tool.Single;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tool.ResourceMgr
{
    public class ResMgr : MonoSingleton<ResMgr>
    {
        private IEnumerator IAsyncLoadRes<T>(string resName, Action<T> callBack,bool isInstantiate = true) where T : Object
        {
            var rq =  Resources.LoadAsync<T>(resName);
            while (!rq.isDone)   yield return null;
            
            //异步加载，直到加载完毕才调用回调方法
            callBack.Invoke((rq.asset is GameObject && isInstantiate) ? GameObject.Instantiate(rq.asset) as T : rq.asset as T);
        }

        /// <summary>
        /// 同步加载Resources下的资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="isInstantiate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SyncLoad<T>(string resName,bool isInstantiate = true) where T : Object
        {
            T load = Resources.Load<T>(resName);

            //判断是否属于unity的GameObject，如果属于则实例化一个GameObject，如果不属于则返回T
            return (load is GameObject && isInstantiate) ? GameObject.Instantiate(load) : load;
        }

        /// <summary>
        /// 异步加载Resources下的资源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callBack"></param>
        /// <param name="isInstantiate"></param>
        /// <typeparam name="T"></typeparam>
        public void AsyncLoad<T>(string name,Action<T> callBack,bool isInstantiate = true) where T : Object
        {
            StartCoroutine(IAsyncLoadRes<T>(name, callBack,isInstantiate));
        }

        /// <summary>
        /// 卸载Resources未占用的资源
        /// </summary>
        public static void Clear()
        {
            //卸载未占用的asset资源
            Resources.UnloadUnusedAssets();
            //GC回收
            GC.Collect();
        }
    }
}
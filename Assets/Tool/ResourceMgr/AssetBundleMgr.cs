using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tool.Single;
using UnityEngine;
using Object = UnityEngine.Object;

//连续异步加载会出现报错,因为同一个ab包不能被同时加载
namespace Tool.ResourceMgr
{
    public class AssetBundleMgr : MonoSingleton<AssetBundleMgr>
    {
        //ab包的路径,不同平台可以有不同路径
        private static string PerPath => Application.persistentDataPath + "/";
        private static string StreamingPath => Application.streamingAssetsPath + "/";

        //主包名称,不同平台可以有不同的名称
        private static string MainAbName => "PC";

        //主包 和 主包配置文件
        private AssetBundle _mainAb;

        private AssetBundleManifest _abManifest;

        //ab包字典
        private Dictionary<string, AssetBundle> _abDic;

        private void Awake()
        {
            _abDic = new Dictionary<string, AssetBundle>();

            //1:加载主包
            //if (_mainAb) return;

            //加载PC包和配置文件
            _mainAb = File.Exists(PerPath + MainAbName)
                ? AssetBundle.LoadFromFile(PerPath + MainAbName)
                : AssetBundle.LoadFromFile(StreamingPath + MainAbName);

            _abManifest = _mainAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 加载ab包
        /// </summary>
        /// <param name="abName"></param>
        public void LoadAssetBundle(string abName)
        {
            //2:加载ab包的依赖
            var allDependencies = _abManifest.GetAllDependencies(abName);
            for (var i = 0; i < allDependencies.Length; i++)
            {
                var dpAbName = allDependencies[i];

                if (!_abDic.ContainsKey(dpAbName))
                {
                    //先判断持久化路径有无要加载的依赖ab，有则加载，无则去寻找streaming是否有要加载的依赖ab
                    AssetBundle ab;
                    ab = File.Exists(PerPath + dpAbName)
                        ? AssetBundle.LoadFromFile(PerPath + dpAbName)
                        : AssetBundle.LoadFromFile(StreamingPath + dpAbName);

                    //添加到字典
                    _abDic.Add(dpAbName, ab);
                }
            }

            //3:加载ab包
            if (!_abDic.ContainsKey(abName))
            {
                //加载目标ab包
                _abDic.Add(abName, File.Exists(PerPath + abName)
                    ? AssetBundle.LoadFromFile(PerPath + abName)
                    : AssetBundle.LoadFromFile(StreamingPath + abName));
            }
        }

        /// <summary>
        /// 异步加载ab包
        /// </summary>
        /// <param name="abName"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAssetBundleAsync(string abName)
        {
            StartCoroutine(ILoadAssetBundle(abName));
        }

        /// <summary>
        /// 同步加载Ab包的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, Type type)
        {
            //加载ab包
            LoadAssetBundle(abName);
            //加载目标资源
            var targetRes = _abDic[abName].LoadAsset(resName, type);
            return targetRes;
        }

        /// <summary>
        /// 同步加载Ab包的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            //加载ab包
            LoadAssetBundle(abName);
            //加载目标资源
            var targetRes = _abDic[abName].LoadAsset<T>(resName);
            return targetRes;
        }

        /// <summary>
        /// 异步加载Ab包中的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callBack"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadResAsync<T>(string abName, string resName, Action<T> callBack = null) where T : Object
        {
            StartCoroutine(ILoadResAsync<T>(abName, resName, callBack));
        }

        /// <summary>
        /// 异步加载Ab包中的资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public void LoadResAsync(string abName, string resName, Type type, Action<Object> callBack = null)
        {
            StartCoroutine(ILoadResAsync(abName, resName, type, callBack));
        }

        /// <summary>
        /// 卸载单个Ab包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isUnloadAllLoadedObjects"></param>
        public void UnloadAssetBundle(string abName, bool isUnloadAllLoadedObjects = false)
        {
            if (!_abDic.TryGetValue(abName, out var ab)) return;
            ab.Unload(isUnloadAllLoadedObjects);
            _abDic.Remove(abName);
        }

        /// <summary>
        /// 卸载所有Ab包
        /// </summary>
        /// <param name="isUnloadAllLoadedObjects"></param>
        public void UnloadAllAssetBundle(bool isUnloadAllLoadedObjects = false)
        {
            AssetBundle.UnloadAllAssetBundles(isUnloadAllLoadedObjects);
            _abDic.Clear();
            _mainAb = null;
            _abManifest = null;
        }

        private IEnumerator ILoadAssetBundle(string abName)
        {
            //2:加载ab包的依赖
            var allDependencies = _abManifest.GetAllDependencies(abName);
            for (var i = 0; i < allDependencies.Length; i++)
            {
                var dpAbName = allDependencies[i];
                if (_abDic.ContainsKey(dpAbName)) continue;
                var abcr = AssetBundle.LoadFromFileAsync(PerPath + dpAbName);
                yield return abcr;
                _abDic.Add(dpAbName, abcr.assetBundle);
            }

            //3:加载ab包
            if (_abDic.ContainsKey(abName)) yield break;
            {
                var abcr = AssetBundle.LoadFromFileAsync(PerPath + abName);
                yield return abcr;
                _abDic.Add(abName, abcr.assetBundle);
            }
        }

        private IEnumerator ILoadResAsync<T>(string abName, string resName, Action<T> callBack = null) where T : Object
        {
            //异步加载ab包
            yield return ILoadAssetBundle(abName);

            //加载目标资源
            var abr = _abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;

            //执行回调函数
            callBack?.Invoke(abr.asset as T);
        }

        private IEnumerator ILoadResAsync(string abName, string resName, Type type, Action<Object> callBack = null)
        {
            //异步加载ab包
            yield return ILoadAssetBundle(abName);

            //加载目标资源
            var abr = _abDic[abName].LoadAssetAsync(resName, type);
            yield return abr;

            //执行回调函数
            callBack?.Invoke(abr.asset);
        }
    }
}
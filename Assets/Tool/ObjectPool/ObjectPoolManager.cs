using System;
using System.Collections.Generic;
using Tool.Single;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tool.ObjectPool
{
    public struct PoolInfo
    {
        public Stack<GameObject> PoolStack;
        public int MaxSize;
    }

    /// <summary>
    /// 对象池只负责生成对象和对象生成在哪个父类，不负责其他
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private const int Maxsize = 999;
        private Dictionary<string, PoolInfo> _poolsDic;
        protected override void OnInit() => _poolsDic = new Dictionary<string, PoolInfo>();

        /// <summary>
        /// 生成对象池
        /// </summary>
        /// <param name="gameObject">元素</param>
        /// <param name="initSize">初始化元素个数，默认0</param>
        /// <param name="maxSize">池子最大元素，默认999</param>
        /// <returns></returns>
        public bool CreatePoolInfo(GameObject gameObject, int initSize = 0, int maxSize = Maxsize)
        {
            //根据元素名字获取池子信息
            var poolName = gameObject.name.Split('(')[0];
            if (_poolsDic.TryGetValue(poolName, out _)) return true;

            //如果没有池子则创建一个池子，然后初始化池子
            var newPoolInfo = new PoolInfo()
            {
                PoolStack = new Stack<GameObject>(initSize),
                MaxSize = maxSize,
            };

            //初始化池子元素
            for (var i = 0; i < initSize; i++)
            {
                var createItem = Object.Instantiate(gameObject,Vector3.zero, Quaternion.identity);
                createItem.SetActive(false);
                newPoolInfo.PoolStack.Push(createItem);
            }

            //存入字典
            return _poolsDic.TryAdd(poolName, newPoolInfo);
        }

        /// <summary>
        /// 池子中获取一个元素
        /// </summary>
        /// <param name="gameObject">元素</param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(GameObject gameObject)
        {
            //如果有池子信息，则从池子中获取元素
            var poolName = gameObject.name.Split('(')[0];
            if (!_poolsDic.TryGetValue(poolName, out var poolInfo)) throw new Exception($"名字为{poolName}的池子不存在");
            
            //判断池子中是否有元素，有则返回，无则实例
            if (poolInfo.PoolStack.Count <= 0) return Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            var returnItem = poolInfo.PoolStack.Pop();
            returnItem.SetActive(true);
            return returnItem;
        }

        /// <summary>
        /// 销毁或返回元素到池子中
        /// </summary>
        /// <param name="gameObject">元素</param>
        public void ReturnObjectToPool(GameObject gameObject)
        {
            var poolName = gameObject.name.Split('(')[0];

            //判断有无对应池子信息
            if (!_poolsDic.TryGetValue(poolName, out var poolInfo))
            {
                Object.Destroy(gameObject);
                return;
            }

            //如果当前物品已经存入池子，则不执行下面操作
            if (poolInfo.PoolStack.Contains(gameObject)) return;
            //对象失活
            gameObject.SetActive(false);
            //判断池子是否已满，或者该池子没有限制最大容量
            if (poolInfo.PoolStack.Count < poolInfo.MaxSize || poolInfo.MaxSize == Maxsize)
                poolInfo.PoolStack.Push(gameObject);
            else Object.Destroy(gameObject);
        }
    }
}
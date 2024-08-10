using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tool.Mono
{
    public class CoroutineManager : MonoBehaviour
    {
        private readonly List<IEnumerator> _coroutineList = new List<IEnumerator>();

        public new void StartCoroutine(IEnumerator coroutine)
        {
            _coroutineList.Add(coroutine);
        }

        public new void StopCoroutine(IEnumerator coroutine)
        {
            var enumerator = _coroutineList.Find(item => item.ToString().Equals(coroutine.ToString()));
            _coroutineList.Remove(enumerator);
        }

        private void Update()
        {
            Update(Time.deltaTime);
        }

        private void Update(float deltaTime)
        {
            //遍历枚举器
            for (var i = 0; i < _coroutineList.Count; i++)
            {
                var enumerator = _coroutineList[i];
                if (enumerator == null) continue;
                if (enumerator.Current is WaitSeconds waitSeconds)
                {
                    waitSeconds.Duration -= deltaTime;
                    if (waitSeconds.Duration > 0) continue;
                }

                //获取枚举器下一个元素
                if (!enumerator.MoveNext())
                {
                    //如果枚举器已经没有元素，则移除枚举器队列
                    _coroutineList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
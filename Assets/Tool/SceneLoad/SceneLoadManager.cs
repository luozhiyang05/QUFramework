using System;
using System.Collections;
using Tool.Mono;
using Tool.Single;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tool.SceneLoad
{
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {

        protected override void OnInit()
        {
           
        }

        public void SyncLoadScene(string sceneName, Action callBack)
        {
            SceneManager.LoadScene(sceneName);
            callBack?.Invoke();
        }

        public void AsyncLoadScene(string sceneName, float bufferBlackUITime = 0, float waitTime = 0, Action callBack = null,CanvasGroup canvasGroup = null) =>
            PublicMonoKit.GetInstance().GetPublicMono().StartCoroutine(IAsyncLoadScene(sceneName, bufferBlackUITime, waitTime, callBack,canvasGroup));

        public string GetNowSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }


        private IEnumerator IAsyncLoadScene(string sceneName, float bufferBlackUITime, float waitTime, Action callBack,CanvasGroup canvasGroup = null)
        {
            if(canvasGroup) yield return Fade(canvasGroup,1, bufferBlackUITime);
           

            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            loadSceneAsync.allowSceneActivation = false;
            while (!loadSceneAsync.isDone)
            {
                if (loadSceneAsync.progress >= 0.9f)
                    loadSceneAsync.allowSceneActivation = true;
                yield return null;
            }

            callBack?.Invoke();

            yield return new WaitForSeconds(waitTime);

            if(canvasGroup) yield return Fade(canvasGroup,0, bufferBlackUITime);
        }

        private IEnumerator Fade(CanvasGroup canvasGroup,int alpha, float time)
        {
            canvasGroup.blocksRaycasts = true;

            float percent = 0;
            float oldAlpha = canvasGroup.alpha;
 
            while (percent < 1)
            {
                percent += Time.deltaTime / time;
                canvasGroup.alpha = Mathf.Lerp(oldAlpha, alpha, percent);
                yield return null;
            }

            canvasGroup.alpha = alpha;

            canvasGroup.blocksRaycasts = false;
        }
    }
}
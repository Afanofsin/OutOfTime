using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProjectFiles.Code.UIScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectFiles.Code.Controllers
{
    public class SceneController : MonoBehaviour
    {
        #region Singleton/Awake
        
        public static SceneController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        #endregion
        
        [SerializeField] private LoadingOverlay loadingOverlay;
        
        private Dictionary<string, string> loadedScenes = new Dictionary<string, string>();
        private bool isBusy = false;

        public SceneTransitionPlan NewTransition()
        {
            return new SceneTransitionPlan();
        }

        private async UniTask ExecutePlan(SceneTransitionPlan plan)
        {
            if (isBusy)
            {
                Debug.LogWarning("Scene transition is already in progress.");
                return;
            }
            isBusy = true;
            await ChangeSceneRoutine(plan);
        }

        private async UniTask ChangeSceneRoutine(SceneTransitionPlan plan)
        {
            if (plan.Overlay)
            {
                await loadingOverlay.FadeIn();
                await UniTask.Delay(500);
            }

            foreach (var slotKey in plan.ScenesToUnload)
            {
                await UnloadSceneRoutine(slotKey);
            }

            if (plan.ClearUnusedAssets) await CleanupUnusedAssets();
            foreach (var kvp in plan.ScenesToLoad)
            {
                if (loadedScenes.ContainsKey(kvp.Key))
                {
                    await UnloadSceneRoutine(kvp.Key);
                }
                await LoadAdditiveScene(kvp.Key, kvp.Value, plan.ActiveScene == kvp.Value);
            }

            if (plan.Overlay)
            {
                await loadingOverlay.FadeOut();
            }

            isBusy = false;
        }

        private async UniTask LoadAdditiveScene(string slotKey, string sceneName, bool setActive)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOp == null) return;
            loadOp.allowSceneActivation = false;
            while (loadOp.progress < 0.9f)
            {
                await UniTask.Yield();
            }
            
            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
            {
                await UniTask.Yield();
            }

            if (setActive)
            {
                Scene newScene = SceneManager.GetSceneByName(sceneName);
                if (newScene.IsValid() && newScene.isLoaded)
                {
                    SceneManager.SetActiveScene(newScene);
                }
            }
            loadedScenes[slotKey] = sceneName;
        }

        private async UniTask UnloadSceneRoutine(string slotKey)
        {
            if (!loadedScenes.TryGetValue(slotKey, out var sceneName)) return;
            if (string.IsNullOrEmpty(sceneName)) return;
             AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
             if (unloadOp != null)
             {
                 while (!unloadOp.isDone)
                 {
                     await UniTask.Yield();
                 }
             }

             loadedScenes.Remove(slotKey);
        }

        private async UniTask CleanupUnusedAssets()
        {
            AsyncOperation cleanupOp = Resources.UnloadUnusedAssets();
            while (!cleanupOp.isDone)
            {
                await UniTask.Yield();
            }
            
            
        }

        public class SceneTransitionPlan
        {
            public Dictionary<string, string> ScenesToLoad { get; } = new Dictionary<string, string>();
            public List<string> ScenesToUnload { get; } = new List<string>();
            public string ActiveScene { get; private set; } = "";
            public bool ClearUnusedAssets { get; private set; } = false;
            public bool Overlay { get; private set; } = false;

            public SceneTransitionPlan Load(string slotKey, string sceneName, bool setActive = false)
            {
                ScenesToLoad[slotKey] = sceneName;
                if(setActive) ActiveScene = sceneName;
                return this;
            }

            public SceneTransitionPlan Unload(string slotKey)
            {
                ScenesToUnload.Add(slotKey);
                return this;
            }

            public SceneTransitionPlan WithOverlay()
            {
                Overlay = true;
                return this;
            }

            public SceneTransitionPlan WithClearUnusedAssets()
            {
                ClearUnusedAssets = true;
                return this;
            }

            public UniTask Perform()
            {
                return SceneController.Instance.ExecutePlan(this);
            }
        }
    }
}
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneManagment
{
    public class SceneGroupManager
    {
        #region Actions and Events
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };
        
        #endregion
        
        [Header("Scenes")]SceneGroup ActiveSceneGroup;

        public IEnumerator LoadSceneCoroutine(SceneGroup sGroup, IProgress<float> progress = null, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = sGroup;
            yield return managerOwner.StartCoroutine(UnloadScenesCoroutine());
            
            int total = ActiveSceneGroup.Scenes.Count;
            int doneCount = 0;
            
            foreach (var sceneData in sGroup.Scenes)
            {
                if (!Application.CanStreamedLevelBeLoaded(sceneData.Name))
                {
                    Debug.LogError($"SceneGroupManager: Scene '{sceneData.Name}' not found in build settings.");
                    continue;
                }
                
                Debug.Log($"SceneGroupManager: Loading scene '{sceneData.Name}'...");
                var op = SceneManager.LoadSceneAsync(sceneData.Name, LoadSceneMode.Additive);
                op.allowSceneActivation = true;

                while (!op.isDone)
                {
                    float overall = (doneCount + op.progress) / (float)total;
                    progress?.Report(overall);
                    yield return null;
                }
                doneCount++;
                OnSceneLoaded(sceneData.Name);
                Debug.Log($"SceneGroupManager: Scene '{sceneData.Name}' loaded.");
            }

            string activeName = ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene);
            var sc = SceneManager.GetSceneByName(activeName);
            if(sc.IsValid()) SceneManager.SetActiveScene(sc);
            
            OnSceneGroupLoaded();
            Debug.Log($"SceneGroupManager: SceneGroup '{sGroup.GroupName}' complete loaded.");
        }

        public IEnumerator UnloadScenesCoroutine()
        {
            var toUnload = new List<string>();
            string active = SceneManager.GetActiveScene().name;
            int count = SceneManager.sceneCount;

            for (int i = count - 1; i > 0; i--)
            {
                var s = SceneManager.GetSceneAt(i);
                if (!s.isLoaded) continue;
                
                if (s.name == active || s.name == "Bootstrapper" || s.name == "PersistantPlayer" || s.name == "UI") continue;
                
                toUnload.Add(s.name);
            }

            foreach (var name in toUnload)
            {
                Debug.Log($"SceneGroupManager: Unloading scene '{name}'...");
                var op = SceneManager.UnloadSceneAsync(name);
                if (op == null) continue;
                while (!op.isDone) yield return null;
                Debug.Log($"SceneGroupManager: Scene '{name}' unloaded.");
            }
        }
        
        MonoBehaviour managerOwner;
        public SceneGroupManager(MonoBehaviour owner) => managerOwner = owner;
    }

    public readonly struct AsyncOperationGroup
    {
        #region Variables

        public readonly List<AsyncOperation> Operations;
        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        #endregion
        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}

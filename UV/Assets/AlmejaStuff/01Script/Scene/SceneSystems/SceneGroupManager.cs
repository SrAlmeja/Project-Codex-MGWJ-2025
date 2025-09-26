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
        #region Variables
        
        private string _activeName;
        private int _total;
        private int _doneCount;
        private float _overall;

        private List<string> _toUnload;
        private string _active;
        private int _count;
        #endregion
        
        #region Dependencies
        private readonly MonoBehaviour managerOwner;
        #endregion

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
            
            _total = ActiveSceneGroup.Scenes.Count;
            _doneCount = 0;
            
            foreach (var sceneData in sGroup.Scenes)
            {
                if (!Application.CanStreamedLevelBeLoaded(sceneData.Name))
                {
                    Debug.LogError($"SceneGroupManager: Scene '{sceneData.Name}' not found in build settings.");
                    continue;
                }
                
                //  Debug.Log($"SceneGroupManager: Loading scene '{sceneData.Name}'..."); //Nombra la escena cargada
                var op = SceneManager.LoadSceneAsync(sceneData.Name, LoadSceneMode.Additive);
                op.allowSceneActivation = true;

                while (!op.isDone)
                {
                    _overall = (_doneCount + op.progress) / (float)_total;
                    progress?.Report(_overall);
                    yield return null;
                }
                _doneCount++;
                OnSceneLoaded(sceneData.Name);
                // Debug.Log($"SceneGroupManager: Scene '{sceneData.Name}' loaded."); //Nombra la escena cargada
            }

            _activeName = ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene);
            var s = SceneManager.GetSceneByName(_activeName);
            if(s.IsValid()) SceneManager.SetActiveScene(s);
            
            OnSceneGroupLoaded();
            //Debug.Log($"SceneGroupManager: SceneGroup '{sGroup.GroupName}' complete loaded.");
        }

        public IEnumerator UnloadScenesCoroutine()
        {
            _toUnload = new List<string>();
            _active = SceneManager.GetActiveScene().name;
            _count = SceneManager.sceneCount;

            for (int i = _count - 1; i > 0; i--)
            {
                var s = SceneManager.GetSceneAt(i);
                if (!s.isLoaded) continue;
                
                if (s.name == _active || s.name == "Bootstrapper" || s.name == "UI") continue;
                
                _toUnload.Add(s.name);
            }

            foreach (var name in _toUnload)
            {
                //Debug.Log($"SceneGroupManager: Unloading scene '{name}'..."); //Muestra que escena se va a desinstalar.
                var op = SceneManager.UnloadSceneAsync(name);
                if (op == null) continue;
                while (!op.isDone) yield return null;
                //Debug.Log($"SceneGroupManager: Scene '{name}' unloaded."); //Muestra que escena ya se desinstaló
            }
        }
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

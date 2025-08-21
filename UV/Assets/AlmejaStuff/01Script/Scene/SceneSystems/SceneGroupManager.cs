using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.Callbacks;
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
        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            foreach (var sceneData in group.Scenes)
            {
                if (!Application.CanStreamedLevelBeLoaded(sceneData.Name))
                {
                    Debug.LogError($"La escena '{sceneData.Name}' no está en el Build Settings.");
                    continue;
                }

                var operation = SceneManager.LoadSceneAsync(sceneData.Name, LoadSceneMode.Additive);
                if (operation == null)
                {
                    Debug.LogError($"No se pudo iniciar la carga de la escena '{sceneData.Name}'.");
                    continue;
                }

                operation.allowSceneActivation = true; // ← Esto es clave
                operationGroup.Operations.Add(operation);
                OnSceneLoaded.Invoke(sceneData.Name);

            }
            
            // TimeOut
            int timeoutMs = 3000; // Tiempo máximo de espera: 15 segundos
            int elapsedMs = 0;
            int delayMs = 100;

            while (!operationGroup.IsDone && elapsedMs < timeoutMs)
            {
                progress?.Report(operationGroup.Progress);
                await Task.Delay(delayMs);
                elapsedMs += delayMs;
            }

            if (!operationGroup.IsDone)
            {
                Debug.LogError("Timeout: Las escenas no terminaron de cargar en el tiempo esperado.");
                foreach (var op in operationGroup.Operations)
                {
                    if (op != null && !op.isDone)
                    {
                        Debug.LogWarning($"Escena aún en carga: {op.allowSceneActivation}, progreso: {op.progress}");
                    }
                }
            }

            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes()
        {
            #region UnloadScenesVariables
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            int sceneCount = SceneManager.sceneCount;
            #endregion

            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                
                if (sceneName.Equals(activeScene) || 
                    sceneName == "Bootstrapper" || 
                    sceneName == "PersistantPlayer" || 
                    sceneName == "UI") continue;

                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            while (!operationGroup.IsDone)
            {
                await Task.Delay(100);
            }
        }
        
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;
        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}

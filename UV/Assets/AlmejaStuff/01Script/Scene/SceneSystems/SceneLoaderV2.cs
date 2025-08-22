using System;
using System.Collections;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoaderV2 : MonoBehaviour
    {
        #region Singleton
        public static SceneLoaderV2 Instance { get; private set; }
        #endregion

        #region Serialized Fields
        [SerializeField, Header("Loading Stuff")] private Image loadingBar;
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Camera loadingCamera;
        [SerializeField, Header("Scenes Reference")] private SceneGroup[] scenesToLoad;
        #endregion

        #region Internal State
        private LoadingProgress _progress;
        private float _targetProgress;
        private bool _isLoading;
        private int _idx;
        private SceneGroupManager Manager;
        #endregion

        #region Unity Functions

        void Awake()
        {
            MakeMePersistent();
            Manager = new SceneGroupManager(this);
            Manager.OnSceneGroupLoaded += OnGroupLoaded;
        }
        void Start()
        {
            if (scenesToLoad == null || scenesToLoad.Length == 0 || scenesToLoad[0] == null)
            {
                Debug.LogWarning("SceneLoaderV2: No hay escenas definidas para cargar.");
                return;
            }
        }
        private void MakeMePersistent()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion
        
        #region LoaderFunctions
        public void LoadSceneGroupByName(string groupName)
        {
            _idx = Array.FindIndex(scenesToLoad, g => g.GroupName == groupName);
            if (_idx < 0)
            {
                Debug.LogError($"SceneLoaderV2: The SceneGroup '{groupName}' was not found)");
                return;
            }

            StartCoroutine(LoadSceneGroupCoroutine(_idx));
        }

        private IEnumerator LoadSceneGroupCoroutine(int index)
        {
            loadingBar.fillAmount = 0;
            _targetProgress = 0;
            EnableLoadingCanvas(true);;

            _progress = new LoadingProgress();
            _progress.Progressed += p => _targetProgress = MathF.Max(_targetProgress, p);
            
            yield return StartCoroutine(Manager.LoadSceneCoroutine(scenesToLoad[index], _progress));
        }
        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
        private void OnGroupLoaded()
        {
            EnableLoadingCanvas(false);
            Debug.Log("SceneLoaderV2: SceneGroup loaded (event).");
        }
        #endregion
    }

    public class LoadingProgress : IProgress<float>
    { 
        public event Action<float> Progressed;
        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}


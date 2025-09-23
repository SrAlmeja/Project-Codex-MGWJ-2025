using System;
using System.Collections;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Systems.SceneManagement
{
    public class SceneLoaderV2 : PersistentSingleton<SceneLoaderV2>
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
            base.Awake();
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
            LoadSceneGroupByName(scenesToLoad[0].GroupName);
        }
        
        void Update()
        {
            if (_isLoading) return;
            
            float current = loadingBar.fillAmount;
            float smoothed = Mathf.SmoothStep(current, _targetProgress, Time.deltaTime * fillSpeed);
            loadingBar.fillAmount = smoothed;
        }

        #endregion
        #region LoaderFunctions
        #region SceneController
        /// <summary>
        /// Load scene by name.
        /// </summary>
        public void LoadScene(string sceneName/*, LoadSceneMode mode = LoadSceneMode.Single*/)
        {
            SceneManager.LoadScene(sceneName/*, mode*/);
        }
        /// <summary>
        /// A public method to load a scene from a button.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneFromButton(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }    
        /// <summary>
        /// Reload current active scene.
        /// </summary>
        public void ReloadCurrentScene()
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }

        /// <summary>
        /// Load scene asynchronously with optional delay or loading screen.
        /// </summary>
        public void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            StartCoroutine(AsyncScene(sceneName, mode));
        }
    
        public void CloseApplication()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
#endif
        }
    
        private IEnumerator AsyncScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        private IEnumerator LoadSingleSceneCoroutine(string sceneName)
        {
            loadingBar.fillAmount = 0;
            _targetProgress = 0;
            EnableLoadingCanvas(true);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                _targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                yield return null;
            }

            EnableLoadingCanvas(false);
        }

        #endregion
        #region ByGroup
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
        #endregion
        #endregion
        #region UI Functions
        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
        private void OnGroupLoaded()
        {
            EnableLoadingCanvas(false);
            //Debug.Log("SceneLoaderV2: SceneGroup loaded (event).");
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


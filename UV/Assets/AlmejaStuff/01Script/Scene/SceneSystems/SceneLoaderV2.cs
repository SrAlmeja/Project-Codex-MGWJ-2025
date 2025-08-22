using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoaderV2 : MonoBehaviour
    {
        #region Loading Variables
        [HideInInspector] public static SceneLoaderV2 Instance {get; private set;}
        [SerializeField, Header("Loading Stuff")]
        private Image loadingBar;
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Camera loadingCamera;

        private float _targetProgress;
        private bool _isLoading;
        
        [SerializeField,Header("ScenesToLoad")]
        SceneGroup[]scenesToLoad;
        
        SceneGroupManager Manager;
        #endregion

        #region Unity Functions

        void Awake()
        {
            MakeMePersistent();
            Manager = new SceneGroupManager(this);
            Manager.OnSceneGroupLoaded += OnGroupLoaded;
        }
        async void Start()
        {
            if (scenesToLoad != null && scenesToLoad.Length > 0) StartCoroutine(LoadSceneGroupCoroutine(0));
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
            int idx = Array.FindIndex(scenesToLoad, g => g.GroupName == groupName);
            if (idx < 0)
            {
                Debug.LogError($"SceneLoaderV2: The SceneGroup '{groupName}' was not found)");
                return;
            }

            StartCoroutine(LoadSceneGroupCoroutine(idx));
        }

        private IEnumerator LoadSceneGroupCoroutine(int index)
        {
            loadingBar.fillAmount = 0;
            _targetProgress = 0;
            EnableLoadingCanvas(true);;

            var progress = new LoadingProgress();
            progress.Progressed += p => _targetProgress = MathF.Max(_targetProgress, p);
            
            yield return StartCoroutine(Manager.LoadSceneCoroutine(scenesToLoad[index], progress));
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


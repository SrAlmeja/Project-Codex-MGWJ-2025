using System;
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
        public readonly SceneGroupManager Manager = new SceneGroupManager();
        #endregion

        #region Unity Functions

        void Awake()
        {
            MakeMePersistent();
            
            Manager.OnSceneLoaded += sceneName => Debug.Log("Loaded" + sceneName);
            Manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded" + sceneName);
            Manager.OnSceneGroupLoaded += () => Debug.Log("Scene Group loaded");
        }
        async void Start()
        {
            await LoadSceneGroup(0);
        }
        private void Update()
        {
            if (!_isLoading)
            {
                float currentFillAmount = loadingBar.fillAmount;
                float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);
                
                float dynamicFillSpeed = progressDifference * fillSpeed;
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
        public async Task LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            _targetProgress = 1f;
            
            if (index < 0 || index >= scenesToLoad.Length)
            {
                Debug.LogError("Invalid scene group index." + index);
                return;
            }
            
            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);
            
            EnableLoadingCanvas();
            await Manager.LoadScenes(scenesToLoad[index], progress);
            EnableLoadingCanvas(false);
        }

        public async Task LoadSceneGroupByName(string groupName)
        {
            var group = scenesToLoad.FirstOrDefault(g => g.GroupName == groupName);
            if (group == null)
            {
                Debug.LogError($"SceneGroup '{groupName}' not found");
            }
            int index = Array.IndexOf(scenesToLoad, group);
            if (index < 0)
            {
                Debug.LogError($"SceneGroup '{groupName}' no se encuentra en el arrya de SceneToLoad");
                return;
            }
            await LoadSceneGroup(index);
        }
        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
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


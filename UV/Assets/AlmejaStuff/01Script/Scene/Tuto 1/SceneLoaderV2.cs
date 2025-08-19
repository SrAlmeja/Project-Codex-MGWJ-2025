using System;
using System.Threading.Tasks;
using Systems.SceneManagment;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoaderV2 : MonoBehaviour
    {
        #region Loading Variables
        [SerializeField, Header("Loading Stuff")] private Image loadingBar;
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Camera loadingCamera;

        private float _targetProgress;
        private bool _isLoading;
        
        [SerializeField,Header("ScenesToLoad")] SceneGroup[]scenesToLoad; 
        public readonly SceneGroupManager Manager = new SceneGroupManager();
        #endregion

        #region Unity Functions

        void Awake()
        {
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

        #endregion
        

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
        
        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
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


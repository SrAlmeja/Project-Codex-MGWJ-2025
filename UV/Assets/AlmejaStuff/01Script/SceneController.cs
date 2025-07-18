using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
<<<<<<< HEAD
    #region Singleton
=======
    #region Singleton (opcional)
>>>>>>> origin/Personal/SrAlmeja
    public static SceneController Instance { get; private set; }

    private void Awake()
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

    #region SceneManagement

    /// <summary>
    /// Load scene by name.
    /// </summary>
<<<<<<< HEAD
    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName, mode);
=======
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
>>>>>>> origin/Personal/SrAlmeja
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
<<<<<<< HEAD
    public void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        StartCoroutine(AsyncScene(sceneName, mode));
=======
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(AsyncScene(sceneName));
>>>>>>> origin/Personal/SrAlmeja
    }
    
    public void CloseApplication()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
        #endif
    }
    
<<<<<<< HEAD
    private IEnumerator AsyncScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
=======
    private IEnumerator AsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
>>>>>>> origin/Personal/SrAlmeja
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    #endregion
}
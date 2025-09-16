using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : PersistentSingleton<Bootstrapper>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Debug.Log("Bootstrapper...");
        LoadBootstrapperScene();
    }
    static void LoadBootstrapperScene()
    {
        if (SceneManager.GetActiveScene().name == "Bootstrapper") return;

        var bootstrapper = new GameObject("BootstrapperLoader").AddComponent<BootstrapperLoader>();
        bootstrapper.StartCoroutine(bootstrapper.LoadBootstrapperCoroutine());
    }
}

public class BootstrapperLoader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadBootstrapperCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Destroy(gameObject); // Limpieza
    }
}

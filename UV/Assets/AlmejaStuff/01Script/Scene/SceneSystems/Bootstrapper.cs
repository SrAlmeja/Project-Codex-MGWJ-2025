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
    static async void LoadBootstrapperScene()
    {
        if (SceneManager.GetActiveScene().name == "Bootstrapper") return;

        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single).AsTask();
    }
}

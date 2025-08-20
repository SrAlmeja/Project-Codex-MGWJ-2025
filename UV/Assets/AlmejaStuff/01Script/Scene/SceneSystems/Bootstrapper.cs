using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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
        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single).AsTask();
    }
}

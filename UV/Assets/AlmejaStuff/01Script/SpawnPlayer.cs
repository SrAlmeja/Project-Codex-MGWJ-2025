using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayer : MonoBehaviour
{
    #region  Variables

    private SceneController _sceneController;

    #endregion

    #region  Singleton 
    
    public static SpawnPlayer Instance { get; private set; }
    
    private void Awake()
    {
        Persistance();
        
        _sceneController = SceneController.Instance;
        
        _sceneController.LoadScene("PersistantPlayer", LoadSceneMode.Additive);
    }

    private void Persistance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        foreach (var scene in SceneManager.GetAllScenes())
        {
            if (scene.name == "PersistantPlayer") return;
        }

    }   
    
    #endregion
}
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuSceneLoader : MonoBehaviour
{
    #region Variables
    [Header("Scene Group Name"), SerializeField]
    private string sceneGroupName;
    
    public void OnStartNGame()
    {
        if (SceneLoaderV2.Instance == null)
        {
            Debug.LogError("SceneLoaderV2 no se encontró en el menú");
            return;
        }
        
        Debug.Log($"Cargando grupo de escenas:  {sceneGroupName}");
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }
    #endregion
}

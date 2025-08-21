using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuSceneLoader : MonoBehaviour
{
    #region Variables
    [Header("Scene Group Name"), SerializeField]
    private string sceneGroupName;
    [SerializeField] private int sceneGroupIndex;
    
    public async void OnStartNGame()
    {
        if (SceneLoaderV2.Instance == null)
        {
            Debug.LogError("SceneLoaderV2 no se encontró en el menú");
            return;
        }

        Debug.Log($"Preparando carga del grupo: {sceneGroupName}");

        // Opcional: descargar el grupo actual si es necesario
        await SceneLoaderV2.Instance.Manager.UnloadScenes();

        // Cargar el nuevo grupo
        await SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
        
        //await SceneLoaderV2.Instance.LoadSceneGroup(sceneGroupIndex);
    }

    #endregion
}

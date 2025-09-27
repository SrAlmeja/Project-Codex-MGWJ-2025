using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuSceneLoader : MonoBehaviour
{
    #region Variables
    [Header("Scene Group Name"), SerializeField]
    private string sceneGroupName;
    
    #endregion
    
    public void OnStartNGame()
    {
        if (SceneLoaderV2.Instance == null)
        {
            Debug.LogError("SceneLoaderV2.Instance es null. ¿Se ejecutó Awake?");
            return;
        }

        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }

}

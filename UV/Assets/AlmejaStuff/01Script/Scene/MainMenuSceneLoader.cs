using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuSceneLoader : MonoBehaviour
{
    #region Variables
    [Header("Scene Group Name"), SerializeField]
    private string sceneGroupName;
    [SerializeField] private int sceneGroupIndex;
    
    #endregion
    
    public void OnStartNGame()
    {
        //Debug.Log($"[Menu] ▶ New Game: {sceneGroupName}");
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }
}

using Systems.SceneManagement;
using UnityEngine;

public class GenericNextLevel : MonoBehaviour
{
    public void NextLevel(string sceneGroupName)
    {
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }
}

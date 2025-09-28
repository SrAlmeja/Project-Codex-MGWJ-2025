using Systems.SceneManagement;
using UnityEngine;

public class TreeLogic : MonoBehaviour
{
    [Header("NextLevel")]
    [SerializeField] private string sceneGroupName;

    private bool _nextLevelCondition = false;

    public SOBoolean conversationFlag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(sceneGroupName) && _nextLevelCondition && conversationFlag.Value == false) //Agregar condición de que finalizó el dialogo
        {
            NextLevel(sceneGroupName);
            return;
        }
    }

    public void LevelConditional()
    {
        _nextLevelCondition = true;
    }

    public void NextLevel(string sceneGroupName)
    {
        SceneLoaderV2.Instance.LoadSceneGroupByName(sceneGroupName);
    }
}

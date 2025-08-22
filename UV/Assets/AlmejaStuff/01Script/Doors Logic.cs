using UnityEngine;

public class DoorsLogic : MonoBehaviour
{
    #region Variables

    [Header("SceneController")] public string NextScene;
    private SceneController _sceneController;

    #endregion
    

    // Start is called before the first frame update
    void Start()
    {
        _sceneController = FindObjectOfType<SceneController>();
    }

    private void SceneToChange()
    {
        if (_sceneController != null)
        {
            _sceneController.LoadScene(NextScene);
        }
        else
        {
            Debug.LogError("SceneController not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player Touchme");
            SceneToChange();
        }
    }
}

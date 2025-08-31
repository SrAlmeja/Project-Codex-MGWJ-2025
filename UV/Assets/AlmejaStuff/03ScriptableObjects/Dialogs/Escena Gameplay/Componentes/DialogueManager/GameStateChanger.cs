using UnityEngine;

public class GameStateChanger : MonoBehaviour
{
    [Header("Dependencies")]
    public GameManagerSO gameManager;

    public void SetGameState(GameStateSO gameState)
    {
        this.gameManager.SetGameState(gameState);
        Debug.Log(gameState.ToString());
    }

    public void RestorePreviousState()
    {
        this.gameManager.RestorePreviousState();
    }
}

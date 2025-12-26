using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public GameState gameState;
    public void LoadLevel()
    {
        SceneManager.LoadScene(gameState.currentLevel.sceneName);
    }
}

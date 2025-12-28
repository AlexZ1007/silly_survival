using UnityEngine;

public class LoadGameState : MonoBehaviour
{
    public GameState gameState;
    public LevelDatabase levelDatabase;

    public void Awake()
    {
        SaveData data = SaveSystem.Load();

        if (data != null && gameState !=null)
        {

            Debug.Log("Loading game state: Level ID " + data.currentLevelID);
            LevelData levelData = levelDatabase.GetByLevelId(data.currentLevelID);

            if (levelData != null)
            {
                gameState.currentLevel = levelData;
            }
            else
            {
                gameState.currentLevel = levelDatabase.GetByLevelId(1); // Fallback to level ID 1
                Debug.LogWarning("LevelData not found for level ID: " + data.currentLevelID);
            }
        } else if(gameState != null)
        {
            gameState.currentLevel = levelDatabase.GetByLevelId(1); // Fallback to level ID 1
        }

        SoundManager.Instance.PlayMusic("background");
    }

}

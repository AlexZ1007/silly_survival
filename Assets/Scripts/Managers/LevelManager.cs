using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelData levelData;
    public GameState gameState;

    [Header("Scenes")]
    public string nextLevelMenuScene = "NextLevelMenu";
    public string endGameScene = "EndScene";

    [Header("Dispaly")]
    public TMP_Text questText;
    private void Awake()
    {
        // Register this level as current
        if (gameState != null)
            gameState.currentLevel = levelData;

        if (questText != null && levelData != null && levelData.requiredItem != null)
        {
            questText.text = $"Quest: Craft {levelData.requiredItem.itemName}";
        }
        else
        {
            Debug.LogWarning("QuestUI: Missing references for quest text.");
        }

        Debug.Log("Loaded level: " + levelData.sceneName);
    }

    public void OnItemCrafted(ItemScriptableObject craftedItem)
    {
        if (craftedItem != levelData.requiredItem)
            return;

        CompleteLevel();
    }

    private void CompleteLevel()
    {
        // No next level → end game

        if (levelData.nextLevel == null)
        {
            SceneManager.LoadScene(endGameScene);
            return;
        }

        // Advance progression in GameState
        levelData = levelData.nextLevel;
        if (gameState != null)
            gameState.currentLevel = levelData;

        // Go to menu (or could go directly to level)
        SceneManager.LoadScene(nextLevelMenuScene);
    }

    public void SaveTo(SaveData data)
    {
        data.currentLevelID = levelData.levelID;
    }

}

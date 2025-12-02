using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public CameraController platerCamera;

    [Header("UI")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private void Start()
    {
        LoadGame();

        // Make sure pause menu starts hidden
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    private void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        // Toggle pause on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    // Pause the game
    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    // Resume game from pause
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }


    public void SaveGame()
    {
        SaveData data = new SaveData();
        player.SaveTo(data);
        platerCamera.SaveTo(data);
        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        player.LoadFrom(data);
        platerCamera.LoadFrom(data);
    }

    // Auto save game on application quit
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SaveGame();            
        Application.Quit();   
    }


}

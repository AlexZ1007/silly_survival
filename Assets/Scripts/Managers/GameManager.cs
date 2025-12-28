using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public static bool applySaveOnStart = true;
    private static bool saveApplied = false;

    public PlayerController player;
    public CameraController platerCamera;
    public LevelManager levelManager;


    [Header("UI")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;


    // Keep this object across scenes
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (applySaveOnStart && !saveApplied)
        {
            LoadGame();
            saveApplied = true;
        }


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
        if(player!= null)
            player.SaveTo(data);
        if(platerCamera != null)
            platerCamera.SaveTo(data);
        if(levelManager != null)
            levelManager.SaveTo(data);
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

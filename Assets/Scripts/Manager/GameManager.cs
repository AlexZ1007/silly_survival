using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public CameraController platerCamera;

    // Auto load game on start
    private void Start()
    {
        LoadGame();
    }

    // Auto save game on application quit
    private void OnApplicationQuit()
    {
        SaveGame();
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

}

using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    // Function to save data to a JSON file
    public static void Save(SaveData data)
    {
        Debug.Log("Saving to: " + path);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    // Function to load data from a JSON file
    public static SaveData Load()
    {
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}

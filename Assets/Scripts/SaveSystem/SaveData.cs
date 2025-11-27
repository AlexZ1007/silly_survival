using System.Collections.Generic;

[System.Serializable]    
public class SaveData
{
    public float[] playerPosition;
    public List<ItemEntry> items;
    public float[] cameraPosition;
    public float[] cameraRotation;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelData> levels;

    public LevelData GetByLevelId(int levelId)
    {
        foreach (var level in levels)
        {
            if (level.levelID == levelId)
                return level;
        }
        return null;
    }

}

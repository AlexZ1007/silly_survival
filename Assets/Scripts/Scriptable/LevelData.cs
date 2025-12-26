using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelID;
    public string sceneName;
    public ItemScriptableObject requiredItem;
    public LevelData nextLevel;
}

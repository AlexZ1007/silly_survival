using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Game State")]
public class GameState : ScriptableObject
{
    public LevelData currentLevel;
}

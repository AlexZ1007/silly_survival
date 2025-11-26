using UnityEngine;

public class InteractableAction : ScriptableObject
{
    public string actionName;//action name

    [System.Serializable]
    public struct ToolRequirement
    {
        public WeaponType tool;
        public float holdTime;//how long to hold the action key
    }

    public ToolRequirement[] toolRequirements; // Assign in inspector
    public float respawnTime = 150f;// how long until the object respawns
    public GameObject[] drops;
   


    public enum WeaponType
    {
        None,
        Axe1,
        Axe2,
        Axe3,
        Pickaxe1,
        Pickaxe2,
        Pickaxe3,
        Shovel1,
        Shovel2,
        Shovel3,
        Bat,
        Sword
    }

}
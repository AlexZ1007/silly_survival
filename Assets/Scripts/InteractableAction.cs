using UnityEngine;

public class InteractableAction : ScriptableObject
{
    public string actionName;//action name
    public float holdTime = 3f;//how long to hold the action key
    public float respawnTime = 150f;// how long until the object respawns


    public WeaponType[] allowedTools;//list of tools allowed for this action

    // prefabs that spawn when action is complete
    public GameObject[] drops;

    
    public enum WeaponType
    {
        None,
        Axe,
        Pickaxe,
        Shovel,
        Weapon
    }
}

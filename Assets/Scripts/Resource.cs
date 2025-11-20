using UnityEngine;

public class Resource : MonoBehaviour
{
    //resource type
    public enum ResourceType
    {
        Stone,
        Gold,
        Iron,
        Tree,
        Bush_With_Fruits,
        Dirt,
        Sand,
        Snow,
        Coal,
        Wheat,
        Grass,
        Flower
    }

    // object type 
    [Header("Resource Settings")]
    public ResourceType resourceType;

    /*ScriptableObject that defines:
        - required hold time
        - allowed tools
        - what drops spawn
        - respwawn time
    */
    [Header("Action Required")]
    public InteractableAction actionRequired;
}

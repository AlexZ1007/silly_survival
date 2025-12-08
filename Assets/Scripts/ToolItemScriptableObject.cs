using UnityEngine;

[CreateAssetMenu(fileName = "ToolItem", menuName = "Item/ToolItem")]
public class ToolItemScriptableObject : ItemScriptableObject
{
    public InteractableAction.WeaponType weaponType; // axe, pickaxe, shovel
}

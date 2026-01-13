using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public LevelManager levelManager;

    public bool TryCraft(RecipeScriptable recipe)
    {
        // 1. Check if player has all ingredients
        foreach (var entry in recipe.ingredients)
        {
            int playerCount = playerInventory.GetAmount(entry.item);

            if (playerCount < entry.amount)
            {
                Debug.Log("Not enough: " + entry.item.name);
                return false;
            }
        }


        // 2. Give result item
        if(playerInventory.TryAddItem(recipe.resultItem, 1) == false)
        {
            Debug.Log("Inventory full, cannot craft: " + recipe.resultItem.name);
            // Optionally, you could return the ingredients back to the inventory here
            return false;
        }


        // 3. Remove ingredients
        foreach (var entry in recipe.ingredients)
        {
            playerInventory.RemoveItem(entry.item, entry.amount);
        }


        // 4. Check level manager if the level is finished
        if (levelManager != null)
        {
            levelManager.OnItemCrafted(recipe.resultItem);
        }

        Debug.Log("Crafted: " + recipe.resultItem.name);
        return true;
    }

}

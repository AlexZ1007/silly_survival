using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public PlayerInventory playerInventory;

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

        // 2. Remove ingredients
        foreach (var entry in recipe.ingredients)
        {
            playerInventory.RemoveItem(entry.item, entry.amount);
        }

        // 3. Give result item
        playerInventory.AddItem(recipe.resultItem, 1);

        Debug.Log("Crafted: " + recipe.resultItem.name);
        return true;
    }

}

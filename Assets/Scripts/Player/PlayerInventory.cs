using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<ItemScriptableObject, int> items = new Dictionary<ItemScriptableObject, int>();

    public event Action OnInventoryChanged;

    public void AddItem(ItemScriptableObject item, int amount = 1)
    {
        if (items.ContainsKey(item))
            items[item] += amount;
        else
            items.Add(item, amount);

        Debug.Log($"Added {amount}x {item.itemName}. Total: {items[item]}");

        OnInventoryChanged?.Invoke();
    }

    public int GetAmount(ItemScriptableObject item)
    {
        return items.TryGetValue(item, out int count) ? count : 0;
    }

    public Dictionary<ItemScriptableObject, int> GetAllItems()
    {
        return new Dictionary<ItemScriptableObject, int>(items);
    }

    public void RemoveItem(ItemScriptableObject item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= amount;
            if (items[item] <= 0)
                items.Remove(item);
            Debug.Log($"Removed {amount}x {item.itemName}. Remaining: {GetAmount(item)}");
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Attempted to remove {item.itemName} which is not in inventory.");
        }
    }

    public List<RecipeScriptable> GetVisibleRecipes(RecipeDatabase database)
    {
        List<RecipeScriptable> result = new List<RecipeScriptable>();

        foreach (var recipe in database.allRecipes)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (this.GetAmount(ingredient.item) > 0)
                {
                    result.Add(recipe);
                    break; 
                }
            }
        }

        return result;
    }

}

using NUnit;
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
            if(recipe.requiredStation != null && this.GetAmount(recipe.requiredStation) <= 0)
                continue;

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

    // Function to save inventory data
    public void SaveTo(SaveData data)
    {
        data.items = new List<ItemEntry>();

        foreach (var pair in items)
        {
            data.items.Add(new ItemEntry
            {
                itemName = pair.Key.itemName,  // unique ID
                amount = pair.Value
            });
        }

    }

    // Function to load inventory data
    public void LoadFrom(SaveData data)
    {
        items = new Dictionary<ItemScriptableObject, int>();
            

        // load all item assets from folder
        ItemScriptableObject[] allItems =
            Resources.LoadAll<ItemScriptableObject>("Items");

        foreach (var item in allItems)
        {
            Debug.Log($"Loaded item asset: {item.itemName}");
        }

        foreach (var entry in data.items)
        {
            // find matching item
            ItemScriptableObject item =
                System.Array.Find(allItems, x => x.itemName == entry.itemName);

            if (item != null)
            {
                items[item] = entry.amount;
            }
            else
            {
                Debug.LogWarning($"Item not found: {entry.itemName}");
            }
        }
    }

}


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
}

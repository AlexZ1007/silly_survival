using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipieScriptable", menuName = "Scriptable Objects/RecipieScriptable")]
public class RecipieScriptable : ScriptableObject
{
    [SerializeField] public List<IngredientEntry> ingredients;
    [SerializeField] public ItemScriptableObject resultItem;
    public ItemScriptableObject requiredStation;

    // Optional: Build a dictionary at runtime if you need fast lookup
    public Dictionary<ItemScriptableObject, int> ToDictionary()
    {
        Dictionary<ItemScriptableObject, int> dict = new Dictionary<ItemScriptableObject, int>();
        foreach (var entry in ingredients)
        {
            if (entry != null && entry.item != null)
                dict[entry.item] = entry.amount;
        }
        return dict;
    }
}

[System.Serializable]
public class IngredientEntry
{
    public ItemScriptableObject item;
    public int amount;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public List<RecipieScriptable> allRecipes;
}

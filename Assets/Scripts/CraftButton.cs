using UnityEngine;

public class CraftButton : MonoBehaviour
{
    private CraftingSystem craftingSystem;
    private RecipieScriptable recipe;

    void Awake()
    {
        if (craftingSystem == null)
            craftingSystem = Object.FindFirstObjectByType<CraftingSystem>();
    }

    public void SetRecipe(RecipieScriptable newRecipe)
    {
        recipe = newRecipe;
    }


    public void OnCraftPressed()
    {
        bool success = craftingSystem.TryCraft(recipe);
        if (success)
        {
            Debug.Log("Crafting succeeded!");
        }
        else
        {
            Debug.Log("Crafting failed!");
        }
    }
}

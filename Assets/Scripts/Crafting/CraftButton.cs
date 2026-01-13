using UnityEngine;

public class CraftButton : MonoBehaviour
{
    private CraftingSystem craftingSystem;
    private RecipeScriptable recipe;

    void Awake()
    {
        if (craftingSystem == null)
            craftingSystem = Object.FindFirstObjectByType<CraftingSystem>();
    }

    public void SetRecipe(RecipeScriptable newRecipe)
    {
        recipe = newRecipe;
    }


    public void OnCraftPressed()
    {
        Debug.Log("Craft button pressed for recipe: " + (recipe != null ? recipe.name : "null"));

        bool success = craftingSystem.TryCraft(recipe);
        if (success)
        {
            Debug.Log("Crafting succeeded!");
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("success_craft");
        }
        else
        {
            Debug.Log("Crafting failed!");
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("fail_craft");
        }
    }
}

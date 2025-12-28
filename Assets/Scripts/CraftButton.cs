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
        bool success = craftingSystem.TryCraft(recipe);
        if (success)
        {
            Debug.Log("Crafting succeeded!");
            SoundManager.Instance.PlaySFX("success_craft");
        }
        else
        {
            Debug.Log("Crafting failed!");
            SoundManager.Instance.PlaySFX("fail_craft");
        }
    }
}

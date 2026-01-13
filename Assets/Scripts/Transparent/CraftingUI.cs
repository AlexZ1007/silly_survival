using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public RecipeDatabase recipeDatabase;

    [Header("UI")]
    public Transform recipeListParent;      // Where recipe prefabs appear
    public GameObject fullRecipePrefab;     // The UI element that displays a recipe

    private void Start()
    {
        // Listen for inventory changes
        playerInventory.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        // Safety — avoid leaks
        playerInventory.OnInventoryChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearRecipeUI();

        // ⭐ You already have this in PlayerInventory
        List<RecipeScriptable> visibleRecipes = playerInventory.GetVisibleRecipes(recipeDatabase);

        foreach (RecipeScriptable recipe in visibleRecipes)
        {
            GameObject obj = Instantiate(fullRecipePrefab, recipeListParent);

            RecipeDisplay ui = obj.GetComponent<RecipeDisplay>();
            if (ui != null)
            {
                ui.Initialize(recipe);
            }
            else
            {
                Debug.LogError("FullRecipePrefab is missing a FullRecipeUI component!");
            }
        }
    }

    private void ClearRecipeUI()
    {
        foreach (Transform child in recipeListParent)
        {
            Destroy(child.gameObject);
        }
    }
}

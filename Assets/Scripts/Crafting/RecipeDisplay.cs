using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{
    [Header("Ingredient Slots (Order Matters)")]
    public IngredientSlotUI[] ingredientSlots;

    [Header("Result Button")]
    public CraftButton resultButton;
    public Image resultButtonImage;

    [Header("Recipe To Show")]
    public RecipeScriptable recipeToDisplay;


    public void Initialize(RecipeScriptable recipe)
    {
        recipeToDisplay = recipe;
        DisplayRecipe(recipe);

        if (resultButton != null)
            resultButton.SetRecipe(recipe);
    }


    public void DisplayRecipe(RecipeScriptable recipe)
    {
        // Hide all ingredient slots first
        foreach (var slot in ingredientSlots)
            slot.gameObject.SetActive(false);

        // Fill visible slots
        for (int i = 0; i < recipe.ingredients.Count && i < ingredientSlots.Length; i++)
        {
            var entry = recipe.ingredients[i];
            var uiSlot = ingredientSlots[i];

            uiSlot.icon.sprite = entry.item.icon;
            uiSlot.count.text = entry.amount.ToString();
            uiSlot.gameObject.SetActive(true);
        }

        // Set result icon
        if (recipe.resultItem != null && recipe.resultItem.icon != null)
        {
            resultButtonImage.sprite = recipe.resultItem.icon;
            resultButtonImage.enabled = true;
        }

        // Remove any default TMP text on button
        TMP_Text tmp = resultButton.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = "";
    }
}

[System.Serializable]
public class IngredientSlotUI
{
    public GameObject gameObject;
    public Image icon;
    public TMP_Text count;
}

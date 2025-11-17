using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{
    [Header("Ingredient Slots (Order Matters)")]
    public IngredientSlotUI[] ingredientSlots;   // 3 slots in order

    [Header("Result Button")]
    public CraftButton resultButton;                 // Button object
    public Image resultButtonImage;             // The Image component on the button

    [Header("Recipe To Show")]
    public RecipieScriptable recipeToDisplay;




    void Start()
    {
        if (recipeToDisplay != null)
        {
            DisplayRecipe(recipeToDisplay);
            if (resultButton != null)
                resultButton.SetRecipe(recipeToDisplay);
        }

    }

    public void DisplayRecipe(RecipieScriptable recipe)
    {
        // Hide all ingredient slots first
        foreach (var slot in ingredientSlots)
            slot.gameObject.SetActive(false);

        // Fill slots based on how many ingredients exist
        for (int i = 0; i < recipe.ingredients.Count && i < ingredientSlots.Length; i++)
        {
            var entry = recipe.ingredients[i];
            var uiSlot = ingredientSlots[i];

            uiSlot.icon.sprite = entry.item.icon;
            uiSlot.count.text = entry.amount.ToString();
            uiSlot.gameObject.SetActive(true);
        }

        // --- Set the button icon ---
        if (recipe.resultItem != null && recipe.resultItem.icon != null)
        {
            resultButtonImage.sprite = recipe.resultItem.icon;
            resultButtonImage.enabled = true;
        }

        // Remove text if there's a TMP component on the button
        TMP_Text tmp = resultButton.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = "";
    }
}

[System.Serializable]
public class IngredientSlotUI
{
    public GameObject gameObject;   // Ingredient1, Ingredient2...
    public Image icon;              // Image child
    public TMP_Text count;          // Count TMP text
}

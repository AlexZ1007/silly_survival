using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;

    [Header("Items")]
    [SerializeField] private ItemScriptableObject woodItem;
    [SerializeField] private ItemScriptableObject stoneItem;

    [Header("Resource UI")]
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;

    private void Start()
    {
        inventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        inventory.OnInventoryChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        woodText.text = inventory.GetAmount(woodItem).ToString();
        stoneText.text = inventory.GetAmount(stoneItem).ToString();
    }
}

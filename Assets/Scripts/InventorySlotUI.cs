using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void SetItem(ItemScriptableObject item, int amount)
    {
        if (item == null || amount <= 0)
        {
            icon.enabled = false;
            countText.text = "";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = item.icon;
            countText.text = amount > 0 ? amount.ToString() : "";
        }
    }
}

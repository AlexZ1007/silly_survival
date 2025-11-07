using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemScriptableObject itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddItem(itemData, 1);

                Debug.Log($"<color=green>PICKED UP: {itemData.itemName} x 1</color>");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player entered trigger but has no PlayerInventory component!");
            }

        }

    }
}

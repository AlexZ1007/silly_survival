using UnityEngine;

public class PooledItemPickup : MonoBehaviour
{
    public ItemScriptableObject itemData;

    private PrefabPoolManager ownerPool;

    // Called by the pool when this item is spawned/reused
    public void Init(PrefabPoolManager pool, Vector3 position, Quaternion rotation)
    {
        ownerPool = pool;
        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("Player entered trigger but has no PlayerInventory component!");
            return;
        }

        if(inventory.TryAddItem(itemData, 1) == false)
        {
            //Debug.LogWarning("Could not add item to inventory. Inventory may be full.");
            return;
        }


        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("item_pickup");

        Debug.Log($"<color=green>PICKED UP: {itemData.itemName} x 1</color>");

        // RETURN TO POOL INSTEAD OF DESTROY
        if(ownerPool != null)
            ownerPool.Return(this);
        else
            Destroy(gameObject);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public PooledItemPickup prefab;
        public int prewarm = 20;
    }

    public List<PoolEntry> poolsToCreate = new();

    // Key: prefab reference
    private readonly Dictionary<PooledItemPickup, Queue<PooledItemPickup>> pools = new();
    // Key: spawned instance -> its prefab key
    private readonly Dictionary<PooledItemPickup, PooledItemPickup> instanceToPrefab = new();

    private void Awake()
    {
        foreach (var entry in poolsToCreate)
        {
            if (entry.prefab == null) continue;

            if (!pools.ContainsKey(entry.prefab))
                pools[entry.prefab] = new Queue<PooledItemPickup>();

            for (int i = 0; i < entry.prewarm; i++)
                CreateAndEnqueue(entry.prefab);
        }
    }

    private void CreateAndEnqueue(PooledItemPickup prefab)
    {
        var item = Instantiate(prefab, transform);
        item.gameObject.SetActive(false);
        instanceToPrefab[item] = prefab;
        pools[prefab].Enqueue(item);
    }

    public GameObject Spawn(GameObject prefabGO, Vector3 position, Quaternion rotation)
    {
        var prefab = prefabGO.GetComponent<PooledItemPickup>();
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab '{prefabGO.name}' has no PooledItemPickup. Falling back to Instantiate.");
            return Instantiate(prefabGO, position, rotation);
        }

        if (!pools.ContainsKey(prefab))
        {
            // auto-create pool if you forgot to add it in inspector
            pools[prefab] = new Queue<PooledItemPickup>();
            // optional: prewarm a little
            for (int i = 0; i < 5; i++)
                CreateAndEnqueue(prefab);
        }

        if (pools[prefab].Count == 0)
            CreateAndEnqueue(prefab);

        var item = pools[prefab].Dequeue();
        item.Init(this, position, rotation);
        return item.gameObject;
    }

    public void Return(PooledItemPickup instance)
    {
        if (instance == null) return;

        instance.gameObject.SetActive(false);

        if (!instanceToPrefab.TryGetValue(instance, out var prefabKey))
        {
            // Shouldn’t happen, but keep safe.
            Destroy(instance.gameObject);
            return;
        }

        pools[prefabKey].Enqueue(instance);
    }
}

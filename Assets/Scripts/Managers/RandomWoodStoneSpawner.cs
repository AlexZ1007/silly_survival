using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns WOOD and STONE pickups around the player at the start of the game
/// and over time, using object pooling.
/// 
/// Rules:
/// - Spawn 2–3 wood and 2–3 stone at the beginning
/// - Then spawn 1 of each every 30 seconds
/// - After phase 1 ends, spawn 1 of each every 60 seconds
/// - Never allow more than 3 of each active at the same time
/// </summary>
public class RandomWoodStoneSpawner : MonoBehaviour
{
    // ------------------------------
    // REFERENCES
    // ------------------------------

    [Header("References")]
    [Tooltip("PrefabPoolManager that handles pooled spawning")]
    public PrefabPoolManager pool;

    [Tooltip("Player transform – used as center for spawn area")]
    public Transform player;

    // ------------------------------
    // PREFABS
    // ------------------------------

    [Header("Prefabs (must have PooledItemPickup)")]
    [Tooltip("Wood pickup prefab")]
    public GameObject woodPrefab;

    [Tooltip("Stone pickup prefab")]
    public GameObject stonePrefab;

    // ------------------------------
    // LIMITS
    // ------------------------------

    [Header("Limits")]
    [Tooltip("Maximum wood pickups allowed at one time")]
    public int maxWoodAlive = 3;

    [Tooltip("Maximum stone pickups allowed at one time")]
    public int maxStoneAlive = 3;

    // ------------------------------
    // INITIAL SPAWN
    // ------------------------------

    [Header("Initial Spawn")]
    [Tooltip("Minimum amount spawned at start")]
    public int initialMin = 2;

    [Tooltip("Maximum amount spawned at start")]
    public int initialMax = 3;

    // ------------------------------
    // TIMING
    // ------------------------------

    [Header("Timing")]
    [Tooltip("Spawn interval during phase 1 (seconds)")]
    public float phase1Interval = 30f;

    [Tooltip("How long phase 1 lasts (seconds)")]
    public float phase1Duration = 180f;

    [Tooltip("Spawn interval after phase 1 (seconds)")]
    public float phase2Interval = 60f;

    // ------------------------------
    // SPAWN AREA
    // ------------------------------

    [Header("Spawn Area")]
    [Tooltip("Minimum distance from player")]
    public float innerRadius = 6f;

    [Tooltip("Maximum distance from player")]
    public float outerRadius = 22f;

    [Tooltip("Attempts to find a valid spawn point")]
    public int maxTries = 25;

    // ------------------------------
    // GROUND CHECKING
    // ------------------------------

    [Header("Grounding")]
    [Tooltip("Layer mask used to detect ground")]
    public LayerMask groundMask;

    [Tooltip("Raycast start height")]
    public float rayStartHeight = 40f;

    [Tooltip("Offset above ground to place pickup")]
    public float placeAboveGround = 0.15f;

    [Tooltip("Reject surfaces steeper than this angle")]
    public float maxSlopeAngle = 45f;

    // ------------------------------
    // INTERNAL STATE
    // ------------------------------

    // Track active wood and stone objects so we never exceed maxAlive
    readonly HashSet<GameObject> activeWood = new();
    readonly HashSet<GameObject> activeStone = new();

    // ------------------------------
    // UNITY LIFECYCLE
    // ------------------------------

    void Start()
    {
        // Validate required references early
        if (pool == null || player == null || woodPrefab == null || stonePrefab == null)
        {
            Debug.LogError("RandomWoodStoneSpawner: Missing required references.");
            enabled = false;
            return;
        }

        // Initial spawn at game start
        SpawnInitial(woodPrefab, activeWood, maxWoodAlive);
        SpawnInitial(stonePrefab, activeStone, maxStoneAlive);

        // Begin timed spawning logic
        StartCoroutine(SpawnLoop());
    }

    // ------------------------------
    // INITIAL SPAWN LOGIC
    // ------------------------------

    /// <summary>
    /// Spawns 2–3 items at start, capped by maxAlive.
    /// </summary>
    void SpawnInitial(GameObject prefab, HashSet<GameObject> activeSet, int maxAlive)
    {
        int count = Random.Range(initialMin, initialMax + 1);
        count = Mathf.Min(count, maxAlive);

        for (int i = 0; i < count; i++)
            TrySpawn(prefab, activeSet, maxAlive);
    }

    // ------------------------------
    // TIMED SPAWN LOOP
    // ------------------------------

    /// <summary>
    /// Handles phase-based timed spawning.
    /// </summary>
    IEnumerator SpawnLoop()
    {
        float elapsed = 0f;

        // PHASE 1 — faster spawning (every 30s)
        while (elapsed < phase1Duration)
        {
            yield return new WaitForSeconds(phase1Interval);
            elapsed += phase1Interval;

            TrySpawn(woodPrefab, activeWood, maxWoodAlive);
            TrySpawn(stonePrefab, activeStone, maxStoneAlive);
        }

        // PHASE 2 — slower spawning (every 60s)
        while (true)
        {
            yield return new WaitForSeconds(phase2Interval);

            TrySpawn(woodPrefab, activeWood, maxWoodAlive);
            TrySpawn(stonePrefab, activeStone, maxStoneAlive);
        }
    }

    // ------------------------------
    // SPAWN ATTEMPT
    // ------------------------------

    /// <summary>
    /// Attempts to spawn one pickup of the given type,
    /// respecting maxAlive and valid ground placement.
    /// </summary>
    void TrySpawn(GameObject prefab, HashSet<GameObject> activeSet, int maxAlive)
    {
        Debug.Log("Attempting to spawn " + prefab.name);

        // Remove entries that were picked up / returned to pool
        activeSet.RemoveWhere(go => go == null || !go.activeInHierarchy);

        // Do not exceed the allowed active count
        if (activeSet.Count >= maxAlive)
            return;

        // Find a valid grounded position
        if (!TryGetSpawnPoint(out Vector3 pos))
            return;

        // Spawn from pool
        GameObject obj = pool.Spawn(prefab, pos, Quaternion.identity);
        if (obj != null)
            activeSet.Add(obj);
    }

    // ------------------------------
    // SPAWN POSITION FINDER
    // ------------------------------

    /// <summary>
    /// Finds a valid ground position around the player.
    /// </summary>
    bool TryGetSpawnPoint(out Vector3 groundedPos)
    {
        groundedPos = default;

        for (int i = 0; i < maxTries; i++)
        {
            // Random direction & distance around player
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(innerRadius, outerRadius);
            Vector3 candidate = player.position + new Vector3(dir.x, 0f, dir.y) * dist;

            // Raycast downward to find ground
            Vector3 rayStart = candidate + Vector3.up * rayStartHeight;
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayStartHeight * 2f, groundMask))
            {
                // Reject steep slopes
                if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                    continue;

                groundedPos = hit.point + Vector3.up * placeAboveGround;
                return true;
            }
        }

        return false;
    }
}

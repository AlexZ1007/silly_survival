using UnityEngine;

public class PlayerMine : MonoBehaviour
{
    [Header("Mining Settings")]
    public GameObject stonePrefab;       // prefab to spawn after mining
    public float interactRange = 2f;     // how close the player must be
    public LayerMask rockLayerMask;      // optional: layer for rocks
    public string rockTag = "Rock";      // or use tag

    [Header("Mining Timing")]
    public float mineHoldTime = 3f;      // how long to hold key
    private float mineTimer = 0f;
    private bool isChopping = false;
    private Collider targetRock;

    [Header("Input Key")]
    public KeyCode mineKey = KeyCode.C;  // press & hold this key to mine

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool holdingMineKey = Input.GetKey(mineKey);
        animator?.SetBool("IsCPressed", holdingMineKey); // optional animation parameter

        if (holdingMineKey)
        {
            // Start or continue mining
            if (!isChopping)
            {
                targetRock = FindNearestRock();
                if (targetRock != null)
                {
                    isChopping = true;
                    mineTimer = 0f;
                    Debug.Log("Started mining...");
                }
            }

            if (isChopping)
            {
                mineTimer += Time.deltaTime;

                if (mineTimer >= mineHoldTime)
                {
                    MineRock(targetRock);
                    ResetMining();
                }
            }
        }
        else
        {
            // Released key early -> cancel mining
            if (isChopping)
            {
                Debug.Log("Mining cancelled — released key too soon.");
                ResetMining();
            }
        }
    }

    private void ResetMining()
    {
        isChopping = false;
        mineTimer = 0f;
        targetRock = null;
    }

    private Collider FindNearestRock()
    {
        Collider[] hits;

        if (rockLayerMask != 0)
            hits = Physics.OverlapSphere(transform.position, interactRange, rockLayerMask);
        else
            hits = Physics.OverlapSphere(transform.position, interactRange);

        Collider nearestRock = null;
        float nearestSqr = float.MaxValue;

        foreach (var col in hits)
        {
            if (col == null) continue;

            if (!string.IsNullOrEmpty(rockTag) && !col.CompareTag(rockTag))
                continue;

            float sqr = (col.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearestRock = col;
            }
        }

        return nearestRock;
    }

    private void MineRock(Collider rock)
    {
        if (rock == null)
        {
            Debug.Log("No rock to mine.");
            return;
        }

        Vector3 spawnPos = rock.transform.position;
        Quaternion spawnRot = rock.transform.rotation;

        if (stonePrefab != null)
            Instantiate(stonePrefab, spawnPos, spawnRot);
        else
            Debug.LogWarning("Stone prefab not assigned on PlayerMine.");

        Destroy(rock.transform.root.gameObject);
        Debug.Log("Rock mined successfully after holding key!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}

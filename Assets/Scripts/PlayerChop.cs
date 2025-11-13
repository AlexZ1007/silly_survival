using UnityEngine;

public class PlayerChop : MonoBehaviour
{
    [Header("Chop Settings")]
    public GameObject logPrefab;
    public float interactRange = 2f;
    public LayerMask treeLayerMask;
    public string treeTag = "Tree";

    [Header("Chop Timing")]
    public float chopHoldTime = 3f; // how long you must hold C
    private float chopTimer = 0f;
    private bool isChopping = false;

    private Animator animator;
    private Collider targetTree;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool holdingC = Input.GetKey(KeyCode.C);
        animator?.SetBool("IsCPressed", holdingC);

        if (holdingC)
        {
            // Start or continue chopping
            if (!isChopping)
            {
                targetTree = FindNearestTree();
                if (targetTree != null)
                {
                    isChopping = true;
                    chopTimer = 0f;
                    Debug.Log("Started chopping...");
                }
            }

            if (isChopping)
            {
                chopTimer += Time.deltaTime;

                // Optional: show progress bar, play animation, etc.
                if (chopTimer >= chopHoldTime)
                {
                    ChopTree(targetTree);
                    ResetChop();
                }
            }
        }
        else
        {
            // Released the key early -> cancel chop
            if (isChopping)
            {
                Debug.Log("Chop cancelled — released C too soon.");
                ResetChop();
            }
        }
    }

    private void ResetChop()
    {
        isChopping = false;
        chopTimer = 0f;
        targetTree = null;
    }

    private Collider FindNearestTree()
    {
        Collider[] hits;

        if (treeLayerMask != 0)
            hits = Physics.OverlapSphere(transform.position, interactRange, treeLayerMask);
        else
            hits = Physics.OverlapSphere(transform.position, interactRange);

        Collider nearestTree = null;
        float nearestSqr = float.MaxValue;

        foreach (var col in hits)
        {
            if (col == null) continue;

            if (!string.IsNullOrEmpty(treeTag) && !col.CompareTag(treeTag))
                continue;

            float sqr = (col.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearestTree = col;
            }
        }

        return nearestTree;
    }

    private void ChopTree(Collider tree)
    {
        if (tree == null)
        {
            Debug.Log("No tree to chop.");
            return;
        }

        Vector3 spawnPos = tree.transform.position;
        Quaternion spawnRot = tree.transform.rotation;

        if (logPrefab != null)
            Instantiate(logPrefab, spawnPos, spawnRot);
        else
            Debug.LogWarning("Log prefab not assigned on PlayerChop.");

        Destroy(tree.transform.root.gameObject);
        Debug.Log("Tree chopped successfully after hold!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}


using System.Collections;
using UnityEngine;

public class PlayerMine : MonoBehaviour
{
    [Header("Settings")]
    public GameObject stonePrefab;
    public float interactRange = 2f;
    public LayerMask rockLayerMask;
    public string rockTag = "Rock";

    [Header("Timing")]
    public float holdTime = 3f; // time to hold key
    private float timer = 0f;
    private bool isWorking = false;
    private Collider target;

    [Header("Input")]
    public KeyCode actionKey = KeyCode.C;

    [Header("Respawn")]
    public float respawnTime = 1800f; // time until rock respawns

    private Animator animator;

    public WeaponSwitcher weaponSwitcher;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool holdingKey = Input.GetKey(actionKey);
        animator?.SetBool("IsCPressed", holdingKey);

        if (holdingKey)
        {
            if (!isWorking)
            {
                target = FindNearestTarget();
                if (target != null)
                {
                    isWorking = true;
                    timer = 0f;
                    Debug.Log("Started mining...");
                }
            }

            if (isWorking)
            {
                timer += Time.deltaTime;
                if (timer >= holdTime)
                {
                    PerformAction(target);
                    ResetAction();
                }
            }
        }
        else
        {
            if (isWorking)
            {
                Debug.Log("Mining cancelled — released key too soon.");
                ResetAction();
            }
        }
    }

    private void ResetAction()
    {
        isWorking = false;
        timer = 0f;
        target = null;
    }

    private Collider FindNearestTarget()
    {
        Collider[] hits = rockLayerMask != 0
            ? Physics.OverlapSphere(transform.position, interactRange, rockLayerMask)
            : Physics.OverlapSphere(transform.position, interactRange);

        Collider nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var col in hits)
        {
            if (col == null) continue;
            if (!string.IsNullOrEmpty(rockTag) && !col.CompareTag(rockTag)) continue;

            float sqr = (col.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = col;
            }
        }

        return nearest;
    }

    private void PerformAction(Collider target)
    {
        if (weaponSwitcher != null && weaponSwitcher.IsUsingAxe)
        {
            Debug.Log("You need a pickaxe to mine!");
            return; // exit if wrong tool
        }

        if (target == null)
        {
            Debug.Log("No rock to mine.");
            return;
        }

        if (stonePrefab != null)
            Instantiate(stonePrefab, target.transform.position, target.transform.rotation);
        else
            Debug.LogWarning("Stone prefab not assigned on PlayerMine.");

        StartCoroutine(Respawn(target.gameObject, respawnTime));
    }


    private IEnumerator Respawn(GameObject obj, float delay)
    {
        obj.SetActive(false);            // hide the rock
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);             // show the rock again
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static InteractableAction;

public class PlayerAction : MonoBehaviour
{
    [Header("Input")]//key used to perform the action
    public KeyCode actionKey = KeyCode.C;

    [Header("Hold Settings")]
    public float holdHitInterval = 0.6f; // time between hits when holding


    [Header("Reference")]//reference to weapon Swithcer system
    public WeaponSwitcher weaponSwitcher;

    [Header("Interaction")]//how far the player can interact
    public float interactionRadius = 3f;

    private Collider cachedTarget;
    private Resource cachedResource;
    private Animator animator;//animator reference
    private InteractableAction action; // dynamic action based on object interacted with

    [Header("UI")]
    public ProgressBar progressBar; // drag your UI ProgressBar here in the inspector

    private Resource hoveredResource;
    private ProgressBar hoveredBar;

    private float nextAllowedHitTime;
    private bool isHolding;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleProximityUI();

        bool pressed = Input.GetKeyDown(actionKey);
        bool held = Input.GetKey(actionKey);

        if (pressed)
            TryStartHit();

        if (held && Time.time >= nextAllowedHitTime)
            TryStartHit();
    }
    private void TryStartHit()
    {
        cachedTarget = FindNearestTarget();
        if (cachedTarget == null) return;

        cachedResource = cachedTarget.GetComponent<Resource>();
        if (cachedResource == null) return;

        action = cachedResource.actionRequired;
        if (action == null) return;

        float damage = GetToolDamage();

        UpdateProgressBar(cachedResource);

        if (damage <= 0f)
            return;


        // Start attack
        nextAllowedHitTime = Time.time + holdHitInterval;
        animator.SetTrigger("Hit");
    }


    private void HandleProximityUI()
    {
        Collider nearest = FindNearestTarget();

        // No resource nearby -> hide current bar
        if (nearest == null)
        {
            HideHoveredBar();
            return;
        }

        Resource resource = nearest.GetComponent<Resource>();
        if (resource == null || resource.actionRequired == null)
        {
            HideHoveredBar();
            return;
        }

        // Same resource -> just update color
        if (hoveredResource == resource)
        {
            UpdateProgressBar(resource);
            return;
        }

        // New resource -> switch bar
        HideHoveredBar();

        hoveredResource = resource;

        var bars = resource.GetComponentsInChildren<ProgressBar>(true);
        hoveredBar = bars.Length > 0 ? bars[0] : null;

        if (hoveredBar == null) return;

        progressBar = hoveredBar;

        hoveredBar.Show();
        UpdateProgressBar(resource);
    }

    private void HideHoveredBar()
    {
        if (hoveredBar != null)
            hoveredBar.Hide();

        hoveredBar = null;
        hoveredResource = null;
    }

    //find the nearest interactable resourcce inside the radius
    private Collider FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);
        Collider nearest = null;
        float nearestSqr = float.MaxValue;

        //loop through all colliders in range
        foreach (var col in hits)
        {
            var resource = col.GetComponent<Resource>();
            if (resource == null) continue;

            float sqr = (col.transform.position - transform.position).sqrMagnitude;//calculate squared distance
            if (sqr < nearestSqr)//keep the closest one
            {
                nearestSqr = sqr;
                nearest = col;
            }
        }

        return nearest;
    }

    //execute the action (spawn drops + respawn)
    private void PerformAction(Collider target)
    {
        if (target == null) return;

        if (action.drops != null && action.drops.Length > 0)
        {
            Vector3 lineDirection = Vector3.forward; // line along world forward
            float spacing = 1f; // distance between drops

            for (int i = 0; i < action.drops.Length; i++)
            {
                var dropPrefab = action.drops[i];
                if (dropPrefab == null) continue;

                // Position along a straight line in front of the resource
                Vector3 spawnPos = target.transform.position + lineDirection * i * spacing;

                // Keep drops slightly above ground
                spawnPos.y = 5f;

                // Avoid overlapping with other objects
                spawnPos = FindFreeDropPosition(spawnPos, 0.5f, 0.3f);

                Instantiate(dropPrefab, spawnPos, Quaternion.identity);
            }
        }

        StartCoroutine(Respawn(target.gameObject, action.respawnTime));
    }

    private float GetToolDamage()
    {
        WeaponType currentTool = weaponSwitcher.CurrentWeaponType;

        foreach (var req in action.toolRequirements)
        {
            if (req.tool == currentTool)
                return req.damage;
        }

        return 0f; // wrong tool
    }

    public void ApplyDamage()
    {
        if (cachedResource == null) return;

        // Player moved too far away before hit
        if (Vector3.Distance(transform.position, cachedResource.transform.position) > interactionRadius)
            return;

        float damage = GetToolDamage();
        if (damage <= 0f) return;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("chop");

        cachedResource.currentHealth -= damage;
        cachedResource.currentHealth = Mathf.Max(0f, cachedResource.currentHealth);

        UpdateProgressBar(cachedResource);

        var chicken = cachedResource.GetComponent<ChickenWander>();
        if (chicken != null)
        {
            chicken.FleeFrom(transform.position);
        }


        if (cachedResource.currentHealth <= 0f)
        {
            PerformAction(cachedTarget);
            ClearCachedTarget();
        }
    }

    private void UpdateProgressBar(Resource resource)
    {
        if (resource == null || resource.actionRequired == null) return;

        // Get bar once
        if (progressBar == null)
        {
            var bars = resource.GetComponentsInChildren<ProgressBar>(true);
            if (bars.Length == 0) return;
            progressBar = bars[0];
        }

        progressBar.Show();

        float progress = resource.currentHealth / resource.actionRequired.maxHealth;

        // Tool only affects color, NOT visibility
        bool hasCorrectTool = HasCorrectTool(resource.actionRequired);

        progressBar.SetProgress(progress, hasCorrectTool);
    }

    private bool HasCorrectTool(InteractableAction action)
    {
        WeaponType currentTool = weaponSwitcher.CurrentWeaponType;

        foreach (var req in action.toolRequirements)
        {
            if (req.tool == currentTool)
                return true;
        }

        return false;
    }



    private void ClearCachedTarget()
    {
        cachedTarget = null;
        cachedResource = null;
        action = null;
    }


    // Respawn the resource object after delay
    private IEnumerator Respawn(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);

        Resource res = obj.GetComponent<Resource>();
        if (res != null)
            res.ResetHealth();

        obj.SetActive(true);
    }


    // finds an empty position nearby so drops don’t overlap with objects
    private Vector3 FindFreeDropPosition(Vector3 center, float radius, float checkRadius, int maxAttempts = 20)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            Vector3 candidate = center + offset;

            if (!Physics.CheckSphere(candidate, checkRadius))
                return candidate;
        }

        return center;
    }

}
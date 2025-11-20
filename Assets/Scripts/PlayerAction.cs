using System.Collections;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [Header("Input")]//key used to perform the action
    public KeyCode actionKey = KeyCode.C;

    [Header("Reference")]//reference to weapon Swithcer system
    public WeaponSwitcher weaponSwitcher;

    [Header("Interaction")]//how far the player can interact
    public float interactionRadius = 3f;


    private float timer = 0f;//counts hold time
    private bool isWorking = false;//bool if player is currently perfoming the action
    private Collider target;//resource being interacted with
    private Animator animator;

    private InteractableAction action; // dynamic action based on object interacted with

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool holdingKey = Input.GetKey(actionKey);
        animator?.SetBool("IsCPressed", holdingKey);

        if (holdingKey)//when holding the action key
        {
            if (!isWorking)
            {
                target = FindNearestTarget();//find nearest resource

                if (target != null)
                {
                    var resource = target.GetComponent<Resource>();
                    if (resource == null) return;

                    action = resource.actionRequired;   //  get required action from the resource

                    if (action == null)
                    {
                        Debug.LogWarning("Resource has NO InteractableAction assigned!");
                        return;
                    }

                    //check if the player has the required tool
                    if (!HasRequiredTool())
                    {
                        string tools = string.Join(", ", action.allowedTools);
                        Debug.Log($"You need one of these tools to {action.actionName}: {tools}");
                        return;
                    }

                    //start action
                    isWorking = true;
                    timer = 0f;

                    Debug.Log($"Started {action.actionName.ToLower()}...");
                }
            }

            //continue action->increase timer
            if (isWorking)
            {
                timer += Time.deltaTime;
                if (timer >= action.holdTime)//action complete
                {
                    PerformAction(target);
                    ResetAction();
                }
            }
        }
        else
        {
            //player released key too early
            if (isWorking)
            {
                Debug.Log($"{action.actionName} cancelled — released key too soon.");
                ResetAction();
            }
        }
    }

    private bool HasRequiredTool()
    {
        if (action == null) return false;

        if (action.allowedTools == null || action.allowedTools.Length == 0)
            return false;//if nothing is specified, nothing is allowed

        foreach (var tool in action.allowedTools)
        {
            switch (tool)
            {
                case InteractableAction.WeaponType.Axe:
                    if (weaponSwitcher.IsUsingAxe) return true;
                    break;
                case InteractableAction.WeaponType.Pickaxe:
                    if (weaponSwitcher.IsUsingPickaxe) return true;
                    break;
                case InteractableAction.WeaponType.Shovel:
                    if (weaponSwitcher.IsUsingShovel) return true;
                    break;
                case InteractableAction.WeaponType.Weapon:
                    if (weaponSwitcher.IsUsingWeapon) return true;
                    break;
                case InteractableAction.WeaponType.None:
                    return true;
            }
        }

        return false;
    }


    private void ResetAction()
    {
        isWorking = false;
        timer = 0f;
        target = null;
        action = null;
    }

    private Collider FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);
        Collider nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var col in hits)
        {
            var resource = col.GetComponent<Resource>();
            if (resource == null) continue;

            float sqr = (col.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestSqr)
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



    private IEnumerator Respawn(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }

    // Simple method to check for free positions around a center
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

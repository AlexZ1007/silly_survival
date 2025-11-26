using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static InteractableAction;

public class PlayerAction : MonoBehaviour
{
    [Header("Input")]//key used to perform the action
    public KeyCode actionKey = KeyCode.C;

    [Header("Reference")]//reference to weapon Swithcer system
    public WeaponSwitcher weaponSwitcher;

    [Header("Interaction")]//how far the player can interact
    public float interactionRadius = 3f;

    [Header("UI")]
    public ProgressBar progressBar; // drag your UI ProgressBar here in the inspector



    private float timer = 0f;//counts hold time
    private bool isWorking = false;//bool if player is currently perfoming the action
    private Collider target;//resource being interacted with
    private Animator animator;//animator reference

    private InteractableAction action; // dynamic action based on object interacted with

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool holdingKey = Input.GetKey(actionKey);//track if the key is pressed
        animator?.SetBool("IsCPressed", holdingKey);//bool for animation

        //1. NOT WORKING -> try to start action 
        if (!isWorking)
        {
            if (!holdingKey) return;  // must press C to start ANY action

            target = FindNearestTarget();//find nearest target in range
            if (target == null) return;

            var resource = target.GetComponent<Resource>();//get resource data
            if (resource == null) return;

            action = resource.actionRequired;//what action is required(chop, mine, etc)
            if (action == null) return;

            // get progress bar from object
            var bars = target.GetComponentsInChildren<ProgressBar>(true);
            progressBar = bars.Length > 0 ? bars[0] : null;
            if (progressBar == null) return;

            //show the progress bar and reset to 0
            progressBar.Show();
            progressBar.SetProgress(0f, true);

            //reset timer
            timer = 0f;

            //mark that the player is now performing the action
            isWorking = true;
            return;
        }

        // 2. WORKING -> performing action 
        if (isWorking)
        {
            // stop if key released
            if (!holdingKey)
            {
                Debug.Log($"{action.actionName} cancelled - released key too soon.");
                ResetAction();
                return;
            }

            // cancel if player leaves interaction radius
            if (target == null || Vector3.Distance(transform.position, target.transform.position) > interactionRadius)
            {
                Debug.Log($"{action.actionName} cancelled - moved too far from target.");
                ResetAction();
                return;
            }


            bool hasTool = HasRequiredTool();//check if the player has correct tool
            float requiredTime = GetCurrentHoldTime();//how long should the action take with current tool

            // increase timer
            timer += Time.deltaTime;

            // update bar
            float p = timer / requiredTime;
            progressBar.SetProgress(p, hasTool);

            // wrong tool -> bar stays red & full but won't complete
            if (!hasTool)
            {
                if (progressBar != null)
                    progressBar.SetProgress(1f, false);
                return;
            }

            // correct tool & full -> perform action
            if (timer >= requiredTime)
            {
                PerformAction(target);//spawn drops, disable object
                ResetAction();//hide Ui and clear variables
            }
        }
    }


    //ckeck if the currently tool is valid for the active action
    private bool HasRequiredTool()
    {
        if (action == null || action.toolRequirements == null) return false;

        WeaponType currentTool = weaponSwitcher.CurrentWeaponType;

        //loop through allowed tools defined in the action
        foreach (var req in action.toolRequirements)
        {
            if (req.tool == currentTool) return true;
        }

        return false;
    }



    //reset all state: stop action, hide UI, clear target and action
    private void ResetAction()
    {
        isWorking = false;
        timer = 0f;

        if (progressBar != null)
        {
            progressBar.Hide();
            progressBar.SetProgress(0f, false);
        }

        progressBar = null;
        target = null;
        action = null;
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

    private float GetCurrentHoldTime()
    {
        if (action == null || action.toolRequirements == null)
            return 0f;

        WeaponType currentTool = weaponSwitcher.CurrentWeaponType;

        foreach (var req in action.toolRequirements)//find matching tool requirement
        {
            if (req.tool == currentTool)
                return req.holdTime;
        }

        return float.MaxValue; // cannot complete if tool not allowed
    }



    // Respawn the resource object after delay
    private IEnumerator Respawn(GameObject obj, float delay)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(delay);
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
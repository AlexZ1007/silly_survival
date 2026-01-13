using System.Collections.Generic;
using UnityEngine;
using static InteractableAction;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("All Possible Tools (in player hand, can be disabled)")]
    public GameObject[] allTools; // drag all tool prefabs from player hand

    [Header("References")]
    public PlayerInventory playerInventory; // assign your PlayerInventory here
    public KeyCode switchKey = KeyCode.Z;   // key to switch tools

    private List<GameObject> ownedTools = new List<GameObject>(); // currently switchable tools
    private int currentIndex = 0; // current active tool index in ownedTools

    void Start()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        UpdateOwnedTools();
        ActivateTool(currentIndex);

        // Subscribe to inventory changes to update tools dynamically
        playerInventory.OnInventoryChanged += UpdateOwnedTools;
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && ownedTools.Count > 1)
        {
            currentIndex++;
            if (currentIndex >= ownedTools.Count)
                currentIndex = 0;

            ActivateTool(currentIndex);
        }
    }

    // Enable only the current tool
    void ActivateTool(int index)
    {
        for (int i = 0; i < allTools.Length; i++)
            allTools[i].SetActive(false);

        if (ownedTools.Count == 0) return;

        ownedTools[index].SetActive(true);
        Debug.Log("Switched to: " + ownedTools[index].name);
    }

    // Rebuild the list of owned tools based on the inventory
    void UpdateOwnedTools()
    {
        ownedTools.Clear();

        foreach (var toolObj in allTools)
        {
            var toolItem = toolObj.GetComponent<ToolItemHolder>();
            if (toolItem == null) continue;


            if (playerInventory.GetAmount(toolItem.item) > 0)
            {
                ownedTools.Add(toolObj);
            }
        }

        if (ownedTools.Count == 0)
        {
            currentIndex = 0;
            return;
        }

        if (currentIndex >= ownedTools.Count)
            currentIndex = 0;

        ActivateTool(currentIndex);
    }


    // Return currently active WeaponType for PlayerAction
    public WeaponType CurrentWeaponType
    {
        get
        {
            if (ownedTools.Count == 0) return WeaponType.None;

            var toolItem = ownedTools[currentIndex].GetComponent<ToolItemHolder>();
            return toolItem != null ? toolItem.weaponType : WeaponType.None;
        }
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= UpdateOwnedTools;
    }
}

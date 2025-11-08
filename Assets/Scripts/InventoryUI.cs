using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;


    private Canvas canvas;
    private bool isOpen = false;

    private List<InventorySlotUI> slots = new List<InventorySlotUI>();


    private void CreateSlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slot = newSlot.GetComponent<InventorySlotUI>();
            slots.Add(slot);
        }
    }


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        canvas = GetComponentInChildren<Canvas>(true);
        canvas.enabled = false; // Start hidden
    }

    private void Start()
    {
        // Initialize slots
        CreateSlots(24);

        inventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        int slotIndex = 0;
        foreach (var itemEntry in inventory.GetAllItems())
        {
            if (slotIndex >= slots.Count)
                break;
            slots[slotIndex].SetItem(itemEntry.Key, itemEntry.Value);
            slotIndex++;
        }
        // Clear remaining slots
        for (int i = slotIndex; i < slots.Count; i++)
        {
            slots[i].SetItem(null, 0);
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        canvas.enabled = isOpen;

        if (isOpen)
            UpdateUI(); // Refresh UI when opened
    }
}

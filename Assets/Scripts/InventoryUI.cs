using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private int numberOfSlots = 20;

    [Header("UI Panel (to hide/show)")]
    [SerializeField] private GameObject inventoryPanel;   // ← only this will be enabled/disabled

    private bool isOpen = false;

    private List<InventorySlotUI> slots = new List<InventorySlotUI>();


    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Start hidden
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        CreateSlots(numberOfSlots);

        inventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateUI;
    }

    private void CreateSlots(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slot = newSlot.GetComponent<InventorySlotUI>();
            slots.Add(slot);
        }
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

        // clear remaining
        for (int i = slotIndex; i < slots.Count; i++)
        {
            slots[i].SetItem(null, 0);
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        // Show/hide ONLY the inventory panel
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
            UpdateUI();
    }

   
}

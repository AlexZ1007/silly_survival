using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject axe;
    public GameObject pickaxe;
    public GameObject shovel;
    public GameObject weapon;

    private GameObject[] weapons;
    private int currentIndex = 0; // 0 = axe, 1 = pickaxe, 2 = shovel, 3 = weapon

    void Start()
    {
        // Create array of weapons in order
        weapons = new GameObject[] { axe, pickaxe, shovel, weapon };

        // Activate only the first one
        ActivateWeapon(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchWeapon();
        }
    }

    void SwitchWeapon()
    {
        currentIndex++;

        // Loop back when reaching the end
        if (currentIndex >= weapons.Length)
            currentIndex = 0;

        ActivateWeapon(currentIndex);

        Debug.Log("Switched to: " + weapons[currentIndex].name);
    }

    void ActivateWeapon(int index)
    {
        // Disable all weapons
        foreach (GameObject w in weapons)
            w.SetActive(false);

        // Enable the selected weapon
        weapons[index].SetActive(true);
    }

    // Optional: Quick checks for other scripts
    public bool IsUsingAxe => currentIndex == 0;
    public bool IsUsingPickaxe => currentIndex == 1;
    public bool IsUsingShovel => currentIndex == 2;
    public bool IsUsingWeapon => currentIndex == 3;
}

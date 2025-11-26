using UnityEngine;
using static InteractableAction;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]//assign all weapon prefabs in the inspector
    public GameObject axe1;
    public GameObject axe2;
    public GameObject axe3;
    public GameObject pickaxe1;
    public GameObject pickaxe2;
    public GameObject pickaxe3;
    public GameObject shovel1;
    public GameObject shovel2;
    public GameObject shovel3;
    public GameObject bat;
    public GameObject sword;

    private GameObject[] weapons;//array to store all wapons for easy switching
    private int currentIndex = 0;//index of currently active weapon

    void Start()
    {
        //fill array with all wapons
        weapons = new GameObject[] { axe1, axe2, axe3, pickaxe1, pickaxe2, pickaxe3, shovel1, shovel2, shovel3, bat, sword };
        ActivateWeapon(currentIndex);//first weapon
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SwitchWeapon();//check if z is pressed for switching
    }

    void SwitchWeapon()
    {
        currentIndex++;
        if (currentIndex >= weapons.Length) currentIndex = 0;//lopp back to first if out of range
        ActivateWeapon(currentIndex);//activate selected weapon
        Debug.Log("Switched to: " + weapons[currentIndex].name);
    }

    void ActivateWeapon(int index)//deactivate all weapons except the one at the given index
    {
        foreach (var w in weapons)
            w.SetActive(false);
        weapons[index].SetActive(true);
    }

    
    public WeaponType CurrentWeaponType//returns the type of the currently active weapon
    {
        get
        {
            switch (currentIndex)
            {
                case 0: return WeaponType.Axe1;
                case 1: return WeaponType.Axe2;
                case 2: return WeaponType.Axe3;
                case 3: return WeaponType.Pickaxe1;
                case 4: return WeaponType.Pickaxe2;
                case 5: return WeaponType.Pickaxe3;
                case 6: return WeaponType.Shovel1;
                case 7: return WeaponType.Shovel2;
                case 8: return WeaponType.Shovel3;
                case 9: return WeaponType.Bat;
                case 10: return WeaponType.Sword;
                default: return WeaponType.None;
            }
        }
    }
}

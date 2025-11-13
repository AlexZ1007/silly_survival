using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject axe;
    public GameObject pickaxe;

    [Header("State")]
    [SerializeField] private bool usingAxe = true; // private, but visible in Inspector

    // Public getter so other scripts can check which weapon is active
    public bool IsUsingAxe
    {
        get { return usingAxe; }
    }

    void Start()
    {
        // Start with axe active
        axe.SetActive(true);
        pickaxe.SetActive(false);
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
        usingAxe = !usingAxe;

        axe.SetActive(usingAxe);
        pickaxe.SetActive(!usingAxe);

        Debug.Log("Switched to " + (usingAxe ? "Axe" : "Pickaxe"));
    }
}

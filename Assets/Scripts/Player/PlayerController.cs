using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    private PlayerInventory inventory;
    private PlayerMovement movement;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        movement = GetComponent<PlayerMovement>();
    }


    // Function for saving player information
    public void SaveTo(SaveData data)
    {
        data.playerPosition = new float[]
        {
            transform.position.x,
            transform.position.y,
            transform.position.z
        };

        inventory.SaveTo(data);
    }


    // Function for loading player information
    public void LoadFrom(SaveData data)
    {
        if (data.playerPosition != null && data.playerPosition.Length == 3)
        {
            Vector3 loadedPosition = new Vector3(
                data.playerPosition[0],
                data.playerPosition[1],
                data.playerPosition[2]
            );
            movement.setRigidBody(loadedPosition);
        }

        inventory.LoadFrom(data);
    }
}

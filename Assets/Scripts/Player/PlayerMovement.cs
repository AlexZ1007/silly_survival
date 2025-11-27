using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    InputAction moveAction;
    InputAction inventoryAction;

    Vector2 moveRead;

    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private Rigidbody rb;

    private PlayerInventory inventory;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        inventoryAction = InputSystem.actions.FindAction("Inventory");

        if (moveAction == null)
        {
            Debug.LogError("The Input Action 'Move' could not be found. Check your Input Action Asset setup!");
        }
        else moveAction.Enable();

        if (inventoryAction == null)
        {
            Debug.LogError("The Input Action 'Inventory' could not be found. Check your Input Action Asset setup!");
        }
        else inventoryAction.Enable();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        ReadInput();
        Animate();

        if (inventoryAction != null && inventoryAction.WasPerformedThisFrame())
        {
            InventoryUI.Instance.ToggleInventory();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void ReadInput()
    {
        moveRead = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    }

    private void Movement()
    {
        Vector3 movementVector = new Vector3(moveRead.x, 0, moveRead.y);

        if (movementVector.sqrMagnitude > 1f)
            movementVector.Normalize();

        Vector3 targetPosition = rb.position + movementVector * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        if (movementVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementVector);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    private void Animate()
    {
        if (animator != null)
        {
            float currentSpeed = moveRead.magnitude;
            animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        }
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
            rb.position = loadedPosition;
        }

        inventory.LoadFrom(data);
    }

    public void setRigidBody(Vector3 position)
    {
        rb.position = position;
    }
}

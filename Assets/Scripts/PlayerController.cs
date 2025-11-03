using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputAction moveAction;
    Vector2 moveRead;
    public float speed; 
    public float rotationSpeed;

    private Animator animator;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        if (moveAction == null)
        {
            Debug.LogError("The Input Action 'Move' could not be found. Check your Input Action Asset setup!");
        }
        else
        {
            moveAction.Enable(); 
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadInput();
        Movement();//aply the movement and rotation every frame
        Animate();
    }

    private void ReadInput()
    {
        moveRead = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    }

    // Moves the player and rotates them to face the movement direction
    private void Movement()
    {
        Vector3 movementVector = new Vector3(moveRead.x, 0, moveRead.y);

        if (movementVector.magnitude > 1)
        {
            movementVector.Normalize();
        }

        if (movementVector != Vector3.zero)
        {
            //calculate the target rotation that faces the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementVector);

            //smoothy rotate towords the target rotation
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position += movementVector * Time.deltaTime * speed;
    }

    // Control the Animator smoothly
    private void Animate()
    {
        if (animator != null)
        {
            float currentSpeed = moveRead.magnitude;
            animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime); // damping for smooth blend
        }
    }
}
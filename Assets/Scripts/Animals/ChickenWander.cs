using UnityEngine;

public class ChickenWander : MonoBehaviour
{
    [Header("Wander")]
    public float radius = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float waitTime = 2f;

    [Header("Flee")]
    public float fleeSpeed = 5f;
    public float fleeDuration = 2.5f;
    public float fleeDistance = 6f;

    [Header("Ground")]
    public float groundCheckDistance = 5f;
    public Collider groundCollider;



    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastPosition;

    private float timer;
    private float fleeTimer;

    private bool isFleeing;

    private Animator anim;

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
        anim = GetComponent<Animator>();
        ChooseNewTarget();
    }

    void Update()
    {
        Vector3 direction = (targetPosition - transform.position);
        Vector3 dirNorm = direction.normalized;

        // Animation speed
        float movement = (transform.position - lastPosition).magnitude / Time.deltaTime;
        anim.SetFloat("Speed", movement);
        lastPosition = transform.position;

        // Rotate
        if (dirNorm.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirNorm);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move
        float speed = isFleeing ? fleeSpeed : moveSpeed;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Wander logic only when NOT fleeing
        if (!isFleeing && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                ChooseNewTarget();
                timer = 0f;
            }
        }

        // Flee timer
        if (isFleeing)
        {
            fleeTimer -= Time.deltaTime;
            if (fleeTimer <= 0f)
            {
                isFleeing = false;
                ChooseNewTarget();
            }
        }
    }

    public void FleeFrom(Vector3 dangerPosition)
    {
        isFleeing = true;
        fleeTimer = fleeDuration;

        Vector3 fleeDir = (transform.position - dangerPosition).normalized;
        Vector3 flatTarget = transform.position + fleeDir * fleeDistance;

        if (groundCollider != null &&
            groundCollider.Raycast(
                new Ray(flatTarget + Vector3.up * groundCheckDistance, Vector3.down),
                out RaycastHit hit,
                groundCheckDistance * 2f))
        {
            targetPosition = hit.point;
        }
        else
        {
            targetPosition = flatTarget;
        }
    }


    void ChooseNewTarget()
    {
        // Pick a random point on XZ plane
        Vector2 circle = Random.insideUnitCircle * radius;
        Vector3 flatTarget = startPosition + new Vector3(circle.x, 0f, circle.y);

        // Raycast down to the ground collider
        if (groundCollider != null &&
            groundCollider.Raycast(
                new Ray(flatTarget + Vector3.up * groundCheckDistance, Vector3.down),
                out RaycastHit hit,
                groundCheckDistance * 2f))
        {
            targetPosition = hit.point;
        }
        else
        {
            // Fallback: keep current Y
            targetPosition = new Vector3(flatTarget.x, transform.position.y, flatTarget.z);
        }
    }


}

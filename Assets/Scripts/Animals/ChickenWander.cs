using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ChickenWander : MonoBehaviour
{
    [Header("Wander")]
    public float radius = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 6f;
    public float waitTime = 2f;

    [Header("Flee")]
    public float fleeSpeed = 5f;
    public float fleeDuration = 2.5f;
    public float fleeDistance = 6f;

    [Header("Ground")]
    public float groundCheckDistance = 5f;
    public Collider groundCollider;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayers;
    public float avoidDistance = 2f;
    public float avoidTurnAngle = 90f;

    private Rigidbody rb;
    private Animator anim;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastPosition;

    private float waitTimer;
    private float fleeTimer;
    private bool isFleeing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        startPosition = transform.position;
        lastPosition = transform.position;

        // Rigidbody setup
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        ChooseNewTarget();
    }

    void Update()
    {
        // Update animation
        float movement = (transform.position - lastPosition).magnitude / Time.deltaTime;
        anim.SetFloat("Speed", movement);
        lastPosition = transform.position;

        // Flee logic
        if (isFleeing)
        {
            fleeTimer -= Time.deltaTime;
            if (fleeTimer <= 0f)
            {
                isFleeing = false;
                ChooseNewTarget();
            }
        }

        // Wait at destination
        if (!isFleeing && Vector3.Distance(transform.position, targetPosition) < 0.15f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                ChooseNewTarget();
                waitTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 toTarget = targetPosition - rb.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < 0.05f)
            return;

        Vector3 moveDir = toTarget.normalized;
        float speed = isFleeing ? fleeSpeed : moveSpeed;
        float moveStep = speed * Time.fixedDeltaTime;

        // SphereCast to detect obstacles in front
        Vector3 sphereOrigin = rb.position + Vector3.up * 0.3f;
        float sphereRadius = 0.2f;
        float sphereDistance = moveStep + 0.2f;

        RaycastHit hit;
        if (Physics.SphereCast(sphereOrigin, sphereRadius, moveDir, out hit, sphereDistance, obstacleLayers))
        {
            // Obstacle detected -> pick new direction
            AvoidObstacle(moveDir);
            return;
        }

        // Rotate toward target
        Quaternion lookRotation = Quaternion.LookRotation(moveDir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.fixedDeltaTime));

        // Move forward
        rb.MovePosition(rb.position + moveDir * moveStep);
    }

    void AvoidObstacle(Vector3 currentDir)
    {
        // Try several random directions until one is clear
        for (int i = 0; i < 6; i++)
        {
            float angle = Random.Range(-120f, 120f);
            Vector3 newDir = Quaternion.Euler(0f, angle, 0f) * currentDir;

            RaycastHit hit;
            if (!Physics.SphereCast(rb.position + Vector3.up * 0.3f, 0.2f, newDir, out hit, 1f, obstacleLayers))
            {
                Vector3 newTarget = rb.position + newDir.normalized * avoidDistance;
                SetTargetOnGround(newTarget);
                return;
            }
        }

        // Fallback if all directions blocked
        ChooseNewTarget();
    }

    public void FleeFrom(Vector3 dangerPosition)
    {
        isFleeing = true;
        fleeTimer = fleeDuration;

        Vector3 fleeDir = (transform.position - dangerPosition).normalized;
        Vector3 fleeTarget = transform.position + fleeDir * fleeDistance;

        SetTargetOnGround(fleeTarget);
    }

    void ChooseNewTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 flatTarget = startPosition + new Vector3(circle.x, 0f, circle.y);

            SetTargetOnGround(flatTarget);

            if (!Physics.CheckSphere(targetPosition, 0.2f, obstacleLayers))
                return;
        }

        targetPosition = transform.position; // fallback
    }

    void SetTargetOnGround(Vector3 flatTarget)
    {
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
            targetPosition = new Vector3(flatTarget.x, transform.position.y, flatTarget.z);
        }
    }
}

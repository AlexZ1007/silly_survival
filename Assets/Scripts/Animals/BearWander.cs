using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class BearWander : MonoBehaviour
{
    [Header("Wander")]
    public float radius = 5f;
    public float walkSpeed = 2f;
    public float rotationSpeed = 120f;
    public float waitTime = 2f;

    [Header("Chase")]
    public Transform player;
    public float chaseRadius = 10f;
    public float runSpeed = 4.5f;

    [Header("Attack")]
    public float attackRadius = 2f;
    public int attackDamage = 20;
    public float attackCooldown = 1.5f;

    [Header("Ground")]
    public float groundCheckDistance = 5f;
    public Collider groundCollider;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayers;
    public float avoidDistance = 3f;

    private Rigidbody rb;
    private Animator anim;
    private PlayerHealth playerHealth;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float waitTimer;
    private float attackTimer;

    private bool isChasing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        startPosition = transform.position;

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        ChooseNewTarget();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            isChasing = false;
            TryAttack();
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            isChasing = true;
            targetPosition = player.position;
        }
        else
        {
            isChasing = false;

            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    ChooseNewTarget();
                    waitTimer = 0f;
                }
            }
        }

        anim.SetBool("IsRunning", isChasing);
        anim.SetFloat("Speed", rb.linearVelocity.magnitude, 0.15f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        Vector3 toTarget = targetPosition - rb.position;
        toTarget.y = 0f;

        if (toTarget.magnitude < 0.05f)
            return;

        Vector3 desiredDir = toTarget.normalized;

        float speed = isChasing ? runSpeed : walkSpeed;
        float moveStep = speed * Time.fixedDeltaTime;

        // Obstacle check
        Vector3 origin = rb.position + Vector3.up * 0.6f;
        if (Physics.SphereCast(origin, 0.5f, transform.forward, out _, moveStep + 0.4f, obstacleLayers))
        {
            AvoidObstacle();
            return;
        }

        // Smooth heavy rotation
        Quaternion targetRot = Quaternion.LookRotation(desiredDir);
        Quaternion newRot = Quaternion.RotateTowards(
            rb.rotation,
            targetRot,
            rotationSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(newRot);

        // Move forward ONLY
        rb.MovePosition(rb.position + transform.forward * moveStep);
    }

    void TryAttack()
    {
        if (attackTimer > 0f)
            return;

        anim.SetTrigger("Attack");

        if (playerHealth != null)
            playerHealth.ModifyHealth(-attackDamage);

        attackTimer = attackCooldown;
    }

    void AvoidObstacle()
    {
        for (int i = 0; i < 6; i++)
        {
            float angle = Random.Range(-120f, 120f);
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            if (!Physics.SphereCast(rb.position + Vector3.up * 0.6f, 0.5f, dir, out _, 1.2f, obstacleLayers))
            {
                Vector3 newTarget = rb.position + dir * avoidDistance;
                SetTargetOnGround(newTarget);
                return;
            }
        }

        ChooseNewTarget();
    }

    void ChooseNewTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 flatTarget = startPosition + new Vector3(circle.x, 0f, circle.y);

            SetTargetOnGround(flatTarget);

            if (!Physics.CheckSphere(targetPosition, 0.6f, obstacleLayers))
                return;
        }

        targetPosition = transform.position;
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

    // Scene view debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

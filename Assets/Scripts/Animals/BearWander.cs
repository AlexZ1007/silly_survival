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
    public float knockbackStrength = 2f; // optional small push

    [Header("Ground")]
    public float groundCheckDistance = 5f;
    public Collider groundCollider;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayers;
    public float obstacleCheckDistance = 1.5f;

    private Rigidbody rb;
    private Animator anim;
    private PlayerHealth playerHealth;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastPosition;

    private float waitTimer;
    private float attackTimer;

    private bool isChasing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        startPosition = transform.position;
        lastPosition = transform.position;

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

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool isInAttackRange = distanceToPlayer <= attackRadius;
        bool isInChaseRange = distanceToPlayer <= chaseRadius;

        if (isInAttackRange)
        {
            // Face player and attack
            TryAttack();
            isChasing = true; // keep facing player
            targetPosition = player.position; // always face player
        }
        else if (isInChaseRange)
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

        // Animation driven by actual movement
        float actualSpeed =
            (transform.position - lastPosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        anim.SetFloat("Speed", actualSpeed, 0.15f, Time.deltaTime);
        anim.SetBool("IsRunning", isChasing);

        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 toTarget = targetPosition - rb.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = SmoothAvoidance(toTarget.normalized);
        float speed = isChasing ? runSpeed : walkSpeed;
        float moveStep = speed * Time.fixedDeltaTime;

        // If attacking, rotate toward player but do NOT move forward
        if (attackTimer > 0f && Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            Vector3 dirToPlayer = (player.position - rb.position).normalized;
            dirToPlayer.y = 0f;
            if (dirToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
            }
            return; // stop forward movement
        }

        // Normal movement
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }

        rb.MovePosition(rb.position + transform.forward * moveStep);
    }

    void TryAttack()
    {
        if (attackTimer > 0f || playerHealth == null) return;

        anim.SetTrigger("Attack");

        // Apply damage
        playerHealth.ModifyHealth(-attackDamage);

        // Optional knockback using velocity (safe, keeps player grounded)
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDir = (player.position - transform.position).normalized;
            knockbackDir.y = 0f;
            playerRb.linearVelocity = knockbackDir * knockbackStrength;
        }

        attackTimer = attackCooldown;
    }

    Vector3 SmoothAvoidance(Vector3 forwardDir)
    {
        Vector3 origin = rb.position + Vector3.up * 0.6f;
        if (Physics.SphereCast(origin, 0.5f, forwardDir, out RaycastHit hit, obstacleCheckDistance, obstacleLayers))
        {
            Vector3 slideDir = Vector3.Cross(Vector3.up, hit.normal).normalized;
            if (Vector3.Angle(forwardDir, slideDir) > 90f)
                slideDir = -slideDir;
            return slideDir;
        }
        return forwardDir;
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

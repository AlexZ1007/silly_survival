using UnityEngine;

public class ChickenWander : MonoBehaviour
{
    public float radius = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float waitTime = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 lastPosition;


    private float timer;

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

        // Calculate velocity
        float movement = (transform.position - lastPosition).magnitude / Time.deltaTime;
        anim.SetFloat("Speed", movement);
        lastPosition = transform.position;

        // Smooth rotation
        if (dirNorm.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirNorm);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // Move
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Pick new point
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                ChooseNewTarget();
                timer = 0f;
            }
        }
    }


    void ChooseNewTarget()
    {
        Vector2 circle = Random.insideUnitCircle * radius;
        targetPosition = startPosition + new Vector3(circle.x, 0, circle.y);
    }
}

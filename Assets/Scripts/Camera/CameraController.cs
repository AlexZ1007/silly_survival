using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform player;
    public float moveSpeed;
    public Vector3 offset;
    public float followDistance;
    public Quaternion rotation;

    public float teleportDistanceThreshold = 100f;
    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) > teleportDistanceThreshold)
        {
            transform.position = player.position+offset + -transform.forward * followDistance;
        }
        else
        {
            Vector3 pos = Vector3.Lerp(transform.position, player.position + offset + -transform.forward * followDistance, moveSpeed * Time.deltaTime);
            transform.position = pos;
        }

        transform.rotation = rotation;
    }
}

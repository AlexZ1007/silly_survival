using UnityEngine;

public class CameraController : MonoBehaviour
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

    // Function for saving camera
    public void SaveTo(SaveData data)
    {
        // Save position
        data.cameraPosition = new float[]
        {
            transform.position.x,
            transform.position.y,
            transform.position.z
        };

        // Save rotation (Quaternion)
        data.cameraRotation = new float[]
        {
            transform.rotation.x,
            transform.rotation.y,
            transform.rotation.z,
            transform.rotation.w
        };
    }



    // Function for loading camera
    public void LoadFrom(SaveData data)
    {
        // Load camera position
        if (data.cameraPosition != null && data.cameraPosition.Length == 3)
        {
            transform.position = new Vector3(
                data.cameraPosition[0],
                data.cameraPosition[1],
                data.cameraPosition[2]
            );
        }

        // Load camera rotation
        if (data.cameraRotation != null && data.cameraRotation.Length == 4)
        {
            transform.rotation = new Quaternion(
                data.cameraRotation[0],
                data.cameraRotation[1],
                data.cameraRotation[2],
                data.cameraRotation[3]
            );

            // Also update stored rotation variable so Update() applies correctly
            rotation = transform.rotation;
        }
    }

}

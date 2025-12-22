using UnityEngine;

public class ProgressBarFaceCamera : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Face the camera without inheriting parent rotation
        transform.rotation = Quaternion.LookRotation(
            transform.position - cam.transform.position
        );
    }
}

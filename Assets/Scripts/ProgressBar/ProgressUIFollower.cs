using UnityEngine;

public class ProgressUIFollower : MonoBehaviour
{
    public Transform target;        // object to follow
    public Vector3 offset = new Vector3(0, 2f, 0); // height above the object

    void LateUpdate()
    {
        if (target == null) return;

        // follow target
        transform.position = target.position + offset;

        // always face the camera
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }
}

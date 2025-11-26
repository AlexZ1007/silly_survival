using UnityEngine;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform player;
    private FadeObject lastFadedObject;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        RaycastHit hit;

        // Ray from camera to player
        if (Physics.Raycast(transform.position, direction, out hit, 20f))
        {
            // If the hit object has FadeObject
            FadeObject fade = hit.collider.GetComponent<FadeObject>();

            if (fade != null)
            {
                // Fade new tree
                if (lastFadedObject != fade)
                {
                    if (lastFadedObject != null)
                        lastFadedObject.Unfade();

                    fade.Fade();
                    lastFadedObject = fade;
                }
            }
            else
            {
                // No tree hit -> un-fade last one
                if (lastFadedObject != null)
                {
                    lastFadedObject.Unfade();
                    lastFadedObject = null;
                }
            }
        }
        else
        {
            // Nothing blocking
            if (lastFadedObject != null)
            {
                lastFadedObject.Unfade();
                lastFadedObject = null;
            }
        }
    }
}

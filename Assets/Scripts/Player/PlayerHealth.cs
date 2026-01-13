using UnityEngine;
using UnityEngine.UI; // for Slider

public class PlayerHealth : MonoBehaviour
{
    [Header("Health values")]
    [Tooltip("Maximum health")]
    public int maxHealth = 100;

    [Tooltip("Starting health (will be clamped between 0 and maxHealth)")]
    public int startingHealth = 100;

    [Tooltip("How much to reduce each time")]
    public int decrementAmount = 1;

    [Tooltip("Interval in seconds between decrements (10 minutes = 600 seconds)")]
    public float decrementInterval = 600f; // 10 minutes = 10 * 60 = 600

    [Header("UI")]
    [Tooltip("Assign the Slider from the Canvas (optional, can be null)")]
    public Slider healthSlider; // drag HealthSlider here

    [Header("Respawn")]
    [Tooltip("Safe respawn position in world coordinates")]
    public Transform respawnPoint;


    // current health (private but visible in inspector if you need)
    [HideInInspector]
    public int currentHealth;

    private float timer = 0f;

    void Start()
    {
        // initialize health
        currentHealth = Mathf.Clamp(startingHealth, 0, maxHealth);

        // setup slider if assigned
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.wholeNumbers = true;
            healthSlider.value = currentHealth;
            healthSlider.interactable = false;
        }

        timer = 0f;
    }

    void Update()
    {
        // Increase timer by deltaTime
        timer += Time.deltaTime;

        // If we've reached or exceeded the interval, decrement health and reset timer
        if (timer >= decrementInterval)
        {
            ApplyDecrement();
            // subtract the multiples of the interval that passed (in case of a large delta)
            // this keeps timing accurate if the game lags
            timer -= decrementInterval;
        }
    }

    void ApplyDecrement()
    {
        if (currentHealth <= 0) return;

        currentHealth -= decrementAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        // update UI if present
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // optional: handle death
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }


    void OnDeath()
    {
        Debug.Log("Player died - respawning at safe space");

        // Move player to safe position
        transform.position = respawnPoint.position;

        // Restore full health
        currentHealth = maxHealth;

        // Reset timer so health doesn't instantly decrease again
        timer = 0f;

        // Update UI
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }


    // Public helper: immediate damage or heal
    public void ModifyHealth(int delta)
    {
        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }


    void OnDrawGizmos()
    {
        if (respawnPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(respawnPoint.position, 0.4f);
    }


}

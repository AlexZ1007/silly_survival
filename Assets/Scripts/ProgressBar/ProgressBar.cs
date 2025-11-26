using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;     // the colored part of the bar
    public Image backgroundImage; // optional if you want

    [Header("Colors")]
    public Color correctToolColor = Color.green;
    public Color wrongToolColor = Color.red;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        gameObject.SetActive(false); // IMPORTANT: start hidden
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
            fillImage.color = correctToolColor;
        }
    }


    /// Updates the bar fill, color, and visibility.
    public void SetProgress(float value, bool hasCorrectTool)
    {
        if (fillImage == null) return;

        // Make the bar visible if progress > 0
        if (!gameObject.activeSelf && value > 0f)
            gameObject.SetActive(true);

        fillImage.fillAmount = Mathf.Clamp01(value);
        fillImage.color = hasCorrectTool ? correctToolColor : wrongToolColor;

        // Optional: hide the bar if progress is 0
        if (value <= 0f)
            gameObject.SetActive(false);
    }
}

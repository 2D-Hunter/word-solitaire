using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerBar : MonoBehaviour
{
    public Image greenBarImage;
    public float totalTime = 60f;

    private float timeLeft;
    private float originalWidth;

    private Color startColor = Color.green;
    private Color midColor = Color.yellow;
    private Color endColor = Color.red;

    private bool isFlashing = false;

    void Start()
    {
        timeLeft = totalTime;

        // Store the initial full width of the bar
        originalWidth = greenBarImage.rectTransform.sizeDelta.x;

        // Set initial color
        greenBarImage.color = startColor;
    }

    void Update()
    {
        if (!GameManager.instance.isCountdownTimerDone) return;
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            float fillAmount = Mathf.Clamp01(timeLeft / totalTime);

            // Smooth width update
            float targetWidth = originalWidth * fillAmount;
            float currentWidth = greenBarImage.rectTransform.sizeDelta.x;
            float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * 5f);

            Vector2 size = greenBarImage.rectTransform.sizeDelta;
            size.x = newWidth;
            greenBarImage.rectTransform.sizeDelta = size;

            // Smooth color transition: Green → Yellow → Red
            if (fillAmount > 0.5f)
            {
                float t = Mathf.InverseLerp(1f, 0.5f, fillAmount);
                greenBarImage.color = Color.Lerp(startColor, midColor, t);
            }
            else
            {
                float t = Mathf.InverseLerp(0.5f, 0f, fillAmount);
                greenBarImage.color = Color.Lerp(midColor, endColor, t);
            }

            // Start flashing under 10 seconds
            if (timeLeft <= 10f && !isFlashing)
            {
                isFlashing = true;
                StartCoroutine(FlashBar());
            }
        }
    }

    IEnumerator FlashBar()
    {
        while (timeLeft > 0f)
        {
            greenBarImage.enabled = false;
            yield return new WaitForSeconds(0.2f);
            greenBarImage.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }

        // Ensure it's visible at the end
        greenBarImage.enabled = true;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerFrame : MonoBehaviour
{
    public Image strokeImage;
    public float totalTime = 60f;

    public Color startColor = Color.green;
    public Color midColor = Color.yellow;
    public Color endColor = Color.red;

    public float pulseSpeed = 2f;
    public float pulseScale = 1.1f;

    private float timeLeft;
    private bool isFlickering = false;
    private Vector3 originalScale;

    void Start()
    {
        timeLeft = totalTime;
        strokeImage.fillAmount = 1f;
        strokeImage.color = startColor;
        strokeImage.enabled = true;

        originalScale = strokeImage.rectTransform.localScale;
    }

    void Update()
    {
        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            float fillAmount = Mathf.Clamp01(timeLeft / totalTime);
            strokeImage.fillAmount = fillAmount;

            // Color transition
            if (fillAmount > 0.5f)
            {
                float t = Mathf.InverseLerp(1f, 0.5f, fillAmount);
                strokeImage.color = Color.Lerp(startColor, midColor, t);
            }
            else
            {
                float t = Mathf.InverseLerp(0.5f, 0f, fillAmount);
                strokeImage.color = Color.Lerp(midColor, endColor, t);
            }

            // Start pulsing effect in final 10 seconds
            if (timeLeft <= 10f && !isFlickering)
            {
                isFlickering = true;
                StartCoroutine(ColorFlickerEffect());
            }
        }
        else
        {
            strokeImage.enabled = false; // Hide stroke at the end
            StopAllCoroutines();
        }
    }

    IEnumerator ColorFlickerEffect()
    {
        while (timeLeft > 0f)
        {
            // Flicker between red and white
            float t = Mathf.PingPong(Time.time * 5f, 1f); // Adjust 5f for speed
            strokeImage.color = Color.Lerp(endColor, Color.white, t);
            yield return null;
        }

        // After timer ends
        strokeImage.enabled = false;
    }
}
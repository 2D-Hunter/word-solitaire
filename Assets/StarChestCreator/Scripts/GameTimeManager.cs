using UnityEngine;
using TMPro;
using System.Collections;

namespace StarChestCreator         
{
public class GameTimeManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timeText; // UI text to display the remaining time

    [Header("Game Settings")]
    public float countdownMinutes = 1f; // Countdown time in minutes (set in Inspector)
    public float pulseStartTime = 15f;  // Time at which pulse effect starts (in seconds, set in Inspector)

    [Header("Warning Color")]
    public Color redWarningColor = new Color(1f, 0.15f, 0.3f, 1f); // Warning color can be selected in the Inspector

    private float timeLeft; // Remaining time in seconds
    private bool isCountingDown;

    private static GameTimeManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeTimeForLevel(); // Initialize the countdown timer at the start of the level
    }

    // Start the countdown for the level
    public void InitializeTimeForLevel()
    {
        MenuManager.instance.SwitchToGameScreen(); // Switch UI to the game screen
        gameObject.SetActive(true); // Ensure the GameTimeManager object is active
        timeLeft = countdownMinutes * 60; // Convert minutes to seconds
        isCountingDown = true; // Enable countdown
        UpdateTimeText(); // Update the UI text immediately
    }

    // Pause the countdown
    public static void PauseGame()
    {
        instance.TogglePause(true);
    }

    // Resume the countdown
    public static void ResumeGame()
    {
        instance.TogglePause(false);
    }

    // Handle the pause toggle
    private void TogglePause(bool isPaused)
    {
        ToggleCountDown(!isPaused);
        gameObject.SetActive(!isPaused); // Hide the game object when paused, show when resumed
    }

    // Control the countdown activity
    public static void ToggleCountDown(bool isActive)
    {
        instance.isCountingDown = isActive;
    }

    // Get the remaining time
    public static float GetRemainingTime()
    {
        return instance.timeLeft; // Return the remaining time
    }

    // Update the timer in each frame
    private void Update()
    {
        HandleTimeTick();
    }

    // Handle the countdown and check if the game is over
    private void HandleTimeTick()
    {
        if (isCountingDown && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0; // Ensure time does not go below zero
                DisplayGameOverPane();
            }

            UpdateTimeText();
        }
    }

    // Update the time text on the screen
    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";

        if (timeLeft <= pulseStartTime && timeLeft > 0)
        {
            timeText.color = redWarningColor; // Use custom red warning color
            if (!IsInvoking(nameof(StartPulseEffect))) // Only start pulse if it's not already running
            {
                InvokeRepeating(nameof(StartPulseEffect), 0f, 1f); // Start the pulse effect
            }
        }
        else
        {
            CancelInvoke(nameof(StartPulseEffect)); // Stop the pulse effect when not needed
            timeText.color = Color.white; // Reset the color to white during normal time
        }
    }

    // Show the game over panel or handle the game end scenario
    private void DisplayGameOverPane()
    {
        Debug.Log("Game Over!");
        gameObject.SetActive(false); // Hide the game object when the game is over
    }

    // Start the pulse effect for the time text
    private void StartPulseEffect()
    {
        StartCoroutine(PulseEffect());
    }

    // Pulse effect that repeatedly enlarges and shrinks the text
    IEnumerator PulseEffect()
    {
        Vector3 originalScale = timeText.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float pulseDuration = 0.5f;

        // Enlarge
        yield return ScaleText(originalScale, targetScale, pulseDuration);

        // Shrink
        yield return ScaleText(targetScale, originalScale, pulseDuration);
    }

    // Smoothly scale the text from one size to another
    IEnumerator ScaleText(Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            timeText.transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        timeText.transform.localScale = toScale;
    }

    // Method to pause the game and show the stars based on remaining time
    public void StopTime()
    {
        // Pause the timer
        ToggleCountDown(false);

        // Calculate and show stars
        StarManager.UpdateStarDisplay();
    }

    // This function will be called when the "Go to Menu" button is clicked
    public void GoToMenu()
    {
        // 1. Stop the countdown timer
        ToggleCountDown(false);

        // 2. Hide the star UI (and stop showing stars)
        StarManager.instance.HideStars();

        // 3. Call the function from MenuManager to handle currency flying and chest logic
        MenuManager.instance.StartCoroutine(MenuManager.instance.HandleCurrencyFlyAndChest());
    }
}
}
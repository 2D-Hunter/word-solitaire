using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace StarChestCreator         
{
public class ChestAnimator : MonoBehaviour
{
    public static ChestAnimator instance;

    public Image chestImage; // The UI Image component displaying the chest

    private List<Sprite> spriteFrames; // Sprites from the sprite sheet

    private List<Sprite> idleFrames = new List<Sprite>();
    private List<Sprite> openChestFrames = new List<Sprite>();
    private List<Sprite> claimChestFrames = new List<Sprite>();

    private Coroutine currentAnimationCoroutine;

    private float idleFrameRate = 30f; // Frames per second for idle animation
    private float openChestFrameRate = 30f; // Frames per second for open chest animation
    private float claimChestFrameRate = 30f; // Frames per second for claim chest animation

    private bool isIdleLoop = true;

    // Events triggered when animations complete
    public event System.Action OnOpenChestAnimationComplete;
    public event System.Action OnClaimChestAnimationComplete;

    public ParticleSystem chestParticleSystem; // Reference to the particle system
    public float particleStartDelay = 1.0f; // Delay for starting particle effects

    [Header("Movement Settings")]
    public Transform spawnPoint;
    public Transform targetPoint;
    public float moveDuration = 0.1f; // Duration for chest movement
    public float bounceDuration = 0.1f; // Duration for chest bounce effect

    [Header("UI Elements")]
    public GameObject openChestButton;
    public GameObject claimRewardButton;

    [Header("Dark Overlay")]
    public Image darkOverlay; // UI Image reference for dark overlay effect
    public float overlayFadeDuration = 0.5f; // Duration for overlay fade effect

    private Vector3 originalScale;

    void Awake()
    {
        // Singleton pattern to ensure only one ChestAnimator instance exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        originalScale = transform.localScale;

        // Initially hide the open chest and claim reward buttons
        //openChestButton.SetActive(false);
        //claimRewardButton.SetActive(false);

        // Set the dark overlay's alpha to zero at the start
        //darkOverlay.color = new Color(darkOverlay.color.r, darkOverlay.color.g, darkOverlay.color.b, 0);
        //darkOverlay.gameObject.SetActive(false);
    }

    void Start()
    {
        // Ensure chestImage is assigned
        if (chestImage == null)
        {
            chestImage = GetComponent<Image>();
        }
    }

    // Set sprite frames for chest animation
    public void SetSpriteFrames(List<Sprite> frames)
    {
        if (frames == null || frames.Count == 0)
        {
            Debug.LogError("Sprite frames list is null or empty.");
            return;
        }

        spriteFrames = frames;

        // Load animation frames for each chest state
        LoadAnimationFrames();

        // Set the chest image to the first idle frame immediately
        if (idleFrames != null && idleFrames.Count > 0)
        {
            chestImage.sprite = idleFrames[0];
        }
    }

    // Load animation frames into different states (idle, open, claim)
    private void LoadAnimationFrames()
    {
        // Clear previously loaded frames
        idleFrames.Clear();
        openChestFrames.Clear();
        claimChestFrames.Clear();

        if (spriteFrames == null || spriteFrames.Count == 0)
        {
            Debug.LogError("Sprite frames not assigned. Cannot load animation frames.");
            return;
        }

        // Check if enough frames are available
        if (spriteFrames.Count < 15)
        {
            Debug.LogError($"Not enough frames in spriteFrames list. Expected at least 15 frames but got {spriteFrames.Count}.");
            return;
        }

        // Idle Animation: Frames 0-9
        for (int i = 0; i <= 9; i++)
        {
            idleFrames.Add(spriteFrames[i]);
        }

        // Open Chest Animation: Frames 11-14
        for (int i = 10; i <= 15; i++)
        {
            openChestFrames.Add(spriteFrames[i]);
        }

        // Claim Chest Animation: Frames 14-11 (reverse order)
        for (int i = 15; i >= 10; i--)
        {
            claimChestFrames.Add(spriteFrames[i]);
        }
    }

    // Play Idle Animation (loops until stopped)
    public void PlayIdleAnimation()
    {
        if (idleFrames == null || idleFrames.Count == 0)
        {
            Debug.LogError("Idle frames are not set. Cannot play idle animation.");
            return;
        }

        // Reset opacity and set the first idle frame
        ResetOpacity();
        chestImage.sprite = idleFrames[0];

        // Stop any currently running animation and start idle animation
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        currentAnimationCoroutine = StartCoroutine(IdleAnimationCoroutine());
    }

    // Play Open Chest Animation
    public void PlayOpenChestAnimation()
    {
        // Stop any currently running animation and start open chest animation
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }
        currentAnimationCoroutine = StartCoroutine(OpenChestAnimationCoroutine());

        // Start particle effects with a delay
        Invoke("StartChestParticleSystem", particleStartDelay);

        // Hide open chest button and show claim reward button
        openChestButton.SetActive(false);
        claimRewardButton.SetActive(true);
    }

    // Idle Animation Coroutine
    private IEnumerator IdleAnimationCoroutine()
    {
        while (true)
        {
            // Loop through idle frames
            foreach (var frame in idleFrames)
            {
                chestImage.sprite = frame;
                yield return new WaitForSeconds(1f / idleFrameRate);
            }

            // Wait for 1 second between loops
            yield return new WaitForSeconds(1f);

            // If looping is disabled, break
            if (!isIdleLoop)
                break;
        }
    }

    // Open Chest Animation Coroutine
    private IEnumerator OpenChestAnimationCoroutine()
    {
        // Loop through open chest frames once
        foreach (var frame in openChestFrames)
        {
            chestImage.sprite = frame;
            yield return new WaitForSeconds(1f / openChestFrameRate);
        }

        // Keep the chest image on the last frame
        chestImage.sprite = openChestFrames[openChestFrames.Count - 1];

        // Invoke event to signal animation completion
        OnOpenChestAnimationComplete?.Invoke();
    }

    // Claim Chest Animation Coroutine
    public IEnumerator PlayClaimChestAnimationCoroutine()
    {
        // Stop the particle system
        Invoke("StopChestParticleSystem", 0.1f);

        // Animate opacity and scale during claim animation
        float fadeDuration = 0.75f;
        float elapsedTime = 0f;
        Color originalColor = chestImage.color;
        Vector3 originalScale = chestImage.transform.localScale;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);  // Target scale

        // Play claim animation while reducing opacity and scale
        for (int i = 0; i < claimChestFrames.Count; i++)
        {
            chestImage.sprite = claimChestFrames[i];
            elapsedTime += Time.deltaTime;

            // Calculate time factor for fading and scaling
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Fade out the chest opacity
            float alpha = Mathf.Lerp(1f, 0f, t);
            chestImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Scale down the chest image
            chestImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            yield return new WaitForSeconds(1f / claimChestFrameRate);
        }

        // Ensure final opacity is zero and scale is set
        chestImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        chestImage.transform.localScale = targetScale;

        // Hide claim reward button
        claimRewardButton.SetActive(false);

        // Signal chest opened state to other systems
        StarManager.instance.ChestOpened();

        // Invoke event to signal animation completion
        OnClaimChestAnimationComplete?.Invoke();
    }

    // Reset opacity to 1 (fully visible)
    public void ResetOpacity()
    {
        Color color = chestImage.color;
        color.a = 1f;
        chestImage.color = color;
    }

    // Start the particle effect when chest opens
    public void StartChestParticleSystem()
    {
        chestParticleSystem.Play();
    }

    // Stop the particle effect when chest closes
    public void StopChestParticleSystem()
    {
        chestParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // Move chest to target position with animation
    public void MoveChestToTarget()
    {
        StartCoroutine(MoveWithEase(transform, spawnPoint.position, targetPoint.position, moveDuration));
        StartCoroutine(ScaleWithEase(transform, originalScale, new Vector3(1.7f, 1.7f, 1.7f), bounceDuration));
        PlayIdleAnimation();

        // Show open chest button and dark overlay
        //openChestButton.SetActive(true);
        //darkOverlay.gameObject.SetActive(true);

        // Fade in the dark overlay
        StartCoroutine(FadeOverlay(0.9f));

        // Hide the star bar UI
        StarManager.instance.HideStarBar();
    }

    // Coroutine for smooth movement from spawn to target position
    private IEnumerator MoveWithEase(Transform obj, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeT = t * t * (3f - 2f * t); // Ease-in-out interpolation
            obj.position = Vector3.Lerp(start, end, easeT);
            yield return null;
        }
        obj.position = end;
    }

    // Coroutine for scaling with ease-in-out interpolation
    private IEnumerator ScaleWithEase(Transform obj, Vector3 startScale, Vector3 targetScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easeT = t * t * (3f - 2f * t); // Ease-in-out interpolation
            obj.localScale = Vector3.Lerp(startScale, targetScale, easeT);
            yield return null;
        }
        obj.localScale = targetScale;
    }

    // Coroutine for fading the dark overlay
    private IEnumerator FadeOverlay(float targetAlpha)
    {
        float currentAlpha = darkOverlay.color.a;
        float elapsed = 0f;

        while (elapsed < overlayFadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsed / overlayFadeDuration);
            darkOverlay.color = new Color(darkOverlay.color.r, darkOverlay.color.g, darkOverlay.color.b, newAlpha);
            yield return null;
        }

        darkOverlay.color = new Color(darkOverlay.color.r, darkOverlay.color.g, darkOverlay.color.b, targetAlpha);
    }

    // Reset chest to its initial position and state
    public void ResetChest()
    {
        transform.position = spawnPoint.position;
        transform.localScale = originalScale;

        // Hide dark overlay
        //darkOverlay.gameObject.SetActive(false);

        // Reset opacity to full
        ResetOpacity();

        // Show the star bar again
        StarManager.instance.ShowStarBar();
    }
    }
    }

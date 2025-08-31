using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StarProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillBar;                      // The UI Image whose fillAmount we tween
    public GameObject[] stars;                 // The 3 star icons
    [SerializeField] private ParticleSystem[] starsParticle; // Optional particle effects per star

    [Header("Scoring")]
    private float maxScore;                    // The score needed to fill the bar completely
    private float currentScore = 0f;           // Player's accumulated score this run

    [Header("Brilliance")]
    private int starValue = 25;                // Points per star for brilliance calculation
    private int starAmount = 0;                // How many stars unlocked this run
    private int brillianceScore = 0;           // Computed brilliance = starAmount * starValue

    [Header("Threshold Data (normalized)")]
    private List<float> starThresholds;        // Fractions (0..1) at which each star unlocks

    [Header("Threshold Data (absolute)")]
    private List<float> absoluteThresholds;    // Raw point thresholds from JSON, e.g. [95,142,189]

    [Header("Level Data (JSON)")]
    [SerializeField] private LevelData levelData;  // Your ScriptableObject or JSON holder

    // Track which stars have been unlocked
    private bool[] starUnlocked;

    void Start()
    {
        // Initialize state
        ResetBar();

        // Initialize unlocked flags to match the number of stars
        starUnlocked = new bool[stars.Length];

        // 1) Determine which level we're on
        int levelIndex = InitManager.instance.isReplay
            ? FBPlayerData.instance.CURRENT_LEVEL - 2
            : FBPlayerData.instance.CURRENT_LEVEL - 1;
        var levelEntry = levelData.levels[levelIndex];

        //// 2) Compute maxScore = sum of card values * estimated multiplier
        //maxScore = Mathf.RoundToInt(TotalPoints() * levelEntry.estimatedMultiplier);

        //// 3) Pull in the raw absolute thresholds from your JSON
        ////    Assume LevelInfo.PointsForEachStar is List<float> or List<int>
        //absoluteThresholds = levelEntry.starThresholds
        //    .Select(p => (float)p)
        //    .ToList();

        //// 4) Normalize those into fractions of maxScore for the fill bar
        //starThresholds = absoluteThresholds
        //    .Select(points => points / maxScore)
        //    .ToList();

        // 1) Read thresholds directly from JSON
        absoluteThresholds = levelEntry.starThresholds.Select(p => (float)p).ToList();

        // 2) Set maxScore = last threshold (i.e. points for 3rd star)
        maxScore = absoluteThresholds.Last();

        // 3) Convert thresholds into 0..1 fractions relative to maxScore
        starThresholds = absoluteThresholds.Select(points => points / maxScore).ToList();

        // 5) Kick off the bar at zero fill
        UpdateStarBar(0f);
    }

    /// <summary>
    /// Call this each time the player gains points.
    /// </summary>
    public void UpdateStarBar(float scoreGained)
    {
        float prevScore = currentScore;
        currentScore = Mathf.Clamp(currentScore + scoreGained, 0f, maxScore);
        float newFill = currentScore / maxScore;

        // Animate the fill amount
        fillBar.DOFillAmount(newFill, 0.5f).SetEase(Ease.OutQuad);

        // Check each star's absolute threshold
        for (int i = 0; i < stars.Length; i++)
        {
            if (!starUnlocked[i]
                && prevScore < absoluteThresholds[i]
                && currentScore >= absoluteThresholds[i])
            {
                UnlockStar(i);
            }
        }
    }

    /// <summary>
    /// Handles unlocking a star: shows it, plays particles & animation, increments counters.
    /// </summary>
    private void UnlockStar(int index)
    {
        if (index < 0 || index >= stars.Length) return;

        stars[index].SetActive(true);
        starUnlocked[index] = true;
        starAmount++;

        if (starsParticle != null && index < starsParticle.Length && starsParticle[index] != null)
        {
            starsParticle[index].Stop();
            starsParticle[index].Play();
        }

        // Pop-scale feedback
        stars[index].transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5);

        // Track in your game manager if needed
        GameManager.instance.earnedStarsInTheLevel++;
    }

    /// <summary>
    /// Calculates brilliance as stars × starValue.
    /// </summary>
    public int CalculateBrillianceScore()
    {
        brillianceScore = starAmount * starValue;
        return brillianceScore;
    }

    /// <summary>
    /// Sums up all card values in the level to determine base points.
    /// </summary>
    public int TotalPoints()
    {
        int sum = 0;
        foreach (var card in CardManager.instance.totalCardsToClear)
            sum += card.cardData.cardValue;
        return sum;
    }

    /// <summary>
    /// Resets the bar, stars, and internal counters to zero.
    /// </summary>
    public void ResetBar()
    {
        currentScore = 0f;
        starAmount = 0;
        fillBar.fillAmount = 0f;

        // Deactivate stars & reset flags
        starUnlocked = new bool[stars.Length];
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
            if (starsParticle != null && i < starsParticle.Length && starsParticle[i] != null)
                starsParticle[i].Stop();
        }
    }

    // Optional getters for other scripts
    public int StarCount => starAmount;
    public int ScoreAmount => Mathf.RoundToInt(currentScore);
}
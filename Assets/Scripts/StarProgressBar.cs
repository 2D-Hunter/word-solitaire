using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StarProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillBar;                     // Fill bar image
    public GameObject[] stars;                // 3 star icons
    [SerializeField] private ParticleSystem[] starsParticle; // Star effects

    [Header("Scoring")]
    private float maxScore = 15f;
    private float currentScore = 0f;

    [Header("Brilliance")]
    private int starValue = 25;
    private int starAmount = 0;
    private int brillianceScore = 0;
    private bool[] starUnlocked = new bool[3];

    [Header("Thresholds")]
    [SerializeField] private float[] starThresholds;
    [SerializeField] private LevelData levelData;

    void Start()
    {
        ResetBar();

        var level = levelData.levels[GameUtils.EffectiveCurrentLevel];
        starThresholds = level.starThresholds;
        maxScore = Mathf.RoundToInt(TotalPoints() * level.estimatedMultiplier);
        Debug.Log($"💡 StarProgressBar Max Score for Level {GameUtils.EffectiveCurrentLevel+1}: {maxScore}");
        Debug.Log($"💡 StarProgressBar Thresholds: {string.Join(", ", starThresholds)}");

        UpdateStarBar(0);
    }

    public void UpdateStarBar(float scoreGained)
    {
        float previousFill = currentScore / maxScore;
        currentScore += scoreGained;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        float newFill = Mathf.Clamp01(currentScore / maxScore);

        fillBar.DOFillAmount(newFill, 0.5f).SetEase(Ease.OutQuad);

        float cumulative = 0f;

        for (int i = 0; i < stars.Length; i++)
        {
            cumulative += starThresholds[i];

            if (!starUnlocked[i] && previousFill < cumulative && newFill >= cumulative)
            {
                UnlockStar(i);
            }
        }
    }

    private void UnlockStar(int index)
    {
        if (index >= stars.Length) return;

        stars[index].SetActive(true);
        starUnlocked[index] = true;
        starAmount++;

        if (starsParticle != null && index < starsParticle.Length)
        {
            starsParticle[index].Stop();
            starsParticle[index].Play();
        }

        // Optional: scale feedback
        stars[index].transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 1);

        // Optional: play sound
        // SoundManager.Instance.PlayOneShot("star_unlock");
    }

    public int CalculateBrillianceScore()
    {
        return brillianceScore = starAmount * starValue;
    }

    public int TotalPoints()
    {
        int totalPoints = 0;
        foreach (Card card in CardManager.instance.totalCardsToClear)
        {
            totalPoints += card.cardData.cardValue;
        }
        return totalPoints;
    }

    public void ResetBar()
    {
        currentScore = 0f;
        starAmount = 0;
        fillBar.fillAmount = 0f;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
            starUnlocked[i] = false;
            if (starsParticle != null && i < starsParticle.Length)
                starsParticle[i].Stop();
        }
    }
}
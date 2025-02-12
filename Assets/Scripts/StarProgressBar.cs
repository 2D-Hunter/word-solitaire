using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StarProgressBar : MonoBehaviour
{
    public Image fillBar;  // Assign the fill bar image
    public GameObject[] stars;  // Assign 3 star images
     float maxScore = 15f;  // Max score for 3 stars
    private float currentScore = 0f;
    int starValue = 25;
    int starAmount = 0;
    int brillianceScore = 0;
    [SerializeField] private ParticleSystem[] starsParticle = null;
    private bool[] starUnlocked = { false, false, false };

    void Start()
    {
        brillianceScore = 0;
        maxScore = TotalPoints();
        UpdateStarBar(0);  // Initialize at 0%
    }
    public int CalculateMaxScoreByCards(int numberOfCards, int pointsPerCard)
    {
        return numberOfCards * pointsPerCard;
    }
    public int TotalPoints()
    {
        int totalPoints = 0;

        foreach (Card card in CardManager.instance.totalCardsToClear)
        {
            totalPoints += card.cardData.cardValue; // Add each card's points to the total
        }
        Debug.Log("totalPoints: "+ totalPoints);
        return totalPoints;
    }

    // Call this method whenever the player earns points
    public void UpdateStarBar(float score)
    {
        currentScore = Mathf.Clamp(score, 0, maxScore);  // Limit to maxScore
        float fillAmount = currentScore / maxScore;  // Normalize to 0-1
        fillBar.fillAmount = fillAmount;  // Update UI fill

        


        for(int i = 0; i < stars.Length; i++)
        {
            // Check if the fill amount meets the condition for the star index
            if (fillAmount >= (i + 1) * 0.33f && !starUnlocked[i])
            {
                stars[i].SetActive(true); // Activate the star
                starsParticle[i].Stop(); // Stop any existing particle effect
                starsParticle[i].Play(); // Play particle effect
                starUnlocked[i] = true; // Mark this star as unlocked

                starAmount++;
                // SoundManager.Instance.PlayOneShot("ui_goal_complete");
            }
        }
    }
    public int CalculateBrillianceScore()
    {
        return brillianceScore = starValue * starAmount;
    }
}
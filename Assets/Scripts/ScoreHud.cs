using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Assign in Inspector
    public TextMeshProUGUI scoreTextShadow; // Assign in Inspector
    private int displayedScore = 0; // UI displayed score

    void Start()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreUpdated.AddListener(AnimateScoreIncrease);
            ScoreManager.instance.ResetScore();
            UpdateScoreUI(ScoreManager.instance.GetScore()); // Ensure UI starts correctly
        }
    }

    void AnimateScoreIncrease(int newScore)
    {
        int tempScore = displayedScore; // Store current displayed score

        DOTween.To(() => tempScore, x =>
        {
            tempScore = x;
            scoreText.text = scoreTextShadow.text = tempScore.ToString();
        }, newScore, 0.5f).SetEase(Ease.OutQuad)
        .OnUpdate(() =>
        {
            displayedScore = tempScore; // Ensure displayedScore follows the tween
    })
        .OnComplete(() =>
        {
            displayedScore = newScore; // Ensure final value is correct
            Debug.Log($"Tween Complete! displayedScore: {displayedScore}, newScore: {newScore}");
            UpdateScoreUI(newScore);
        });
    }

    void UpdateScoreUI(int score)
    {
        // Only update immediately if DoTween isn't running
        //if (!DOTween.IsTweening(displayedScore))
        //{
            displayedScore = score;
            scoreText.text = scoreTextShadow.text = displayedScore.ToString();
            Debug.Log("UpdateScoreUI");
        //}
    }

    void OnDestroy()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreUpdated.RemoveListener(AnimateScoreIncrease);
        }
    }
}
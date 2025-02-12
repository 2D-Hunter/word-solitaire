using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }

    public UnityEvent<int> OnScoreUpdated = new UnityEvent<int>(); // Event for UI updates

    private int score = 0;

    void Awake()
    {
        instance = this;
    }
    public void ResetScore()
    {
        score = 0;
        OnScoreUpdated.Invoke(score); // Notify UI
        Debug.Log("Score Reset to Zero at Game Start");
    }

    public void AddScore(int amount)
    {
        int previousScore = score;
        score += amount;
        Debug.Log("Score Updated: " + score);

        // Animate score increase with DoTween
        OnScoreUpdated.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }
}
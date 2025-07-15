using UnityEngine;
using TMPro; // Only if using TextMeshPro
using DG.Tweening; // Make sure DOTween is installed
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Assign in Inspector
    public float zoomDuration = 0.5f;
    public float waitBetweenNumbers = 0.2f;

    private void Start()
    {
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        int[] countdownNumbers = { 3, 2, 1 };

        foreach (int number in countdownNumbers)
        {
            countdownText.text = number.ToString();
            countdownText.transform.localScale = Vector3.zero;

            // Zoom In
            countdownText.transform.DOScale(1.5f, zoomDuration / 2).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(zoomDuration / 2);

            // Zoom Out
            countdownText.transform.DOScale(0f, zoomDuration / 2).SetEase(Ease.InBack);
            yield return new WaitForSeconds(zoomDuration / 2 + waitBetweenNumbers);
        }

        countdownText.text = ""; // Hide text

        StartGame(); // Replace this with your game start logic
    }

    void StartGame()
    {
        Debug.Log("Game Started!");
        GameManager.instance.isCountdownTimerDone = true;
        Destroy(gameObject);
        // Trigger your game start logic here
    }
}
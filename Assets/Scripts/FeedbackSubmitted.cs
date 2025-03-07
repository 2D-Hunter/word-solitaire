using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FeedbackSubmitted : MonoBehaviour
{
    public RectTransform messageBox;
    public CanvasGroup msg; // For fading effect
    public float tweenDuration = 0.5f; // Duration of the tween animation
    public float displayTime = 2f; // Time the message box stays visible
    private float offScreenY; // Position when hidden
    private float onScreenY; // Position when visible

    private void Start()
    {
        // Calculate positions
        offScreenY = -messageBox.sizeDelta.y; // Off-screen (below)
        onScreenY = messageBox.sizeDelta.y-25; // Final position at the bottom

        // Start hidden
        messageBox.anchoredPosition = new Vector2(0, offScreenY);
        msg.alpha = 0;
        StartCoroutine(ShowMessage());
    }
    IEnumerator ShowMessage()
    {
        yield return new WaitForSeconds(1);

        // Move from off-screen to visible
        messageBox.DOAnchorPosY(onScreenY, tweenDuration).SetEase(Ease.OutBack);
        msg.DOFade(1, tweenDuration); // Fade in

        // Wait, then fade out and move up
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(displayTime); // Wait
        seq.Append(messageBox.DOAnchorPosY(offScreenY, tweenDuration).SetEase(Ease.InBack));
        seq.Join(msg.DOFade(0, tweenDuration)); // Fade out
        seq.OnComplete(RemoveThis);
    }
    void RemoveThis()
    {
        InitManager.instance.feedbackSubmitted = false;
        PopupManager.instance.ToggleMessage(PopupManager.instance.feedbackSubmitted);
    }
}

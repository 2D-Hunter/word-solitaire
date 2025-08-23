using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;
using DG.Tweening;
using TMPro;

public class HintButton : MonoBehaviour
{
    public RectTransform hintBubblePrefab;
    public RectTransform parentRect;      // assign the parent container RectTransform
    private RectTransform currentBubble;
    public Camera uiCamera;               // leave null if Canvas is Screen Space - Overlay

    bool isDestroyed;

    void OnDestroy()
    {
        isDestroyed = true;
        KillAndDestroyCurrent();
    }

    void KillAndDestroyCurrent()
    {
        if (currentBubble)
        {
            // Kill any tweens targeting this bubble BEFORE destroying it
            currentBubble.DOKill();              // extension kill
            DOTween.Kill(currentBubble);         // extra safety
            Destroy(currentBubble.gameObject);
            currentBubble = null;
        }
    }

    public void OnclickHint()
    {
        if (isDestroyed) return;

        FBPlayerData.instance.VibrationEffect();
        SoundManager.instance.PlaySFX("HintSound", 0.3f);

        WordServiceContainer.HintService.HintClick((isFound, cards) =>
        {
            if (isDestroyed || !gameObject) return; // this button might have been destroyed while waiting

            // Remove previous bubble safely
            KillAndDestroyCurrent();

            // Create new bubble
            currentBubble = Instantiate(hintBubblePrefab, parentRect);

            // Position it above this button
            var btnRT = (RectTransform)transform;
            Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(uiCamera, btnRT.position);
            Vector2 localPt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPt, uiCamera, out localPt);
            localPt.y += 95f;
            currentBubble.anchoredPosition = localPt;

            // Animate THIS instance (avoid using a stale singleton)
            currentBubble.localScale = Vector3.zero;

            var bubble = currentBubble.GetComponent<HintBubble>();
            if (bubble != null)
            {
                bubble.AnimateBubble();  // make sure HintBubble kills its own tweens in OnDisable/OnDestroy
            }
            else
            {
                // simple fallback animation if no component
                currentBubble.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
            }
        });
    }
}

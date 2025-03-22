using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public RectTransform handRectTransform;
    private float moveDistance = 50f; // Distance to move in X
    private float duration1 = 0.7f;

    public RectTransform cardTransform;  // Assign the card's RectTransform in the Inspector
    private float scaleFactor = 1.2f;     // How much the card scales up
    private float rotationAmount = 12f;   // Rotation angle for zig-zag effect
    private float duration = 0.5f;        // Animation duration

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isAnimating = false;


    private void Start()
    {
        Vector2 initialPos = handRectTransform.anchoredPosition;
        handRectTransform.DOAnchorPosX(initialPos.x + moveDistance, duration1)
            .SetEase(Ease.InOutSine)  // Smooth motion
            .SetLoops(-1, LoopType.Yoyo);

        if (cardTransform == null)
            cardTransform = GetComponent<RectTransform>();

        originalScale = cardTransform.localScale;
        originalRotation = cardTransform.localRotation;

        //Invoke("AnimateCard", 3f);
    }

    public void AnimateCard()
    {
        if (isAnimating) return;
        isAnimating = true;
        cardTransform.SetAsLastSibling();
        // Scale up
        cardTransform.DOScale(originalScale * scaleFactor, duration * 0.5f).SetEase(Ease.OutQuad);

        // Zig-zag rotation
        cardTransform.DORotate(new Vector3(0, 0, rotationAmount), duration * 0.25f)
            .SetEase(Ease.InOutSine)
            .SetLoops(4, LoopType.Yoyo)
            .ChangeStartValue(new Vector3(0, 0, -rotationAmount))
            .OnComplete(() =>
            {
                cardTransform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad);
            });

        cardTransform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.InQuad).SetDelay(0.3f)
            .OnComplete(() =>
            {
                isAnimating = false;
            });
    }
}

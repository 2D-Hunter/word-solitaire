using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class HintBubble : MonoBehaviour
{
    public static HintBubble instance;
    private RectTransform rectTransform;
    public TextMeshProUGUI hintTxt;
    private void Awake()
    {
        instance = this;
    }
    public void AnimateBubble()
    {
        hintTxt.text = GameManager.instance.hintText;
        rectTransform = GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

        sequence.Append(
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 20, 0.3f) // Move up
                .SetEase(Ease.InOutSine).SetDelay(0.3f)
                .SetLoops(4, LoopType.Yoyo) // Loops up and down twice
        );
        sequence.AppendInterval(0.5f);
        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}

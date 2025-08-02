using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameCoinHud : MonoBehaviour
{
    public static GameCoinHud instance;
    public Canvas canvas = null;
    private RectTransform rectTransform;
    RectTransform canvasRect;
    private Vector2 originalPos;

    private void Awake()
    {
        instance = this;

        rectTransform = gameObject.GetComponent<RectTransform>();
        canvasRect = canvas.GetComponent<RectTransform>();
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasRect.rect.width, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        originalPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }
    public void Show()
    {
        rectTransform.DOKill();
        rectTransform.DOAnchorPosX(-150f, 0.3f).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            Invoke("Hide", 2f);
        });
    }
    public void Hide()
    {
        rectTransform.DOAnchorPosX(canvasRect.rect.width, 0.5f)
        .OnComplete(() =>
        {
            rectTransform.DOKill();
        });
    }
    public void ResetCoinHud()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = originalPos;
    }
}

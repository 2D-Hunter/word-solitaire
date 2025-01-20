using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BackButton : MonoBehaviour
{
    public static BackButton instance;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI shadowText;
    private RectTransform rectTransform;

    void Start()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        HideThis();
    }
    public void ShowThis()
    {
        rectTransform.localScale = new Vector3(0f, 0f, 1.0f);
        rectTransform.DOScale(new Vector3(0.85f, 0.85f, 1f), 0.3f).SetEase(Ease.OutBack);
    }
    public void HideThis()
    {
        rectTransform.localScale = new Vector3(0f, 0f, 1.0f);
        gameObject.SetActive(false);

    }
    public void OnBackButtonTap()
    {
        if (CardManager.instance.rightSideCards.Count > 1)
        {
            Card.instance.FlipCardBack();
        }

    }
}

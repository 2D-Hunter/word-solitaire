using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class MoreHeartsPopup2 : MonoBehaviour
{
    public static MoreHeartsPopup2 instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    public TextMeshProUGUI heartText, heartTextShadow, timerText, timerTextShadow = null;

    
    float delayIncrement = 0.15f;

    private float origScaleAmount = 1f;
    private float scaleAmount = 1.1f;
    private float duration = 0.1f;
    public RectTransform heart;

    int coinsRequiredToRefillAll = 1000;
    int coinsRequiredToRefillOne = 300;

    public GameObject obj1, obj2, obj3;
    public GameObject fullObj1, fullObj2;
    Sequence heartbeatSequence;

    private void Awake()
    {
        instance = this;
        SetInit();
    }
    public void HeartsAreFull()
    {
        popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 814.84f);
        obj1.SetActive(false);
        obj2.SetActive(false);
        obj3.SetActive(false);
        fullObj1.SetActive(true);
        fullObj2.SetActive(true);
    }
    private void SetInit()
    {
        fullObj1.SetActive(false);
        fullObj2.SetActive(false);
        bg.alpha = 0;
        popup.alpha = 0;
        foreach (var btn in btns)
        {
            btn.alpha = 0;
        }
        foreach (var btnRectTransform in btnsRectTransform)
        {
            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        popupRectTransform.anchoredPosition = new Vector2(0, 0);

    }
    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, 0);
        Invoke("StartHeartBeat", 2f);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();
        bg.DOKill();
        popup.DOKill();
        popupRectTransform.DOKill();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupRectTransform.DOAnchorPosY(365, 0.4f).SetEase(Ease.OutBack);

        float delay = 0.1f; // Initial delay

        for (int i = 0; i < btns.Length; i++)
        {
            // Ensure starting conditions
            btns[i].alpha = 0;
            btnsRectTransform[i].localScale = Vector3.one * 0.6f;

            // Fade In
            btns[i].DOFade(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            // Scale Up
            btnsRectTransform[i].DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            delay += delayIncrement; // Increase delay for next button
        }
    }
    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.InBack);

    }
    void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.outOfHeartsPopup);
    }
    void StartHeartBeat()
    {
        heartbeatSequence = DOTween.Sequence();

        heartbeatSequence.Append(heart.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // First beat
                         .Append(heart.DOScale(origScaleAmount, duration).SetEase(Ease.InQuad)) // Back to normal
                         .Append(heart.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // Second beat
                         .Append(heart.DOScale(origScaleAmount, duration).SetEase(Ease.InQuad)) // Back to normal
                         .AppendInterval(UnityEngine.Random.Range(2f, 5f)) // Wait before next heartbeat
                         .SetLoops(-1); // Repeat infinitely
    }

    public void RefillHearts(int amountToIncreaseBy)
    {
        FBPlayerData.instance.VibrationEffect();
        int coinsRequired;
        if (amountToIncreaseBy == 1)
            coinsRequired = coinsRequiredToRefillOne;
        else
            coinsRequired = coinsRequiredToRefillAll;

        if (CoinManager.instance.totalCoins >= coinsRequired)
        {
            CoinManager.instance.SpendCoins(coinsRequired);
            HeartManager.instance.RefillHearts(amountToIncreaseBy);
            ClosePopup();
        }
        else
        {
            PopupManager.instance.ToggleShop();
        }
    }
    private void OnDestroy()
    {
        heartbeatSequence.Kill();
        bg?.DOKill();
        popup?.DOKill();
        popupRectTransform?.DOKill();
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i]?.DOKill();
            btnsRectTransform[i]?.DOKill();
        }
    }

}

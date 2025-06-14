using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class WildCardPopup : MonoBehaviour
{
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    
    float delayIncrement = 0.15f;

    private float origScaleAmount = 2f;
    private float scaleAmount = 2.1f;
    private float duration = 0.1f;
    public RectTransform wildCard;

    int coinsRequired = 400;
    public GameObject videoAdBtn, coinsBtn, useBtn;
    
    Sequence cardSequence;

    private void Awake()
    {
        if(FBPlayerData.instance.TOTAL_WILD_CARD >= 1)
        {
            videoAdBtn.SetActive(false);
            coinsBtn.SetActive(false);
            useBtn.SetActive(true);
        }
        else
        {
            useBtn.SetActive(false);
            videoAdBtn.SetActive(true);
            coinsBtn.SetActive(true);
        }
        SetInit();
    }
    public void HeartsAreFull()
    {
        popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 814.84f);
        
    }
    private void SetInit()
    {
        
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
        Invoke("StartCardAnim", 2f);
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
        PopupManager.instance.TogglePopup(PopupManager.instance.wildcardPopup);
    }
    void StartCardAnim()
    {
        cardSequence = DOTween.Sequence();

        cardSequence.Append(wildCard.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // First beat
                         .Append(wildCard.DOScale(origScaleAmount, duration).SetEase(Ease.InQuad)) // Back to normal
                         .Append(wildCard.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // Second beat
                         .Append(wildCard.DOScale(origScaleAmount, duration).SetEase(Ease.InQuad)) // Back to normal
                         .AppendInterval(UnityEngine.Random.Range(2f, 5f)) // Wait before next heartbeat
                         .SetLoops(-1); // Repeat infinitely
    }

    
    private void OnDestroy()
    {
        cardSequence.Kill();
        bg?.DOKill();
        popup?.DOKill();
        popupRectTransform?.DOKill();
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i]?.DOKill();
            btnsRectTransform[i]?.DOKill();
        }
    }
    void RemoveThis1()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.wildcardPopup);
        //GameManager.instance.UseWildCardNow();
    }
    public void OnTapVideoAd()
    {
        Application.ExternalCall("ShowAd_Reward", "WildCard");
    }
    public void ContinueGameAfterRewardAd()
    {
        FBPlayerData.instance.TOTAL_WILD_CARD++;
        FBPlayerData.instance.SavePlayerData();
        FindObjectOfType<NumberOfWildCard>().UpdateWildCard();

        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis1);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.InBack);
        Invoke("UseWildCard", 0.3f);
    }
    
    public void OnTapCoins()
    {
        if (CoinManager.instance.totalCoins >= coinsRequired)
        {
            CoinManager.instance.SpendCoins(coinsRequired);
            FBPlayerData.instance.TOTAL_WILD_CARD++;
            FBPlayerData.instance.SavePlayerData();
            FindObjectOfType<NumberOfWildCard>().UpdateWildCard();

            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis1);
            popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
            popupRectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.InBack);
            Invoke("UseWildCard", 0.3f);
        }
        else
        {
            PopupManager.instance.ToggleShop();
        }
        
    }
    public void OnTapUse()
    {
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis1);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.InBack);
        Invoke("UseWildCard", 0.3f);
        
    }
    void UseWildCard()
    {
        GameManager.instance.UseWildCardNow();
        this.GetComponent<RectTransform>().SetAsLastSibling();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class MoreCardsPopup : MonoBehaviour
{
    public static MoreCardsPopup instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;


    
    float delayIncrement = 0.15f;

   
    public TextMeshProUGUI price;
    public TextMeshProUGUI priceShadow;



    private void Awake()
    {
        instance = this;
        SetInit();
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
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);

    }
    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);

        ShowPopup();
    }
    private void OnEnable()
    {
        
        Debug.Log("InitManager.instance.buyMoreCardsCntr: " + InitManager.instance.buyMoreCardsCntr);
        if (InitManager.instance.buyMoreCardsCntr == 1)
            InitManager.instance.moreCardsPrice = 150;
        else if (InitManager.instance.buyMoreCardsCntr == 2)
            InitManager.instance.moreCardsPrice = 250;
        else
            InitManager.instance.moreCardsPrice += 250;

        price.text = priceShadow.text = InitManager.instance.moreCardsPrice.ToString();
    }
    public void ShowPopup()
    {
        SetInit();
        bg.DOKill();
        popup.DOKill();
        popupRectTransform.DOKill();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupRectTransform.DOAnchorPosY(-70f, 0.4f).SetEase(Ease.OutBack);

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
        popupRectTransform.DOAnchorPosY(-350f, 0.4f).SetEase(Ease.InBack);

    }
    void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.moreCardsPopup);

    }
    public void TapOnVideoAd()
    {
        FBPlayerData.instance.VibrationEffect();
        FBPlayerData.instance.AD_TYPE = "MoreCards";

#if UNITY_EDITOR
        //FBPlayerData.instance.ShowAdsNotAvailable("Ad not completed");
        ClosePopupAndGiveRewards();
            return;
#endif
            Application.ExternalCall("ShowAd_Reward", "MoreCards");
    }
    public void TapOnCoins()
    {
        FBPlayerData.instance.VibrationEffect();
        Debug.Log("TapOnCoins: " + FBPlayerData.instance.TOTAL_COINS);
        Debug.Log("TapOnCoins: " + InitManager.instance.moreCardsPrice);
        if (FBPlayerData.instance.TOTAL_COINS >= InitManager.instance.moreCardsPrice)
        {
            InitManager.instance.buyMoreCardsCntr++;
            CoinManager.instance.SpendCoins(InitManager.instance.moreCardsPrice);
            ClosePopupAndGiveRewards();
        }
        else
        {
            PopupManager.instance.ToggleShop();
        }
    }
    public void ClosePopupAndGiveRewards()
    {
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis1);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);

    }
    void RemoveThis1()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.moreCardsPopup);
        FBPlayerData.instance.Get5CardsAfterVideoAd();
    }


    private void OnDestroy()
    {
        
        bg?.DOKill();
        popup?.DOKill();
        popupRectTransform?.DOKill();
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i]?.DOKill();
            btnsRectTransform[i]?.DOKill();
        }
    }
    public void OpenShop()
    {
        PopupManager.instance.ToggleShop();
    }

}

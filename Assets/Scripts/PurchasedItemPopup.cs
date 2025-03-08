using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class PurchasedItemPopup : MonoBehaviour
{
    public static PurchasedItemPopup instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.15f;

    public GameObject[] purchasedItems;
    
    public GameObject collectBtn;

    public GameObject sparkle = null;


    private void Awake()
    {
        instance = this;
        
        SetInit();
        
    }
    
    private void SetInit()
    {
        sparkle.SetActive(false);
        foreach (var item in purchasedItems)
        {
            if(item.activeSelf)
                item.SetActive(false);
        }
        
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
        popupRectTransform.anchoredPosition = new Vector2(0, -350);

    }
    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, -350);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();
        ShowPurchasedProduct();
        bg?.DOKill();
        popup?.DOKill();
        popupRectTransform?.DOKill();
        if(bg != null)
            bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        if (popup != null)
            popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        if (popupRectTransform != null)
            popupRectTransform.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack).OnComplete(SpawnSparkle);

        float delay = 0.1f; // Initial delay
        Debug.Log(bg);
        Debug.Log(popup);
        Debug.Log(popupRectTransform);
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
    public void SpawnSparkle()
    {
        sparkle.SetActive(true);
    }
    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        sparkle.SetActive(false);
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);

    }
    void RemoveThis()
    {
        if (PopupManager.instance != null && PopupManager.instance.purchasedItemPopup != null)
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.purchasedItemPopup);
        }
    }
    void ShowPurchasedProduct()
    {
        switch(FBPlayerData.instance.productID)
        {
            
            case "coins_2000":
                purchasedItems[0].SetActive(true);
                CoinManager.instance.AddCoins(2000);
                break;
            case "coins_6000":
                purchasedItems[1].SetActive(true);
                CoinManager.instance.AddCoins(6000);
                break;
            case "coins_16000":
                purchasedItems[2].SetActive(true);
                CoinManager.instance.AddCoins(16000);
                break;
            case "coins_34000":
                purchasedItems[3].SetActive(true);
                CoinManager.instance.AddCoins(34000);
                break;
            case "coins_70400":
                purchasedItems[4].SetActive(true);
                CoinManager.instance.AddCoins(70400);
                break;
            case "no_ads_30_days":
                TextMeshProUGUI[] texts = collectBtn.GetComponentsInChildren<TextMeshProUGUI>();
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].text = "Close";
                }
                purchasedItems[5].SetActive(true);

                DateTime expiryDate = DateTime.UtcNow.AddDays(30);
                string expiryDateStr = expiryDate.ToString("o"); // ISO 8601 format (better for parsing)

                if (GameUtils.IsFacebookBuild())
                {
                    //FBPlayerData.instance.SetData("No_Ads_30_Days", expiryDateStr);
                    FBPlayerData.instance.NO_ADS_30_DAYS = true;
                    FBPlayerData.instance.SavePlayerData();
                }
                else
                {
                    // Save using PlayerPrefs (for non-Facebook builds)
                    PlayerPrefs.SetString("No_Ads_30_Days", expiryDate.ToString());
                    PlayerPrefs.Save();
                }

                



                
                break;
        }
        FBPlayerData.instance.SavePlayerData();
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

}

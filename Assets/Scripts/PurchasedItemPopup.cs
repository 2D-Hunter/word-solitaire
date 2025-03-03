//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using TMPro;
//using UnityEngine.SceneManagement;
//using System;


//public class PurchasedItemPopup : MonoBehaviour
//{
//    public static PurchasedItemPopup instance;
//    public CanvasGroup bg = null;
//    public CanvasGroup popup = null;
//    public RectTransform popupRectTransform = null;

//    public CanvasGroup[] btns = null;
//    public RectTransform[] btnsRectTransform = null;

//    float delay = 0f;
//    float delayIncrement = 0.15f;

//    public GameObject[] purchasedItems;

//    private void Awake()
//    {
//        instance = this;
//        SetInit();
//    }

//    private void SetInit()
//    {
//        foreach (var item in purchasedItems)
//        {
//            if(item.activeSelf)
//                item.SetActive(false);
//        }
//        ShowPurchasedProduct();
//        bg.alpha = 0;
//        popup.alpha = 0;
//        foreach (var btn in btns)
//        {
//            btn.alpha = 0;
//        }
//        foreach (var btnRectTransform in btnsRectTransform)
//        {
//            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
//        }
//        popupRectTransform.anchoredPosition = new Vector2(0, -350);

//    }
//    private void Start()
//    {
//        popupRectTransform.anchoredPosition = new Vector2(0, -350);
//        ShowPopup();
//    }
//    public void ShowPopup()
//    {
//        SetInit();


//        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
//        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
//        popupRectTransform.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

//        float delay = 0.1f; // Initial delay
//        Debug.Log(bg);
//        Debug.Log(popup);
//        Debug.Log(popupRectTransform);
//        for (int i = 0; i < btns.Length; i++)
//        {
//            // Ensure starting conditions
//            btns[i].alpha = 0;
//            btnsRectTransform[i].localScale = Vector3.one * 0.6f;

//            // Fade In
//            btns[i].DOFade(1f, 0.3f)
//                .SetEase(Ease.OutBack)
//                .SetDelay(delay);

//            // Scale Up
//            btnsRectTransform[i].DOScale(1f, 0.3f)
//                .SetEase(Ease.OutBack)
//                .SetDelay(delay);

//            delay += delayIncrement; // Increase delay for next button
//        }
//    }
//    public void ClosePopup()
//    {
//        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
//        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
//        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);

//    }
//    void RemoveThis()
//    {
//        PopupManager.instance.TogglePopup(PopupManager.instance.purchasedItemPopup);
//    }
//    void ShowPurchasedProduct()
//    {
//        switch(FBPlayerData.instance.productID)
//        {
//            case "no_ads_30_days":

//                break;
//            case "coins_2000":
//                purchasedItems[0].SetActive(true);
//                CoinManager.instance.AddCoins(2000);
//                break;
//            case "coins_6000":
//                purchasedItems[1].SetActive(true);
//                CoinManager.instance.AddCoins(6000);
//                break;
//            case "coins_16000":
//                purchasedItems[2].SetActive(true);
//                CoinManager.instance.AddCoins(16000);
//                break;
//            case "coins_34000":
//                purchasedItems[3].SetActive(true);
//                CoinManager.instance.AddCoins(34000);
//                break;
//            case "coins_70400":
//                purchasedItems[4].SetActive(true);
//                CoinManager.instance.AddCoins(70400);
//                break;
//        }
//    }


//}

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


    public Button[] buttons = null;


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
        popupRectTransform.anchoredPosition = new Vector2(0, -350);

    }
    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);
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
        popupRectTransform.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

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
        foreach (var btn in buttons)
        {
            btn.enabled = false;
        }
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);

    }
    public void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.purchasedItemPopup);

    }

}

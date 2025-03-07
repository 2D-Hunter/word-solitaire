using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public RectTransform[] images; // Assign 4 UI Image RectTransforms
    private float scaleUpSize = 0.85f; // How much to scale up
    private float scaleDuration = 0.2f; // Duration of scale animation
    private float waitTime = 5f; // Delay between animations
    // Start is called before the first frame update

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.1f; // Adjust delay between each button if needed

    public CanvasGroup header = null;
    public RectTransform headerRectTransform = null;
    private RectTransform selectedImage;
    public GameObject noAds30Days = null;
    public GameObject noAds30Days_lock = null;

    private void Awake()
    {
        ShowPopup();
    }
    private void SetInit()
    {
        if(FBPlayerData.instance.NO_ADS_30_DAYS)
        {
            noAds30Days.SetActive(false);
            noAds30Days_lock.SetActive(true);
        }
        else
        {
            noAds30Days.SetActive(true);
            noAds30Days_lock.SetActive(false);
        }
            
        noAds30Days_lock.SetActive(FBPlayerData.instance.NO_ADS_30_DAYS);
        if (FBPlayerData.instance.NO_ADS_30_DAYS)
        header.alpha = 0;
        headerRectTransform.anchoredPosition = Vector2.zero;
        foreach (var btn in btns)
        {
            btn.alpha = 0;
        }
        foreach (var btnRectTransform in btnsRectTransform)
        {
            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        }

        
    }

    IEnumerator StartRandomScaling()
    {
        
        
        yield return new WaitForSeconds(3f);
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            int randomIndex = Random.Range(0, images.Length); // Pick a random image
            selectedImage = images[randomIndex];

            // Scale up and down animation
            selectedImage.DOScale(scaleUpSize, scaleDuration).SetEase(Ease.OutBack)
                .OnComplete(() => selectedImage.DOScale(0.75f, 0.5f).SetEase(Ease.InElastic));
        }
    }
    
    public void BuyProduct(string productId)
    {
        FBPlayerData.instance.VibrationEffect();
        Debug.Log("Buy Product Product ID: "+productId);
        InitManager.instance.startShopping = true;
        PopupManager.instance.TogglePopup(PopupManager.instance.loading);
        if (GameUtils.IsFacebookBuild())
            Application.ExternalCall("CallInAppPurchase", productId);
        else
            StartCoroutine(GetProductAfterPurchase(productId));

    }
    IEnumerator GetProductAfterPurchase(string productId)
    {
        yield return new WaitForSeconds(3f);
        FBPlayerData.instance.GetProductsAfterPurchase(productId);
    }

    public void ShowPopup()
    {
        SetInit();
        header.DOFade(1f, 0.3f).SetEase(Ease.OutBack);
        headerRectTransform.DOAnchorPosY(-283f, 0.3f).SetEase(Ease.OutBack);
        float delay = 0f; // Initial delay

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
        StartCoroutine(StartRandomScaling());
    }
    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.ToggleShop();
    }
    private void OnDestroy()
    {
        header?.DOKill();
        headerRectTransform?.DOKill();
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i]?.DOKill();
            btnsRectTransform[i]?.DOKill();
        }
        selectedImage?.DOKill();
    }

}

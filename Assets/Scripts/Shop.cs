using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public RectTransform[] images; // Assign 4 UI Image RectTransforms
    private float scaleUpSize = 1.1f; // How much to scale up
    private float scaleDuration = 0.2f; // Duration of scale animation
    private float waitTime = 5f; // Delay between animations
    // Start is called before the first frame update

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.1f; // Adjust delay between each button if needed

    public CanvasGroup header = null;
    public RectTransform headerRectTransform = null;

    private void Awake()
    {
        ShowPopup();
    }
    private void SetInit()
    {
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
            RectTransform selectedImage = images[randomIndex];

            // Scale up and down animation
            selectedImage.DOScale(scaleUpSize, scaleDuration).SetEase(Ease.OutBack)
                .OnComplete(() => selectedImage.DOScale(0.8f, 0.5f).SetEase(Ease.InElastic));
        }
    }
    public void BuyProduct(string productId)
    {
        //IAPManager.instance.BuyProduct(productId);
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
        ShopManager.instance.ToggleShop();
    }

}

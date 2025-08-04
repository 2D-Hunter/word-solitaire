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
    public GameObject coinAnim1;
    public RectTransform parentPanel;
    int counter = 10;

    private void Awake()
    {
        
        //coinText.text = shadowText.text = counter.ToString();
    }

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
        FBPlayerData.instance.VibrationEffect();
        if (CardManager.instance.rightSideCards.Count > 1)
        {
            if(FBPlayerData.instance.TOTAL_COINS >= counter)
            {
                // Instantiate UI prefab under the parent
                GameObject newUIElement = Instantiate(coinAnim1, parentPanel);
                TextMeshProUGUI[] textComponents = newUIElement.GetComponentsInChildren<TextMeshProUGUI>();

                if (textComponents.Length >= 2)
                {
                    textComponents[0].text = textComponents[1].text = "-" + counter.ToString();
                }
                else
                {
                    Debug.LogWarning("Prefab does not have enough TextMeshProUGUI components!");
                }

                // Get its RectTransform
                RectTransform rectTransform = newUIElement.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    // Set initial position (off-screen or lower)
                    Vector3 startPos = rectTransform.anchoredPosition;
                    startPos.y -= 50;
                    rectTransform.anchoredPosition = startPos;
                    SoundManager.instance.PlaySFX("CoinSpend1");
                    // Tween Y movement smoothly
                    rectTransform.DOAnchorPosY(startPos.y + 80, 0.5f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        Destroy(newUIElement);
                        newUIElement = null;
                    });
                }
                CoinManager.instance.SpendCoins(counter);
                AnalyticsManager.Instance.TrackCoinsSpent("Flip back Extra Card", counter);
                counter += 5;
                UpdateText();
                //var card = CardManager.instance.rightSideCards.RemoveAt(CardManager.instance.rightSideCards.Count-1);
                var card = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
                card.FlipCardBack();
                CardManager.instance.rightSideCards.RemoveCard(card);
                GameCoinHud.instance.Show();
            }
            else
            {
                PopupManager.instance.ToggleShop();
            }
            
        }
    }
    void UpdateText()
    {
        coinText.text = shadowText.text = counter.ToString();

            //Card.instance.FlipCardBack();
        

    }
}

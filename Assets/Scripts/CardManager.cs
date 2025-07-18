using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public List<Card> allCards;
    public List<Card> allFaceUpCards;
    public List<Card> extraCards;
    public List<Card> rightSideCards;
    public List<Card> totalCardsToClear;
    public int totalCardToGet;

    public GameObject cardContainer = null;

    public RectTransform startPoint; // Assign this in the Inspector


    private void Awake()
    {
        Debug.Log("All Cards: ");
        instance = this;
        


    }
    private void Start()
    {
        Debug.Log("____Card Data CardManager");
        MakeRandomCardWild();
        //AnimateCardsFromDeck(); // 🔥 Fan out effect
    }

    public void AnimateCardsFromDeck()
    {
        float moveDuration = 0.5f;
        float delayBetweenCards = 0.1f;

        for (int i = 0; i < totalCardsToClear.Count; i++)
        {
            Card card = totalCardsToClear[i];
            RectTransform rect = card.GetComponent<RectTransform>();
            if (rect == null) continue;

            Vector2 finalPos = rect.anchoredPosition;

            // Step 1: Instantly move card to deck/start point
            rect.anchoredPosition = startPoint.anchoredPosition;

            // Step 2: Animate it back to its original position with delay
            rect.DOAnchorPos(finalPos, moveDuration)
                .SetDelay(i * delayBetweenCards)
                .SetEase(Ease.OutQuad);
        }
    }
    void MakeRandomCardWild()
    {
        if (extraCards.Count == 0 || FBPlayerData.instance.CURRENT_LEVEL < 15) return;
        int randomIndex = Random.Range(0, extraCards.Count);
        Card randomCard = extraCards[randomIndex];
        randomCard.SetAsWild();
        Debug.Log("The wild card is: " + randomCard.name);
    }
    public void AddAllCardsToList()
    {
        allCards = FindObjectsOfType<Card>().ToList();
    }
    public void AddTotalCardsToClearInList()
    {
        Debug.Log("cardContainer.GetComponentsInChildren<Card>(): " + cardContainer.GetComponentsInChildren<Card>().Length);
        totalCardsToClear.AddRange(cardContainer.GetComponentsInChildren<Card>());
    }
    public void UpdateFaceUpCards(Card card, bool isFaceUp)
    {
        //Debug.Log("Face-up cards: " + isFaceUp);
        if (isFaceUp)
        {
            if (!allFaceUpCards.Contains(card) && !card.isWildCard)
                allFaceUpCards.Add(card);
        }
        else
        {
            if (allFaceUpCards.Contains(card))
                allFaceUpCards.Remove(card);
        }

        
    }
    public void AssignExtraCards()
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            char[] letters = { 'A', 'C', 'P', 'T', 'O', 'S', 'A', 'S', 'R', 'J' };

            for (int i = 0; i < extraCards.Count; i++)
            {
                //if (extraCards[i] == null)
                //{
                //    Debug.LogError($"Extra Card at index {i} is null!");
                //    continue;
                //}

                //// Ensure cardData is assigned
                //if (extraCards[i].cardData == null)
                //{
                //    Debug.LogWarning($"CardData at index {i} is null! Assigning now...");
                //    extraCards[i].cardData = extraCards[i].gameObject.AddComponent<CardData>();
                //}

                //if (extraCards[i].cardData.letterText == null || extraCards[i].cardData.valueText == null)
                //{
                //    Debug.LogError($"Text components missing in CardData at index {i}!");
                //    continue;
                //}

                // Assign values

                extraCards[i].cardData.letterText.text = letters[i].ToString();
                int cardValue = extraCards[i].cardData.GetCardValue(letters[i]);
                extraCards[i].cardData.valueText.text = cardValue.ToString();
                extraCards[i].cardData.cardValue = cardValue;
            }
            rightSideCards[0].cardData.letterText.text = 'J'.ToString();
            int cardValue1 = rightSideCards[0].cardData.GetCardValue('J');
            rightSideCards[0].cardData.valueText.text = cardValue1.ToString();
            rightSideCards[0].cardData.cardValue = cardValue1;
        }
    }
    public void DisableAllCards()
    {
        Card[] cards = FindObjectsOfType<Card>();
        foreach (Card card in cards)
        {
            card.DisableClick();
        }
    }

    public void EnableAllCards()
    {
        Card[] cards = FindObjectsOfType<Card>();
        foreach (Card card in cards)
        {
            card.EnableClick();
        }
    }

}

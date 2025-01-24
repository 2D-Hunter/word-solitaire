using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public List<Card> allCards;
    public List<Card> allFaceUpCards;
    public List<Card> extraCards;
    public List<Card> rightSideCards;
    public int totalCardToGet;
    

    private void Awake()
    {
        Debug.Log("All Cards: ");
        instance = this;
        
    }
    private void Start()
    {
        MakeRandomCardWild();
    }
    void MakeRandomCardWild()
    {
        if (extraCards.Count == 0) return;
        int randomIndex = Random.Range(0, extraCards.Count);
        Card randomCard = extraCards[randomIndex];
        randomCard.SetAsWild();
        Debug.Log("The wild card is: " + randomCard.name);
    }
    public void AddAllCardsToList()
    {
        allCards = FindObjectsOfType<Card>().ToList();
    }
    public void UpdateFaceUpCards(Card card, bool isFaceUp)
    {
        Debug.Log("Face-up cards: " + isFaceUp);
        if (isFaceUp)
        {
            if (!allFaceUpCards.Contains(card))
                allFaceUpCards.Add(card);
        }
        else
        {
            if (allFaceUpCards.Contains(card))
                allFaceUpCards.Remove(card);
        }

        
    }

}

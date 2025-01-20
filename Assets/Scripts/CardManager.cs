using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public List<Card> allCards;

    public List<Card> extraCards;
    public List<Card> rightSideCards;
    public int totalCardToGet;

    private void Awake()
    {
        Debug.Log("All Cards: ");
        instance = this;
        
    }
    public void AddAllCardsToList()
    {
        allCards = FindObjectsOfType<Card>().ToList();
    }

}

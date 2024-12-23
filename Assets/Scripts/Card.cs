using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    private CardManager cardManager;
    
    void Start()
    {
         
        // Find the CardManager in the scene
        cardManager = FindObjectOfType<CardManager>();
    }

    public void OnCardClicked()
    {
        Debug.Log("____Card Clicked");
        // Call the CardManager's function to move this card
        

        RectTransform cardRect = GetComponent<RectTransform>();
        if (cardRect != null)
        {
            cardManager.OnCardClicked(cardRect);
        }
    }
}
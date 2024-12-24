using UnityEngine;
using UnityEngine.UI;
using TMPro;
[ExecuteAlways] // Ensures the script runs in edit mode
public class Card : MonoBehaviour
{
    private CardManager cardManager;
    public GameObject cardFace;
    public bool isFaceDown = true;

    private void OnValidate()
    {
        if (isFaceDown)
            cardFace.SetActive(true);
        else
            cardFace.SetActive(false);
    }
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
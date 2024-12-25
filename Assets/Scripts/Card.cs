using UnityEngine;
using UnityEngine.UI;
using TMPro;
[ExecuteAlways]
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
        cardManager = FindObjectOfType<CardManager>();
    }

    public void OnCardClicked()
    {
        Debug.Log("____Card Clicked");

        RectTransform cardRect = GetComponent<RectTransform>();
        if (cardRect != null)
        {
            cardManager.OnCardClicked(cardRect);
        }
    }
}
using UnityEngine;

public class CardSorting : MonoBehaviour
{
    private Canvas canvas;
    private int originalOrder;
    private Card card;
    private void Start()
    {
        card = GetComponent<Card>();

        canvas = GetComponent<Canvas>();
        originalOrder = canvas.sortingOrder;
        Debug.Log("___originalOrder: " + originalOrder);
    }

    public void BringToFront(int newOrder = 15)
    {
        if(card.tag != "ExtraCard")
            canvas.sortingOrder = newOrder;
    }

    public void ResetOrder()
    {
        Debug.Log("ResetOrder");
        if (card.tag != "ExtraCard")
            canvas.sortingOrder = card.Level+1;
    }
}
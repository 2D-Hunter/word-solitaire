using UnityEngine;

public class CardSorting : MonoBehaviour
{
    private Canvas canvas;
    private int originalOrder;
    private Card card;
    private void Awake()
    {
        card = GetComponent<Card>();

        canvas = GetComponent<Canvas>();
        originalOrder = canvas.sortingOrder;
        Debug.Log(card.name+"___originalOrder: " + originalOrder);
    }

    public void BringToFront(int newOrder = 9999)
    {
        if(card.tag != "ExtraCard")
            canvas.sortingOrder = newOrder;
    }

    public void ResetOrder()
    {
       // Debug.Log("ResetOrder111");
        if (FBPlayerData.instance.CURRENT_LEVEL < 4)
        {
            canvas.sortingOrder = originalOrder;
            return;
        }
        if (card.tag != "ExtraCard")
            canvas.sortingOrder = card.Level+1;
    }
}
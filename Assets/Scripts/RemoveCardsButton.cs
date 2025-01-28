using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RemoveCardsButton : MonoBehaviour
{
    public static RemoveCardsButton instance;
    public Button myButton;
    private string firstImage = "RemoveAllCards-1";
    private string secondImage = "RemoveAllCards-2";
    private void Start()
    {
        instance = this;
        //SwapImage();
    }
    
    public void SwapImage()
    {
        string imageToLoad = SlotManager.instance.isSlotOccupied[0] ? secondImage : firstImage;
        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
        myButton.enabled = SlotManager.instance.isSlotOccupied[0];
    }
    public void OnTapRemoveCards()
    {
        for (int i = 0; i < SlotManager.instance.slotsCard.Count; i++)
        {
            
            SlotManager.instance.OnCardClicked(SlotManager.instance.slotsCard[i]);
            if (SlotManager.instance.goingBack)
            {
                GameObject cardContainer = GameObject.Find("UI-Panel/Levels");
                cardContainer.transform.SetAsLastSibling();
                Card.instance.FlipImmediateBelowCards();
            }
        }

    }
}

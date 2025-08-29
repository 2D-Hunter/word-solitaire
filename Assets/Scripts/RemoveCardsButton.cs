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
    public bool sendBackAll = false;
    private void Start()
    {
        instance = this;
        //gameObject.SetActive(false);
        SwapImage();
    }
    
    public void SwapImage()
    {
        if (SlotManager.instance.slotsCard.Count <= 0) return;
        string imageToLoad = SlotManager.instance.isSlotOccupied[0] ? secondImage : firstImage;
        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
        myButton.enabled = SlotManager.instance.isSlotOccupied[0];
    }
    public void OnTapRemoveCards()
    {
        
        FBPlayerData.instance.VibrationEffect();
        sendBackAll = true;
     
        SlotManager.instance.goingBack = true;
        for (int i = 0; i < SlotManager.instance.slotsCard.Count; i++)
        {
            var card = SlotManager.instance.slotsCard[i];
            SlotManager.instance.ReturnBackToDeck(card);
        }
      
    }
    private void SetGoBackValue()
    {
        return;
        for (int i = 0; i < SlotManager.instance.slotsCard.Count; i++)
        {
            var card = SlotManager.instance.slotsCard[i];

            card.FlipImmediateBelowCards();
            //card.MoveBackToOriginalPosition();




        }
        for (int j = 0; j < SlotManager.instance.slotsCard.Count; j++)
        {
            var card = SlotManager.instance.slotsCard[j];
            SlotManager.instance.slotsCard.Remove(card);
        }
    }
}

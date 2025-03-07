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
        FBPlayerData.instance.VibrationEffect();
        sendBackAll = true;
        //Debug.Log("SlotManager.instance.slotsCard.Count: "+ SlotManager.instance.slotsCard.Count);
        SlotManager.instance.goingBack = true;
        for (int i = 0; i < SlotManager.instance.slotsCard.Count; i++)
        {
            var card = SlotManager.instance.slotsCard[i];

            card.FlipImmediateBelowCards();
            card.MoveBackToOriginalPosition();
            //SlotManager.instance.ResetAfterCardBack(card);
            //SlotManager.instance.slotsCard.Remove(card);


        }
        //Debug.Log("SlotManager.instance.slotsCard.Countt: " + SlotManager.instance.slotsCard.Count);
        for (int j = 0; j < SlotManager.instance.slotsCard.Count; j++)
        {
            var card = SlotManager.instance.slotsCard[j];
            SlotManager.instance.ResetAfterCardBack(card);
        }
        //Debug.Log("SlotManager.instance.slotsCard.Counttt: " + SlotManager.instance.slotsCard.Count);
        //for (int k = 0; k < SlotManager.instance.slotsCard.Count; k++)
        //{
        //    Debug.Log("SlotManager.instance.slotsCard.Countttt");
        //    var card = SlotManager.instance.slotsCard[k];
        //    SlotManager.instance.slotsCard.RemoveAt(SlotManager.instance.slotsCard.Count-1);
        //}
        while (SlotManager.instance.slotsCard.Count > 0)
        {
            Debug.Log("SlotManager.instance.slotsCard.Countttt");
            SlotManager.instance.slotsCard.RemoveAt(SlotManager.instance.slotsCard.Count - 1);
        }
        SlotManager.instance.AAA();

        //Invoke("SetGoBackValue", 0.2f);
    }
    private void SetGoBackValue()
    {
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

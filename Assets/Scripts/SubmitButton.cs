using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubmitButton : MonoBehaviour
{
    public static SubmitButton instance;
    public Button myButton;
    private string firstImage = "btn-1";
    private string secondImage = "btn-3";
    //private string tutorialImage = "btn-2";

    public TextMeshProUGUI pointTxt;
    public TextMeshProUGUI pointTxtShadow;
    private void Start()
    {
        instance = this;
        SwapImage();
    }
    public void SwapImage()
    {
        string imageToLoad = GameManager.instance.isValidWord ? secondImage : firstImage;
        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
        if (GameManager.instance.isValidWord)
        {
            pointTxt.text = SlotManager.instance.GetSlotPoints().ToString() + "<size=55>pts</size>";
            pointTxtShadow.text = SlotManager.instance.GetSlotPoints().ToString() + "<size=55>pts</size>";
        }
        else
        {
            pointTxt.text = "";
            pointTxtShadow.text = "";
        }
        myButton.enabled = GameManager.instance.isValidWord;
    }
    public void OnTapSubmit()
    {
        FBPlayerData.instance.VibrationEffect();
        SlotManager.instance.StartCoroutine(SlotManager.instance.SubmitWord());
    }
}

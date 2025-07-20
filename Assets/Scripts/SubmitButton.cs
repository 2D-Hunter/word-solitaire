using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SubmitButton : MonoBehaviour
{
    public static SubmitButton instance;
    public Button myButton;
    private string firstImage = "btn-1";
    private string secondImage = "btn-3";
    private string tutorialImage = "btn-2";

    public TextMeshProUGUI pointTxt;
    public TextMeshProUGUI pointTxtShadow;

    public Tutorial tutorial;
    public EventTrigger eventTrigger;
    public GameObject spriteMask;
    private void Start()
    {
        instance = this;
        if(FBPlayerData.instance.CURRENT_LEVEL == 1)
            spriteMask.SetActive(true);
        else
            spriteMask.SetActive(false);
        eventTrigger = GetComponent<EventTrigger>();
        if (tutorial == null)
            tutorial = FindObjectOfType<Tutorial>(); // Auto-assign
        SwapImage();
    }
    public void SwapImage()
    {
        string imageToLoad;
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
            imageToLoad = GameManager.instance.isValidWord ? tutorialImage : firstImage;
        else
            imageToLoad = GameManager.instance.isValidWord ? secondImage : firstImage;

        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
        if (GameManager.instance.isValidWord && (FBPlayerData.instance.CURRENT_LEVEL != 1 && FBPlayerData.instance.CURRENT_LEVEL != 2))
        {
            pointTxt.text = SlotManager.instance.GetSlotPoints().ToString() + "<size=55>pts</size>";
            pointTxtShadow.text = SlotManager.instance.GetSlotPoints().ToString() + "<size=55>pts</size>";

            //GameManager.instance.foundWords.Add(SlotManager.instance.GetSlotString());
            //WordnikDefinition.instance.FetchDefinition(SlotManager.instance.GetSlotString().ToLower());
        }
        else
        {
            pointTxt.text = "";
            pointTxtShadow.text = "";
        }
        myButton.enabled = eventTrigger.enabled = GameManager.instance.isValidWord;
    }
    public void OnTapSubmit()
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 && !FBPlayerData.instance.TUTORIAL_1_COMPLETED)
        {
            if(InitManager.instance.tutorialCntr == 5)
            {
                tutorial.infoPanel.SetActive(false);
            }
            else
            {
                GameManager.instance.hud1.SetActive(true);
                tutorial.handRectTransform.gameObject.SetActive(false);
                tutorial.infoPanel.SetActive(false);
                tutorial.infoPanel2.SetActive(true);
                tutorial.infoPanel2.GetComponent<RectTransform>().localScale = Vector3.zero;
                tutorial.infoPanel2.GetComponent<RectTransform>().DOScale(0.95f, 0.3f).SetEase(Ease.OutBack);
                spriteMask.SetActive(false);
            }
            
        }
        else if (FBPlayerData.instance.CURRENT_LEVEL == 2 && InitManager.instance.tutorialCntr == 3 && !FBPlayerData.instance.TUTORIAL_2_COMPLETED)
        {
            foreach (var card in CardManager.instance.extraCards)
            {
                card.GetComponent<Button>().enabled = true;

            }
            tutorial.infoPanel.SetActive(false);
            spriteMask.SetActive(false);
            tutorial.gameObject.SetActive(false);
            

        }
        FBPlayerData.instance.VibrationEffect();
        SlotManager.instance.StartCoroutine(SlotManager.instance.SubmitWord());
    }
}

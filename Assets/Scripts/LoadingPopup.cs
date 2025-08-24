using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingPopup : MonoBehaviour
{
    private string[] loadingMessages = {
        // 🎯 Friendly & Game-Oriented
        "Shuffling the cards for your next challenge...",
        "Laying out the puzzle pieces...",
        "Preparing your next adventure...",
        "Generating a fresh board just for you...",
        "Picking the perfect letters...",
        "Arranging tiles for maximum fun...",

        // 🌍 Adventure / Journey Feel
        "Packing your bags for the next destination...",
        "Unfolding a new map of mysteries...",
        "Exploring new paths ahead...",
        "Setting the stage for your journey...",
        "Unlocking the gates to the next world...",

        // ⚡ Motivational / Fun
        "Sharpening your mind for the next puzzle...",
        "Charging up the challenge meter...",
        "Mixing a cocktail of fun and strategy...",
        "Your next brain workout is almost ready...",
        "Get ready… brilliance awaits!"
    };
    public GameObject loadingTxtObj = null;
    public TextMeshProUGUI loadingTxt = null;
    public CanvasGroup bgCanvasGroup = null;

    private void Start()
    {
        if(InitManager.instance.CurrentScene == "Menu")
        {
            loadingTxtObj.SetActive(true);
            bgCanvasGroup.alpha = 0.8f;
            loadingTxt.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
        }
        else
        {
            loadingTxtObj.SetActive(false);
            bgCanvasGroup.alpha = 0.6f;
        }
        
        Debug.Log("InitManager.instance.deleteData: "+ InitManager.instance.deleteData);
        if(InitManager.instance.deleteData)
        {
            Invoke("DataDeleted", 3f);
        }
    }
    void DataDeleted()
    {
        InitManager.instance.deleteData = false;
        PopupManager.instance.TogglePopup(PopupManager.instance.loading);
        PopupManager.instance.TogglePopup(PopupManager.instance.accountDeletedPopup);
    }
}

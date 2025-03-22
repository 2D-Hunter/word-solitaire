using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject overlayPanel;
    public GameObject dictionary = null;
    public bool isValidWord = false;
    public bool foundValidWord = false;
    public string hintWord = "";
    public int totalPoint = 0;
    public GameObject backButton;
    public string hintText;

    public GameObject settingsPopupGame;
    public GameObject quitPopup;
    public int scoreMultiplier = 1;

    public RectTransform uiContainer;
    public GameObject levelUpPrefab;
    private GameObject currentLevelupUI;
    public GameObject moreCards;
    public GameObject endGame;

    //Tutorial
    public GameObject hud;
    public GameObject secondRow;
    public GameObject extraCardsSlots;
    public GameObject bottomIcons;
    public GameObject tutorialPatch;


    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        instance = this;
    }
    private void Start()
    {
        if(FBPlayerData.instance.CURRENT_LEVEL == 1)
        {
            hud.SetActive(false);
            secondRow.SetActive(false);
            extraCardsSlots.SetActive(false);
            bottomIcons.SetActive(false);
        }
        //Invoke("ToggleLevelup", 1f);
        if(PopupManager.instance)
            PopupManager.instance.AssignUIContainer();
    }


    public void ShowDictionary()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.dictionaryPopup);
    }
    public void ShowBackButton()
    {
        if (!backButton.activeSelf)
        {
            backButton.SetActive(true);
            BackButton.instance.ShowThis();
        }

    }

    public void ShowMoreCardsToBuy()
    {
        moreCards.SetActive(true);
        endGame.SetActive(true);
    }
    public void HideMoreCardsToBuy()
    {
        moreCards.SetActive(false);
        endGame.SetActive(false);
    }
    public void ShowSettingsPopupGame()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
    }
    public void ShowQuitPopup()
    {
        FBPlayerData.instance.VibrationEffect();
        quitPopup.SetActive(true);
        QuitPopup.instance.ShowPopup();
    }

    public void ToggleLevelup()
    {
        if (currentLevelupUI == null) // If shop is not open, instantiate it
        {
            currentLevelupUI = Instantiate(levelUpPrefab, uiContainer);
        }
        else // If shop is open, close it
        {
            Destroy(currentLevelupUI);
            currentLevelupUI = null;
        }
    }

    
}

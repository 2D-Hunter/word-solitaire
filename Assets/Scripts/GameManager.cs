using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject overlayPanel;
    public GameObject dictionary = null;
    public bool isValidWord = false;
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
        //Invoke("ToggleLevelup", 1f);
        PopupManager.instance.AssignUIContainer();
    }


    public void ShowDictionary()
    {
        dictionary.SetActive(true);
        Dictionary.instance.ShowPopup();
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
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
    }
    public void ShowQuitPopup()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject overlayPanel;
    public bool isValidWord = false;
    public bool foundValidWord = false;
    public string hintWord = "";
    public int totalPoint = 0;
    public GameObject backButton;
    public string hintText;

    public int scoreMultiplier = 1;

    public RectTransform uiContainer;
    public GameObject levelUpPrefab;
    private GameObject currentLevelupUI;
    public GameObject moreCards;
    public GameObject endGame;

    //Tutorial
    public GameObject hud1;
    public GameObject hud2;
    public GameObject submitBtn;
    public GameObject hintBtn;
    public GameObject removeCardsBtn;
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
            hud1.SetActive(false);
            hud2.SetActive(false);
            submitBtn.SetActive(false);
            hintBtn.SetActive(false);
            removeCardsBtn.SetActive(false);
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
        PopupManager.instance.TogglePopup(PopupManager.instance.quitPopup);
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
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) // Detect touch or click
    //    {
    //        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        int layerMask = LayerMask.GetMask("CardLayer"); // Only detect cards

    //        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layerMask);

    //        if (hit.collider != null)
    //        {
    //            Debug.Log("Tapped on: " + hit.collider.gameObject.name);
    //        }
    //        else
    //        {
    //            Debug.Log("Tapped outside the card");
    //        }
    //    }
    //}


}

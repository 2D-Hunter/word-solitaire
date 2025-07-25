using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;

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
    public bool isEndGamePressed = false;
    public GameObject bonusHud;

    //Tutorial
    public GameObject hud1;
    public GameObject hud2;
    public GameObject submitBtn;
    public GameObject hintBtn;
    public GameObject removeCardsBtn;
    public GameObject extraCardsSlots;
    public GameObject bottomIcons;
    public GameObject wildCardBtn;
    public GameObject tutorialPatch;
    public GameObject tutorial;
    public GameObject boosterTutorial;
    public GameObject hudMask;


    public Card moreCardPrefab;  // Assign the Card Prefab in the Inspector
    public Transform parentPanel;

    private float[] targetPositionsOfMoreCards = { -180f, -165f, -150f, -135f, -120f };
    public BackgroundManager backgroundManager;
    private NumberOfWildCard numberOfWildCard;
    public RectTransform wildCardTab;
    public GameObject settingBtn_secondRow;
    public GameObject settingBtn;
    public GameObject multiplayerHud;
    public GameObject countdownTimer;

    public RectTransform _slots, _extraCards, _greenTab, _secondRow, _levels, _bottomLeft, _bottomRight;
    public bool isCountdownTimerDone = false;
    public GameObject connectionPopup = null;
    public bool animateBonusTarget = false;
    public int gainedPoint;
    public int wordCounter = 0;
    public int submittedWordLength = 0;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public List<string> foundWords = new List<string>();
    //public Dictionary<string, string> wordDefinitions = new Dictionary<string, string>();
    public Dictionary<string, string> wordDefinitions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public int currentIndex = 0;
    public List<string> ReversedFoundWords => foundWords.AsEnumerable().Reverse().ToList();
    public HashSet<string> definitionsBeingFetched = new HashSet<string>();
    public GameObject levelupStars = null;
    public int earnedStarsInTheLevel = 0;

    public CanvasGroup[] allGameStuffs = null;


    private void Awake()
    {
        connectionPopup.SetActive(false);
        numberOfWildCard = FindObjectOfType<NumberOfWildCard>();
        backgroundManager.GetComponent<BackgroundManager>().OnLevelChanged(FBPlayerData.instance.CURRENT_LEVEL);
        InitManager.instance.CurrentScene = "Game";
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
        InitManager.instance.buyMoreCardsCntr = 1;
        InitManager.instance.moreCardsPrice = 150;
    }
    private void Start()
    {
        if(FBPlayerData.instance.CURRENT_LEVEL >= 11)
            wildCardBtn.SetActive(true);
        else
            wildCardBtn.SetActive(false);
        if(FBPlayerData.instance.CURRENT_LEVEL == 11)
        {
            boosterTutorial.SetActive(true);
        }
        else
            boosterTutorial.SetActive(false);

        if (FBPlayerData.instance.CURRENT_LEVEL == 1)
        {

            hud1.SetActive(false);
            hud2.SetActive(false);
            submitBtn.SetActive(false);
            hintBtn.SetActive(false);
            removeCardsBtn.SetActive(false);
            extraCardsSlots.SetActive(false);
            bottomIcons.SetActive(false);
            tutorial.SetActive(true);
        }
        else if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            hud2.SetActive(false);
            hintBtn.SetActive(false);
            removeCardsBtn.SetActive(false);
            bottomIcons.SetActive(false);
            tutorial.SetActive(true);
        }
        else
        {
            tutorial.SetActive(false);
            hudMask.SetActive(false);
        }
        if (FBPlayerData.instance.CURRENT_LEVEL >= 26)
            bonusHud.SetActive(true);
        else
            bonusHud.SetActive(false);

        Debug.Log("MultiplayerEventHandler.Instance.isMultiplayer: "+ MultiplayerEventHandler.Instance.isMultiplayer);
        if (MultiplayerEventHandler.Instance.isMultiplayer)
        {
            countdownTimer.SetActive(true);
            multiplayerHud.SetActive(true);
            hud1.SetActive(false);
            hud2.SetActive(false);
            hintBtn.SetActive(false);
            wildCardBtn.SetActive(false);
            settingBtn.SetActive(false);
            //settingBtn_secondRow.SetActive(true);
            //_slots.anchoredPosition = new Vector2(_slots.anchoredPosition.x, 328f);
            //_extraCards.anchoredPosition = new Vector2(_extraCards.anchoredPosition.x, 565f);
            //_greenTab.anchoredPosition = new Vector2(_greenTab.anchoredPosition.x, 324f);
            //_secondRow.anchoredPosition = new Vector2(_secondRow.anchoredPosition.x, 149f);
            //_levels.anchoredPosition = new Vector2(_levels.anchoredPosition.x, 1124f);///1222
            //_bottomLeft.anchoredPosition = new Vector2(_bottomLeft.anchoredPosition.x, 149f);
            //_bottomRight.anchoredPosition = new Vector2(_bottomRight.anchoredPosition.x, 149f);
        }
        else
        {
            countdownTimer.SetActive(false);
        }
        //Invoke("ToggleLevelup", 1f);
        if (PopupManager.instance)
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
    public void TapEndGame()
    {
        isEndGamePressed = true;
        PopupManager.instance.TogglePopup(PopupManager.instance.quitPopup);
    }
    
    public void SpawnCards()
    {
        StartCoroutine(SpawnAndMoveCards());
    }
    private IEnumerator SpawnAndMoveCards()
    {
        Card[] cards = new Card[5];

        // Get the current highest index to continue from (assume 10 cards already placed)
        int startingIndex = CardData.letterBatches.Count * 10;

        // Step 1: Spawn all cards instantly at x = -400
        for (int i = 0; i < 5; i++)
        {
            Card newCard = Instantiate(moreCardPrefab, parentPanel);
            newCard.tag = "ExtraCard";
            CardManager.instance.extraCards.Add(newCard);

            // ✅ Assign unique cardIndex here
            CardData data = newCard.GetComponent<CardData>();
            data.cardIndex = startingIndex + i;

            // Set image and initial position
            RectTransform cardTransform = newCard.GetComponent<RectTransform>();
            cardTransform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("ExtraCard");
            cardTransform.anchoredPosition = new Vector2(-400, cardTransform.anchoredPosition.y);

            cards[i] = newCard;
        }

        // Step 2: Wait briefly
        yield return new WaitForSeconds(0.3f);

        // Step 3: Move cards to their target positions
        for (int i = 0; i < 5; i++)
        {
            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();
            float delay = i * 0.1f;
            cardTransform.DOAnchorPosX(targetPositionsOfMoreCards[i], 0.25f)
                         .SetEase(Ease.OutExpo)
                         .SetDelay(delay);

            DOVirtual.DelayedCall(delay, () => PlayCardShuffleSound());
        }
    }
    void PlayCardShuffleSound()
    {
        Debug.Log("PlayCardShuffleSound");
        SoundManager.instance.PlaySFX("CardShuffle", 0.3f);
    }
    public void TapMoreCards()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.moreCardsPopup);
//        if(GameUtils.IsFacebookBuild())
//        {
//#if UNITY_EDITOR
//            FBPlayerData.instance.Get5CardsAfterVideoAd();
//            return;
//#endif
//            Application.ExternalCall("ShowAd_Reward", "MoreCards");
//        }
//        else
//        {
//            //if (FBPlayerData.instance.TOTAL_COINS >= 150)
//            //{
//            //    InitManager.instance.buyMoreCardsCntr++;
//            //    HideMoreCardsToBuy();
//            //    CoinManager.instance.SpendCoins(InitManager.instance.moreCardsPrice);
//            //    SpawnCards();
//            //}
//            //else
//            //{
//            //    PopupManager.instance.ToggleShop();
//            //}
//        }
        
    }
    public void Get5CardsAfterVideoAd()
    {
        HideMoreCardsToBuy();
        SpawnCards();
    }
    public void TapWildCardBtn()
    {
        //if(FBPlayerData.instance.TOTAL_WILD_CARD >= 1)
        //{
        //    FBPlayerData.instance.TOTAL_WILD_CARD--;
        //    numberOfWildCard.UpdateWildCard();
        //    FBPlayerData.instance.SavePlayerData();
        //}
        if(boosterTutorial.activeSelf)
        {
            FindObjectOfType<BoosterTutorial>().StopHandAnim();
            boosterTutorial.SetActive(false);
        }
        
        PopupManager.instance.TogglePopup(PopupManager.instance.wildcardPopup);
    }
    public void UseWildCardNow()
    {
        Debug.Log("_____Use Wild Card Now...");
        
        SpawnWildCard();


    }
    public void WildCardBackToCollection()
    {
        Debug.Log("_____Wild Card Back To Collection.");
    }
    public void SpawnWildCard()
    {
        // Instantiate card
        Card newCard = Instantiate(moreCardPrefab, uiContainer);
        newCard.tag = "WildCard";
        newCard.isWildCard = true;
        newCard.isFaceUp = true;

        RectTransform cardTransform = newCard.GetComponent<RectTransform>();

        // Update visuals
        cardTransform.GetChild(2).gameObject.SetActive(false);
        cardTransform.GetChild(3).gameObject.SetActive(true);

        // Convert tab position to the local position of the new card's parent
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiContainer,
            RectTransformUtility.WorldToScreenPoint(null, wildCardTab.position),
            null,
            out localPoint
        );

        cardTransform.anchoredPosition = localPoint;
        cardTransform.localRotation = wildCardTab.localRotation;
        cardTransform.localScale = Vector3.one * 0.71f;

        SlotManager.instance.OnCardClicked(newCard);
        newCard.GetComponent<RectTransform>().SetAsLastSibling();
    }
    public void ShowConnectionPopup()
    {
        connectionPopup.SetActive(true);
    }
    public void HideConnectionPopup()
    {
        connectionPopup.SetActive(false);
    }
    public void StoreWords(string word)
    {
        foundWords.Add(word);
        currentIndex = Mathf.Max(0, foundWords.Count - 1); // Start with latest word
    }
    public void SaveDefinition(string word, string definition)
    {
        Debug.Log("___SaveDefinition: " + word);
        Debug.Log("___SaveDefinition: " + definition);
        if (!wordDefinitions.ContainsKey(word))
        {
            wordDefinitions[word] = definition;
        }
    }

}

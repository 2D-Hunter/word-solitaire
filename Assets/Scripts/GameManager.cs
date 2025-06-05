using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

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
    

    public Card moreCardPrefab;  // Assign the Card Prefab in the Inspector
    public Transform parentPanel;

    private float[] targetPositionsOfMoreCards = { -180f, -165f, -150f, -135f, -120f };
    public BackgroundManager backgroundManager;
    private NumberOfWildCard numberOfWildCard;


    private void Awake()
    {
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
        //wildCardBtn.SetActive(false);
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

        // Step 1: Spawn all cards instantly at x = -400
        for (int i = 0; i < 5; i++)
        {
            Card newCard = Instantiate(moreCardPrefab, parentPanel);
            newCard.tag = "ExtraCard";
            CardManager.instance.extraCards.Add(newCard);
            RectTransform cardTransform = newCard.GetComponent<RectTransform>();
            cardTransform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("ExtraCard");
            // Set initial position
            cardTransform.anchoredPosition = new Vector2(-400, cardTransform.anchoredPosition.y);

            cards[i] = newCard;
            
        }

        // Step 2: Wait for 0.5 seconds
        yield return new WaitForSeconds(0.3f);

        // Step 3: Start tweening all cards to their target positions
        for (int i = 0; i < 5; i++)
        {
            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();
            float delay = i * 0.1f;
            cardTransform.DOAnchorPosX(targetPositionsOfMoreCards[i], 0.25f).SetEase(Ease.OutExpo).SetDelay(delay);
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
        if(GameUtils.IsFacebookBuild())
        {
#if UNITY_EDITOR
            FBPlayerData.instance.Get5CardsAfterVideoAd();
            return;
#endif
            Application.ExternalCall("ShowAd_Reward", "MoreCards");
        }
        else
        {
            //if (FBPlayerData.instance.TOTAL_COINS >= 150)
            //{
            //    InitManager.instance.buyMoreCardsCntr++;
            //    HideMoreCardsToBuy();
            //    CoinManager.instance.SpendCoins(InitManager.instance.moreCardsPrice);
            //    SpawnCards();
            //}
            //else
            //{
            //    PopupManager.instance.ToggleShop();
            //}
        }
        
    }
    public void Get5CardsAfterVideoAd()
    {
        HideMoreCardsToBuy();
        SpawnCards();
    }
    public void TapWildCardBtn()
    {
        if(FBPlayerData.instance.TOTAL_WILD_CARD >= 1)
        {
            FBPlayerData.instance.TOTAL_WILD_CARD--;
            numberOfWildCard.UpdateWildCard();
            FBPlayerData.instance.SavePlayerData();
        }
    }
    
}

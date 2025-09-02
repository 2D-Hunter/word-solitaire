using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;
using Word;

public class SlotManager : MonoBehaviour
{
    public static SlotManager instance;
    public List<RectTransform> cardSlots;
    private Dictionary<RectTransform, Transform> slotToCardMap;
    public List<bool> isSlotOccupied;
    //private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();
    //private Dictionary<RectTransform, Vector2> originalPositions1 = new Dictionary<RectTransform, Vector2>();
    public List<Card> slotsCard;
    public List<Card> ExtraInSlotcards = new List<Card>();
    public Vector2 finalPos;
    private int targetAchieve;

    private float tweenDuration = 0.8f;
    private float delayBetweenTweens = 0.15f;

    public bool allSlotsOccupied = false;
    public bool goingBack = false;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Transform point4;
    [SerializeField]
    private AnimationCurve customEase = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.75f),
            new Keyframe(1, 1)
        );

    public AnimationCurve customFadeEase = new AnimationCurve(
        new Keyframe(0, 1),    // Fully visible at the start
        new Keyframe(0.6f, 1), // Maintain full visibility until 70% of the duration
        new Keyframe(1, 0)     // Fade out to invisible at the end
    );

    public RectTransform slotRect;
     float animDuration = 0.3f;
     float height = 400f;
     Vector3 startScale = new Vector3(1.3f, 1.3f, 1.3f);
     Vector3 endScale = new Vector3(0.83f, 0.83f, 0.83f);
     Vector3 endScale_ExtraCard = new Vector3(0.875f, 0.875f, 0.875f);

    //int lastFilledSlot = -1;

    public GameObject trailEffectPrefab;
    private GameObject activeTrail_WildCard;
    private int extraCardCount = 0;
    //private int cardCount = 0;
    private StarProgressBar starProgressBar;
    public Card extraCard;
    public Tutorial tutorial;
    public string bestWord = "";
    public int bestScore = 0;
    public int tournamentCntr = 0;

    public BonusHud bonusHud;
    private CardSorting sorting;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        bestWord = "";
        bestScore = 0;
        isSlotOccupied = new List<bool>(new bool[cardSlots.Count]);
        slotToCardMap = new Dictionary<RectTransform, Transform>();
        //originalPositions = new Dictionary<RectTransform, Vector2>();

        StartCoroutine(StoreOriginalPositions());
        starProgressBar = FindObjectOfType<StarProgressBar>();
        if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            if (tutorial == null)
                tutorial = FindObjectOfType<Tutorial>();
        }


    }
    IEnumerator StoreOriginalPositions()
    {
        yield return new WaitForEndOfFrame();
        foreach (var card in FindObjectsOfType<Card>())
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                //originalPositions[cardRect] = cardRect.anchoredPosition;
                //Debug.Log("+++++++::: " + originalPositions[cardRect]);
                card.originalPosition = cardRect.anchoredPosition;
            }
        }
    }
    public void OnCardClicked(Card card)
    {
        Debug.Log("OnCardClicked");
        StartCoroutine(OnCardClicked_Coroutine(card));
    }

    public void ReturnBackToDeck(Card card)
    {
        
        StartCoroutine(ReturnBackToDeckRoutine(card));
    }

    private IEnumerator ReturnBackToDeckRoutine(Card card) 
    { 
        yield return new WaitForEndOfFrame();
     
        bool finished = false;
        GetSlotString();
        GameManager.instance.isValidWord = false;
        isSlotOccupied[0] = false;
        allSlotsOccupied = false;
        GameManager.instance.scoreMultiplier = 1;
        RemoveCardsButton.instance.SwapImage();
        GreenTabHandler.instance.HandleGreenTab(GetSlotString());
        SubmitButton.instance.SwapImage();
        DictionaryButton.instance.SwapImage();
        StartCoroutine(DelayedFlip(card, 0.3f));
        ResetAfterCardBack(card);
        slotsCard.Remove(card);
        card.GetComponent<Button>().enabled = true;
        CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        card.MoveBackToOriginalPosition(null, -1, false, () => {
            
            finished = true;
        });
        yield return new WaitUntil(() => finished);
        
        yield return new WaitForEndOfFrame();

    }
    private IEnumerator DelayedFlip(Card card, float delay)
    {
        yield return new WaitForSeconds(delay);
        card.FlipImmediateBelowCards();
        //yield return new WaitForEndOfFrame();
        //
    }

    private int sortingLayer = 0;
    public IEnumerator OnCardClicked_Coroutine(Card card)
    {
        

        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card.GetComponent<RectTransform>());
        Debug.Log("OnCardClicked_Coroutine: " + currentSlotIndex + " FBPlayerData.instance.CURRENT_LEVEL" + FBPlayerData.instance.CURRENT_LEVEL);
        if (currentSlotIndex != -1 && FBPlayerData.instance.CURRENT_LEVEL > 1)
        {
            
            
            Debug.Log("OnCardClicked inside if: ");
            //if (FBPlayerData.instance.CURRENT_LEVEL > 3)
            //{
                var sorting = card.GetComponent<CardSorting>();
                if (sorting != null)
                {
                    sorting.BringToFront(9999);
                }
            //}
            if (card.transform.tag == "ExtraCard")
                {
                    GameObject slotsContainer = GameObject.Find("UI-Panel/Extra Cards");
                }

            FBPlayerData.instance.VibrationEffect();
            ResetAfterCardBack(card);
            slotsCard.Remove(card);
            
            card.FlipImmediateBelowCards();
            if (card.GetComponent<Card>() != null)
            {
                goingBack = true;
                card.MoveBackToOriginalPosition(null, -1);
            }
            if (slotsCard.Count > 0)
                slotsCard[slotsCard.Count - 1].GetComponent<Button>().enabled = true;
        }
        else if (!SlotManager.instance.allSlotsOccupied)
        {
            if (card.tag == "ExtraCard")
            {
                card.sortingOrder_RightSideExtraCard = card.GetComponent<Canvas>().sortingOrder;
            }
          //  Debug.Log("OnCardClicked inside else if: ");
            FBPlayerData.instance.VibrationEffect();
            var sorting = card.GetComponent<CardSorting>();
            if (sorting != null)
            {
                sorting.BringToFront(9999);
            }

            if (FBPlayerData.instance.CURRENT_LEVEL == 2 && InitManager.instance.tutorialCntr == 0)
            {
                InitManager.instance.tutorialCntr++;
                tutorial.ShowNext();
            }
            if(card.tag == "WildCard")
            {
                 animDuration = 0.4f;
            }
            else
            {
                animDuration = 0.3f;
            }
            goingBack = false;
            int emptySlotIndex = -1;

            for (int i = 0; i < isSlotOccupied.Count; i++)
            {
                if (!isSlotOccupied[i])
                {
                    emptySlotIndex = i;
                    break;
                }
            }

            if (emptySlotIndex != -1)
            {
                    RectTransform cardRect = card.GetComponent<RectTransform>();
                if (cardRect != null && (card.tag == "ExtraCard" || card.tag == "WildCard"))
                {
                        card.originalPosition = cardRect.anchoredPosition;
                }

                isSlotOccupied[emptySlotIndex] = true;
                slotToCardMap[cardSlots[emptySlotIndex]] = card.GetComponent<RectTransform>();

                
                Vector2 targetPosition = cardSlots[emptySlotIndex].position;
                Vector3 targetRotation = new Vector3(0, 360, 360);

                
                float lateralOffset = cardSlots[emptySlotIndex].position.x;
                if (card.GetComponent<RectTransform>().tag == "ExtraCard")
                {
                    card.GetComponent<Image>().sprite = Resources.Load<Sprite>("FaceUpCard_0");
                }
                
                card.FlipImmediateBelowCards();
                
                //Sequence cardSequence = DOTween.Sequence();
                slotsCard.Add(card);
                foreach (var item in slotsCard)
                {
                    item.GetComponent<Button>().enabled = false;
                }
                var allwild = slotsCard.FindAll(card=> card.IsWildCard()== true);
                if (allwild.Count>0)
                {
                    WordServiceContainer.HintService.OnWildClick(allwild[0], "");
                }
                for (int i = 0; i < card.GetComponent<RectTransform>().childCount; i++)
                {
                    card.GetComponent<RectTransform>().GetChild(i).localRotation = Quaternion.Euler(0, 0, 0);
                }

                AnimateCardToSlot(card, cardRect, targetPosition, 
                    animDuration, startScale, endScale, endScale_ExtraCard);

            }
        }
        CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        AAA();
        yield return new WaitForSeconds(animDuration);
    }

    public void AnimateCardToSlot(Card card, RectTransform cardRect, Vector2 targetPosition, float animDuration, Vector3 startScale, Vector3 endScale, Vector3 endScale_ExtraCard)
    {
        Vector2 startPos = cardRect.anchoredPosition;
        float apexHeight;
        Transform trail = null;

        // Determine apex height
        switch (cardRect.tag)
        {
            case "ExtraCard":
                apexHeight = 800f;
                CardManager.instance.rightSideCards.Remove(card);
                BackButton.instance.HideThis();
                ExtraInSlotcards.Add(card);
                break;
            case "WildCard":
                apexHeight = 200f;
                break;
            default:
                apexHeight = 400f;
                break;
        }

        // Wild card trail
        if (card.isWildCard)
        {
            //activeTrail_WildCard = TrailEffectPool.Instance.GetTrailEffect(cardRect.position);
        }
        
        float apexY = Mathf.Max(startPos.y, targetPosition.y) + apexHeight;
        Vector3 targetScale = (cardRect.tag == "ExtraCard") ? endScale_ExtraCard : endScale;
        //Vector2 midPosition = cardRect.parent.TransformPoint(new Vector2(targetPosition.x, apexY));
        //midPosition.x = targetPosition.x;
        Vector3[] pathPoints = new Vector3[]
        {
        cardRect.parent.TransformPoint(startPos),                 // Start point
        cardRect.parent.TransformPoint(new Vector2(startPos.x, apexY)),  // Apex point
        targetPosition                                             // End point (slot position, world space)
        };

        cardRect.localRotation = Quaternion.identity;

        // Tween sequence
        Sequence motionSequence = DOTween.Sequence();

        // Arc motion
        motionSequence.Append(cardRect.DOMove(pathPoints[1], animDuration / 2).SetEase(Ease.OutQuad));
        motionSequence.Append(cardRect.DOMove(pathPoints[2], animDuration).SetEase(Ease.InQuad));

        // Rotation while moving
        motionSequence.Join(cardRect.DORotate(new Vector3(0, 0, 360), animDuration / 2, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        float delay = 0;
        if (FBPlayerData.instance.DeviceType == "Android")
            delay = 0.1f;
        else
            delay = 0;
        StartCoroutine(PlayCardPlacedSound(delay));
        

        // Smooth scale to target
        motionSequence.Join(cardRect.DOScale(targetScale, animDuration).SetEase(Ease.InOutQuad));

        // Bounce after landing
        motionSequence.Append(cardRect.DOScale(targetScale * 1.12f, 0.12f).SetEase(Ease.OutBack));
        motionSequence.Append(cardRect.DOScale(targetScale, 0.13f).SetEase(Ease.InOutQuad));

        // Trail update
        motionSequence.OnUpdate(() =>
        {
            //if (card.isWildCard && activeTrail_WildCard != null)
            //{
            //    activeTrail_WildCard.transform.position = cardRect.position;
            //}
        });

        // Cleanup
        motionSequence.OnComplete(() =>
        {
            var targetCard = cardRect;

            if (targetCard.tag == "WildCard")
            {
                FBPlayerData.instance.TOTAL_WILD_CARD--;
                FBPlayerData.instance.SavePlayerData();
                FindObjectOfType<NumberOfWildCard>().UpdateWildCard();
            }

            slotsCard[^1].GetComponent<Button>().enabled = true;

            //if (trail != null)
            //{
            //    StartCoroutine(ReturnTrailToPoolDelayed(trail.gameObject, 0.3f));
            //    trail = null;
            //}

            if (FBPlayerData.instance.CURRENT_LEVEL > 1)
            {
                if (card.TryGetComponent<CardSorting>(out var sorting))
                {
                    sorting.ResetOrder();
                }
                
                card.GetComponent<Card>().isMoving = false;
            }

            if (card.tag == "ExtraCard")
            {
                card.GetComponent<Canvas>().sortingOrder = card.sortingOrder_RightSideExtraCard;
            }
        });

        motionSequence.Play();
    }






    private IEnumerator ReturnTrailToPoolDelayed(GameObject trailObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        TrailEffectPool.Instance.ReturnTrailEffect(trailObj);
    }
    IEnumerator PlayCardPlacedSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.instance.PlaySFX("CardPlaced");
        Invoke("PlayCardTakeSound", 0.5f);
    }
    void PlayCardTakeSound()
    {
        SoundManager.instance.PlaySFX("card_take_"+SlotManager.instance.slotsCard.Count.ToString());
    }

    public void AAA()
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 1) return;
        bool allTrue = isSlotOccupied.All(b => b);
        allSlotsOccupied = allTrue;
        
        if (WordValidator.instance != null)
        {
            WordValidator.instance.ValidateWord(GetSlotString());
            GetSlotPoints();
            
        }
        else
        {
            Debug.LogError("Script instance is null!");
        }
        RemoveCardsButton.instance.SwapImage();

    }

    public void ResetAfterCardBack(Card card)
    {
        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card.GetComponent<RectTransform>());
        if(currentSlotIndex != -1)
        {
            allSlotsOccupied = false;
            isSlotOccupied[currentSlotIndex] = false;
            slotToCardMap.Remove(cardSlots[currentSlotIndex]);

            var allwild = slotsCard.FindAll(card => card.IsWildCard() == true);
            if (allwild.Count > 0)
            {
                WordServiceContainer.HintService.OnWildClick(allwild[0], "");
            }
        }
        
    }

    public string GetSlotString()
    {
        string slotString = "";
        
        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (slotToCardMap.ContainsKey(cardSlots[i]))
            {
                
                CardData cardData = slotToCardMap[cardSlots[i]].GetComponent<CardData>();
                if (cardData != null)
                {
                    slotString += cardData.letter;
                }
            }
        }
       // Debug.Log("____________String: " + slotString);
        return slotString;
    }
    
    public int GetSlotPoints()
    {
        int points = 0;
        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (slotToCardMap.ContainsKey(cardSlots[i]))
            {
                CardData cardData = slotToCardMap[cardSlots[i]].GetComponent<CardData>();
                if (cardData != null)
                {
                    points += cardData.cardValue;
                }
            }
        }
        Debug.Log("____________Points: " + points);
        return points*GameManager.instance.scoreMultiplier;
    }

    public IEnumerator SubmitWord()
    {
        AnalyticsManager.Instance.TrackWordCreated(SlotManager.instance.GetSlotString(), FBPlayerData.instance.CURRENT_LEVEL, GetSlotPoints());
        GameManager.instance.foundWords.Add(GetSlotString());
        WordnikDefinition.instance.FetchDefinition(SlotManager.instance.GetSlotString().ToLower());
        GameManager.instance.submittedWordLength = GetSlotString().Length;
        GameManager.instance.animateBonusTarget = true;
        if (GetSlotString().Length >= 5)
            Appreciations.instance.ShowAppreciation();
        if(InitManager.instance.isReplay)
        {
            if (bonusHud.currentBonusGoalType == BonusGoalType.NumberOfCards && GetSlotString().Length >= bonusHud.levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfLetters)
                GameManager.instance.wordCounter++;
        }
        else
        {
            if (bonusHud.currentBonusGoalType == BonusGoalType.NumberOfCards && GetSlotString().Length >= bonusHud.levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-1].numberOfLetters)
                GameManager.instance.wordCounter++;
        }
        
        GameManager.instance.hintText = "";
        GameManager.instance.foundValidWord = false;
        GameManager.instance.hintWord = "";
        GetSlotString();
        GameManager.instance.isValidWord = false;
        isSlotOccupied[0] = false;
        allSlotsOccupied = false;
        RemoveCardsButton.instance.SwapImage();
        GreenTabHandler.instance.HandleGreenTab(GetSlotString());
        SubmitButton.instance.SwapImage();
        DictionaryButton.instance.SwapImage();
        GameManager.instance.gainedPoint = GetSlotPoints();
        FigureOutBestWord(GetSlotString());
        Debug.Log("GameManager.instance.gainedPoint: " + GameManager.instance.gainedPoint);
        ScoreManager.instance.AddScore(GetSlotPoints());
        if (starProgressBar != null)
        {
            starProgressBar.UpdateStarBar(GetSlotPoints());
        }
        for (int i = slotsCard.Count - 1; i >= 0; i--)
        {
            Card card1 = slotsCard[i];
            if(card1.tag == "ExtraCard" || card1.tag == "WildCard")
            {
                extraCardCount++;
            }
            else
            {
                //cardCount++;
            }
        }
        GameManager.instance.scoreMultiplier = 1;
        CardManager.instance.totalCardToGet -= (GetSlotString().Length) - extraCardCount;
        //CardManager.instance.totalCardToGet -= cardCount;
        targetAchieve = GetSlotString().Length;
        extraCardCount = 0;
        Debug.Log("CardManager.instance.totalCardToGet... " + CardManager.instance.totalCardToGet);

        if (CardManager.instance.rightSideCards.Count <= 0 && CardManager.instance.extraCards.Count >= 1 && CardManager.instance.totalCardToGet >= 1)
        {
            if (extraCard != null)
                extraCard.OnExtraCardClick();
        }
        if (CardManager.instance.totalCardToGet <= 0)
        {
            Debug.Log("Game Completed...");
            if (starProgressBar != null)
            {
                int prevBrillanceScore;
                if (GameUtils.IsFacebookBuild())
                    prevBrillanceScore = FBPlayerData.instance.BRILLIANCE;
                else
                    prevBrillanceScore = PlayerPrefs.GetInt("BrillianceScore");
                if (!InitManager.instance.isReplay)
                    InitManager.instance.brillianceScore = prevBrillanceScore + starProgressBar.CalculateBrillianceScore();
                else
                    InitManager.instance.brillianceScore = prevBrillanceScore;
                if (GameUtils.IsFacebookBuild())
                    FBPlayerData.instance.BRILLIANCE = InitManager.instance.brillianceScore;
                else
                    PlayerPrefs.SetInt("BrillianceScore", InitManager.instance.brillianceScore);
                FBPlayerData.instance.SavePlayerData();
            }
            InitManager.instance.levelCompleted = true;
            if (FBPlayerData.instance.CURRENT_LEVEL == 1)
            {
                Invoke("LoadGame", 2f);
            }
            else
            {
                StartCoroutine(TweenExtraCards());
            }
        }

        Vector2 targetPosition;
        if(MultiplayerEventHandler.Instance.isMultiplayer)
        {
            targetPosition = FindObjectOfType<MultiplayerHud>().hudCard.position;
        }
        else
        {
            targetPosition = Hud.instance.hudCard.position;
        }
        
        for (int i = slotsCard.Count-1; i >= 0; i--)
        {
            isSlotOccupied[i] = false;
            slotToCardMap.Remove(cardSlots[i]);
            RectTransform card = slotsCard[i].GetComponent<RectTransform>();

            Vector3[] path = new Vector3[]
            {
                card.position,       // Start Point (P0)
                point1.position,  // Control Point 1 (P1)
                point2.position,  // Control Point 2 (P2)
                targetPosition    // End Point (P3)
            };

            Sequence cardSequence = DOTween.Sequence();
            card.SetAsLastSibling();
            CardManager.instance.UpdateFaceUpCards(slotsCard[i], isSlotOccupied[i]);
            cardSequence.Append(card.DOPath(path, tweenDuration, PathType.CatmullRom).SetEase(customEase));
            cardSequence.Join(card.DOScale(Hud.instance.hudCard.localScale, tweenDuration));
            cardSequence.Join(card.DORotate(new Vector3(0, 0, 348), tweenDuration, RotateMode.LocalAxisAdd).SetEase(customEase));
            //Debug.Log("scoreText.text: " + scoreText.text);
            Invoke("PlayWhooshSound", 0.3f);
            
            cardSequence.OnComplete(() =>
            {
                FBPlayerData.instance.VibrationEffect();
                card.gameObject.SetActive(false);
                Debug.Log("InitManager.instance.currentTarget: "+ InitManager.instance.currentTarget);
                if(InitManager.instance.currentTarget > 0)
                {
                    Debug.Log("card.tag: " + card.tag);
                    if(card.tag != "ExtraCard" && card.tag != "WildCard")
                    {
                        
                        InitManager.instance.currentTarget--;
                        Debug.Log("card.tag inside: " + InitManager.instance.currentTarget);
                        LevelManager.instance.UpdateCurrentTarget(InitManager.instance.currentTarget);
                        Debug.Log("InitManager.instance.currentTarget Hud.instance.DecreaseTarget");
                        Hud.instance.DecreaseTarget();
                    }
                        
                    
                }
                
            });

            yield return new WaitForSeconds(delayBetweenTweens);
        }
        
        slotsCard.Clear();
        

    }
    void PlayWhooshSound()
    {
        SoundManager.instance.PlaySFX("WhooshSound", 0.3f);
    }
    void LoadGame()
    {
        Debug.Log("Load Game");
        InitManager.instance.tutorialCntr = 0;
        FBPlayerData.instance.TUTORIAL_1_COMPLETED = true;
        FBPlayerData.instance.CURRENT_LEVEL++;
        FBPlayerData.instance.SavePlayerData();
        Initiate.Fade("Game", Color.black, 1f);
    }
    IEnumerator TweenExtraCards()
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            for (int i = CardManager.instance.extraCards.Count - 1; i >= 0; i--)
            {
                RectTransform card = CardManager.instance.extraCards[i].GetComponent<RectTransform>();
                card.gameObject.SetActive(false);
                FBPlayerData.instance.VibrationEffect();
                SoundManager.instance.PlaySFX("CardTurn", 0.3f);
                FBPlayerData.instance.VibrationEffect();
                yield return new WaitForSeconds(0.2f); // 0.3 sec delay between each
            }
            FBPlayerData.instance.TUTORIAL_2_COMPLETED = true;
            FBPlayerData.instance.CURRENT_LEVEL++;
            FBPlayerData.instance.SavePlayerData();
            Initiate.Fade("Game", Color.black, 1f);
            yield break;
        }
        GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
        extraCardContainer.transform.SetAsLastSibling();
        Vector2 targetPosition = Hud.instance.hudStar.position;
        int cardCount = CardManager.instance.extraCards.Count;
        int completedTweens = 0;
        if (cardCount <= 0)
        {
            //Invoke("ShowAd", 1.5f);
            StartCoroutine(HandleTournamentLogic());
        }
        else
        {
            for (int i = CardManager.instance.extraCards.Count - 1; i >= 0; i--)
            {
                Debug.Log("___Loop");
                RectTransform card = CardManager.instance.extraCards[i].GetComponent<RectTransform>();
                CanvasGroup canvasGroup = CardManager.instance.extraCards[i].GetComponent<CanvasGroup>();

                Vector3[] path = new Vector3[]
                {
                card.position,       // Start Point (P0)
                point3.position,  // Control Point 1 (P1)
                point4.position,  // Control Point 2 (P2)
                targetPosition    // End Point (P3)
                };

                Sequence cardSequence = DOTween.Sequence();
                card.SetAsLastSibling();

                cardSequence.Append(card.DOPath(path, 0.8f, PathType.CatmullRom)
        .SetEase(customEase));

                cardSequence.Join(card.DOScale(Hud.instance.hudCard.localScale, 0.8f)
                    .SetEase(Ease.OutBack));
                cardSequence.Join(card.DORotate(new Vector3(0, 0, 348), 0.8f, RotateMode.LocalAxisAdd)
                    .SetEase(customEase));

                //cardSequence.Append(canvasGroup.DOFade(0, 0.8f).SetEase(customFadeEase));
                cardSequence.OnUpdate(() =>
                {
                //Debug.Log($"Current Alpha: {canvasGroup.alpha}");
            });
                //cardSequence.Play();

                cardSequence.OnComplete(() =>
                {
                    FBPlayerData.instance.VibrationEffect();
                //Debug.Log("_______canvasGroup.alpha: "+canvasGroup.alpha);
                card.gameObject.SetActive(false);
                    FBPlayerData.instance.VibrationEffect();
                    completedTweens++;
                    Debug.Log("Card Count: " + cardCount);
                    Debug.Log("Card Count Completed Tween: " + completedTweens);

                    if (completedTweens == cardCount)
                    {
                        //Invoke("ShowAd", 1f);

                        StartCoroutine(HandleTournamentLogic());
                        

                    }
                });

                yield return new WaitForSeconds(delayBetweenTweens);

            }
        }
        
    }
    void ShowAd_AfterTutorial()
    {
        Debug.Log("___Show Interstitial");
        // All tweens complete, now show interstitial and continue
        //AdTimerHandler.Instance.TryShowAd("Tutorial");
        FBPlayerData.instance.ContinueGameAfterInterstitial("Levelup");

#if UNITY_EDITOR
        FBPlayerData.instance.ContinueGameAfterInterstitial("Levelup");
#endif
    }
    private IEnumerator HandleTournamentLogic()
    {
        

        FBPlayerData.instance.SavePlayerData();

        int level = FBPlayerData.instance.CURRENT_LEVEL;
        Debug.Log($"[HandleTournamentLogic] Level: {level}");

#if UNITY_EDITOR
        ContinueGameAfterTournament();
        yield break;
#endif

        if (level % 5 == 0)
        {
            // 🎯 Every 5th level → Tournament
            Debug.Log("Showing Tournament Popup...");
            yield return StartCoroutine(ShowTournament(0));
        }
        else
        {
            // 🎯 Otherwise → Normal level-up popup
            Debug.Log("Showing Level-Up Popup...");
            StartCoroutine(ShowLevelup(1.5f));
        }
    }
    private IEnumerator ShowTournament(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        Debug.Log("Showing Tournament...");

        Debug.Log("FBPlayerData.instance.BRILLIANCE: " + GetSlotPoints());

        // ✅ Tournament should ONLY handle tournament logic
        Debug.Log("processTournament FBPlayerData.instance.BRILLIANCE: " + FBPlayerData.instance.BRILLIANCE);
        Application.ExternalCall("processTournament", FBPlayerData.instance.BRILLIANCE.ToString());
    }

    public void ContinueGameAfterInterstitial()
    {
        Debug.Log("_____ContinueGameAfterInterstitial");
        InitManager.instance.CurrentScene = "Levelup";
        if(!InitManager.instance.isReplay)
            FBPlayerData.instance.CURRENT_LEVEL++;
        FBPlayerData.instance.SavePlayerData();
        
        PopupManager.instance.TogglePopup(PopupManager.instance.levelupPopup);
        Debug.Log("_____ContinueGameAfterInterstitial: Levelup");
        //StartCoroutine(LoadMenu());
    }
    public void ContinueGameAfterTournament()
    {
        Debug.Log("_____ContinueGameAfterTournament");
        StartCoroutine(ShowLevelup(0f));
    }
    private IEnumerator ShowLevelup(float delay)
    {
        if (!InitManager.instance.isReplay)
        {
            if(FBPlayerData.instance.CURRENT_LEVEL == 3)
                AdTimerHandler.Instance.shouldShowAd = true;
            FBPlayerData.instance.CURRENT_LEVEL++;
        }

        Debug.Log("ShowLevelup");
        yield return new WaitForSeconds(delay);
        InitManager.instance.CurrentScene = "Levelup";
        PopupManager.instance.TogglePopup(PopupManager.instance.levelupPopup);
    }
    IEnumerator LoadMenu()
    {

        yield return new WaitForSeconds(0f);
        
        //if (FBPlayerData.instance.CURRENT_LEVEL > 5)
        //{
        //    FBPlayerData.instance.CURRENT_LEVEL = 1;
        //}
        
        Debug.Log("FBPlayerData.instance.CURRENT_LEVEL: " + FBPlayerData.instance.CURRENT_LEVEL);
        Initiate.Fade("Menu", Color.black, 1f);

    }
    void FigureOutBestWord(string word)
    {
        int score = GameManager.instance.gainedPoint; // your existing scoring logic

        if (score > bestScore)
        {
            bestScore = score;
            bestWord = word;
        }

        // ...continue normal flow
    }
}
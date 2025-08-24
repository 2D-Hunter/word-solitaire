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

    public BonusHud bonusHud;

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
        StartCoroutine(OnCardClicked_Coroutine(card));
    }

    public void ReturnBackToDeck(Card card)
    {
        
        StartCoroutine(ReturnBackToDeckRoutine(card));
    }

    private IEnumerator ReturnBackToDeckRoutine(Card card) 
    { 
        yield return new WaitForEndOfFrame();
        //card.MoveBackToOriginalPosition(null,false);
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
        // ? Slight delay before flipping below cards
        StartCoroutine(DelayedFlip(card, 0.3f)); // Delay based on how fast top card moves
        //yield return DelayedFlip(card, 0.3f);
        //card.FlipImmediateBelowCards();
        ResetAfterCardBack(card);
        slotsCard.Remove(card);
        card.GetComponent<Button>().enabled = true;
        CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        card.MoveBackToOriginalPosition(null, false, () => {
            
            finished = true;
        });
        yield return new WaitUntil(() => finished);
        
        //yield return new WaitForSeconds(0.4f);
        //card.FlipImmediateBelowCards();
        //ResetAfterCardBack(card);
        //slotsCard.Remove(card);
        //if (slotsCard.Count > 0)
        //    slotsCard[slotsCard.Count - 1].GetComponent<Button>().enabled = true;

        //CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        //AAA();
        //yield return new WaitForSeconds(0.2f);
        
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
        if(FBPlayerData.instance.CURRENT_LEVEL > 2)
        {
            sortingLayer = card.GetComponent<Canvas>().sortingOrder;
            card.GetComponent<Canvas>().sortingOrder = 15;
        }
        
        Debug.Log("___Card Clicked...");
        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card.GetComponent<RectTransform>());
        Debug.Log("___currentSlotIndex: "+ currentSlotIndex);
        //Debug.Log("___currentSlotIndex: "+ currentSlotIndex);
        if (currentSlotIndex != -1)
        {
            if (FBPlayerData.instance.CURRENT_LEVEL == 1) yield break;
            //card going back to place
            Debug.Log("___currentSlotIndex: " + currentSlotIndex + "____"+ (GetSlotString().Length-1));
            //if (currentSlotIndex == GetSlotString().Length-1)
            //{
                if (card.transform.tag == "ExtraCard")
                {
                    GameObject slotsContainer = GameObject.Find("UI-Panel/Extra Cards");
                   // slotsContainer.transform.SetAsLastSibling();
                }

            FBPlayerData.instance.VibrationEffect();
            ResetAfterCardBack(card);
            slotsCard.Remove(card);
            
           // card.GetComponent<RectTransform>().SetAsLastSibling();
            card.FlipImmediateBelowCards();
            if (card.GetComponent<Card>() != null)
            {
                goingBack = true;
                //Sequence cardSequence = DOTween.Sequence();
                card.MoveBackToOriginalPosition(card);
            }
            if (slotsCard.Count > 0)
                slotsCard[slotsCard.Count - 1].GetComponent<Button>().enabled = true;
        }
        else
        {
            if (SlotManager.instance.allSlotsOccupied) yield  break;
            FBPlayerData.instance.VibrationEffect();
            Invoke("PlayCardPlacedSound", 0);
            
            if (FBPlayerData.instance.CURRENT_LEVEL == 2 && InitManager.instance.tutorialCntr == 0)
            {
                InitManager.instance.tutorialCntr++;
                tutorial.ShowNext();
            }
            //card is going to bottom slot
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
                    //originalPositions1[cardRect] = cardRect.anchoredPosition;
                    //Debug.Log("+++++++::: " + originalPositions1[cardRect]);
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
                /*Vector2 startPos = cardRect.anchoredPosition;

                cardRect.DOMove(targetPosition, animDuration).SetEase(Ease.Linear);

                Sequence verticalMotion = DOTween.Sequence();
                if (card.GetComponent<RectTransform>().tag == "ExtraCard")
                {
                    height = 800f;
                    CardManager.instance.rightSideCards.Remove(card);
                }
                else if(card.GetComponent<RectTransform>().tag == "WildCard")
                {
                    height = 200f;
                }
                else
                {
                    height = 400f;
                }
                if (card.isWildCard)
                {
                    activeTrail_WildCard = TrailEffectPool.Instance.GetTrailEffect(cardRect.position); //Instantiate(trailEffectPrefab, cardRect.position, Quaternion.identity);
                }

                float apexY = Mathf.Max(startPos.y, targetPosition.y) + height;
                verticalMotion.Append(cardRect.DOAnchorPosY(apexY, animDuration/2).SetEase(Ease.OutQuad));

                verticalMotion.Append(cardRect.DOMove(targetPosition, animDuration ).SetEase(Ease.OutQuad));

                cardRect.DOScale(startScale, animDuration / 4).SetEase(Ease.OutQuad);
                if (card.GetComponent<RectTransform>().tag == "ExtraCard")
                    cardRect.DOScale(endScale_ExtraCard, animDuration).SetEase(Ease.InOutQuad);
                else
                    cardRect.DOScale(endScale, animDuration).SetEase(Ease.InOutQuad);

                verticalMotion.Play();
                verticalMotion.OnUpdate(() =>
                {
                    if(card.isWildCard)
                    {
                        if (activeTrail_WildCard != null)
                        {
                            activeTrail_WildCard.transform.position = cardRect.position;
                        }
                    }
                    
                });

                cardRect.DORotate(new Vector3(0, 0, 360), animDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                verticalMotion.OnComplete(() =>
                {
                    Debug.Log("Card reached the slot with projectile motion!");
                    if(card.tag == "WildCard")
                    {
                        FBPlayerData.instance.TOTAL_WILD_CARD--;
                        FBPlayerData.instance.SavePlayerData();
                        FindObjectOfType<NumberOfWildCard>().UpdateWildCard();
                    }
                    slotsCard[slotsCard.Count - 1].GetComponent<Button>().enabled = true;
                    if (activeTrail_WildCard != null)
                    {
                        if (card.isWildCard)
                        {
                            *//*  Destroy(activeTrail_WildCard, 0.3f); // Add a delay if needed for particles to finish
                              activeTrail_WildCard = null;*//*
                            StartCoroutine(ReturnTrailToPoolDelayed(activeTrail_WildCard, 0.3f));
                        }
                    }
                });*/


            }
        }
        CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        AAA();
        yield return new WaitForEndOfFrame();
    }


    public void AnimateCardToSlot(Card card, RectTransform cardRect, Vector2 targetPosition, float animDuration, Vector3 startScale, Vector3 endScale, Vector3 endScale_ExtraCard)
    {
        Vector2 startPos = cardRect.anchoredPosition;
        float apexHeight;
        Transform trail = null;

        // Decide arc height and manage card category
        switch (cardRect.tag)
        {
            case "ExtraCard":
                apexHeight = 800f;
                CardManager.instance.rightSideCards.RemoveCard(card);
                break;
            case "WildCard":
                apexHeight = 200f;
                break;
            default:
                apexHeight = 400f;
                break;
        }

        // Handle wild card trail effect using pool
        if (card.isWildCard)
        {
            activeTrail_WildCard = TrailEffectPool.Instance.GetTrailEffect(cardRect.position); //Instantiate(trailEffectPrefab, cardRect.position, Quaternion.identity);
        }

        float apexY = Mathf.Max(startPos.y, targetPosition.y) + apexHeight;
        Vector3 targetScale = (cardRect.tag == "ExtraCard") ? endScale_ExtraCard : endScale;
        Vector2[] pathPoints = new Vector2[]
        {
            cardRect.parent.TransformPoint(startPos),     // start
            cardRect.parent.TransformPoint(new Vector2(startPos.x, apexY)), // mid arc point
            targetPosition       // end (must be world space!)
        };
        // Create full tween sequence
        Sequence motionSequence = DOTween.Sequence();

        // Vertical arc animation
        Debug.Log("pathPoints[1] >>>"+pathPoints[1]);
        motionSequence.Append(cardRect.DOMove(pathPoints[1], animDuration).SetEase(Ease.OutQuad));
        motionSequence.Append(cardRect.DOMove(pathPoints[2], animDuration).SetEase(Ease.OutQuad));
        // Join scaling and rotation
        motionSequence.Join(cardRect.DOScale(targetScale, animDuration).SetEase(Ease.InOutQuad));
        Debug.Log("Error");
        motionSequence.Join(cardRect.DORotate(new Vector3(0, 0, 360), animDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear));

        // Optional bounce-in scale
        cardRect.DOScale(startScale, animDuration / 4).SetEase(Ease.OutQuad);

        // On every frame of animation
        motionSequence.OnUpdate(() =>
        {
            if (card.isWildCard)
            {
                if (activeTrail_WildCard != null)
                {
                    activeTrail_WildCard.transform.position = cardRect.position;
                }
            }
        });

        // Final callback
        motionSequence.OnComplete(() =>
        {
            Debug.Log("Card reached the slot with projectile motion!");

            if (cardRect.tag == "WildCard")
            {
                FBPlayerData.instance.TOTAL_WILD_CARD--;
                FBPlayerData.instance.SavePlayerData();
                FindObjectOfType<NumberOfWildCard>().UpdateWildCard();
            }

            slotsCard[^1].GetComponent<Button>().enabled = true;

            if (trail != null)
            {
                StartCoroutine(ReturnTrailToPoolDelayed(trail.gameObject, 0.3f));
                trail = null;
            }
            if (FBPlayerData.instance.CURRENT_LEVEL > 2)
            {
                card.GetComponent<Canvas>().sortingOrder = sortingLayer;
            }
        });

        motionSequence.Play();
    }

    

    private IEnumerator ReturnTrailToPoolDelayed(GameObject trailObj, float delay)
    {
        yield return new WaitForSeconds(delay);
        TrailEffectPool.Instance.ReturnTrailEffect(trailObj);
    }
    void PlayCardPlacedSound()
    {
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
        Debug.Log("____________String: " + slotString);
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
            Invoke("ShowAd", 1f);


            yield break;
        }
        GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
        extraCardContainer.transform.SetAsLastSibling();
        Vector2 targetPosition = Hud.instance.hudStar.position;
        int cardCount = CardManager.instance.extraCards.Count;
        int completedTweens = 0;
        if (cardCount <= 0)
        {
            Invoke("ShowAd", 1.5f);
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
                        Invoke("ShowAd", 1f);
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
    void ShowAd()
    {
        Debug.Log("___Show Interstitial");
        // All tweens complete, now show interstitial and continue
        if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            FBPlayerData.instance.TUTORIAL_2_COMPLETED = true;
            FBPlayerData.instance.CURRENT_LEVEL++;
            FBPlayerData.instance.SavePlayerData();
            Initiate.Fade("Game", Color.black, 1f);
        }
        else
        {
            if (!InitManager.instance.isReplay)
                FBPlayerData.instance.CURRENT_LEVEL++;
            FBPlayerData.instance.SavePlayerData();
#if UNITY_EDITOR
            ContinueGameAfterTournament();
#endif
            Debug.Log("FBPlayerData.instance.BRILLIANCE: " + FBPlayerData.instance.BRILLIANCE.ToString());

            Application.ExternalCall("processTourement", FBPlayerData.instance.BRILLIANCE.ToString());
            //AdTimerHandler.Instance.TryShowAd("Levelup");


        }


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
        InitManager.instance.CurrentScene = "Levelup";
        

        PopupManager.instance.TogglePopup(PopupManager.instance.levelupPopup);
        Debug.Log("_____ContinueGameAfterInterstitial: Levelup");
        //StartCoroutine(LoadMenu());
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
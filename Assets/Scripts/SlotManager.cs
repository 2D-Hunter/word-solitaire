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
     Vector3 startScale = new Vector3(2f, 2f, 2f);
     Vector3 endScale = new Vector3(0.83f, 0.83f, 0.83f);

    int lastFilledSlot = -1;

    public GameObject trailEffectPrefab;
    private GameObject activeTrail_WildCard;
    private int extraCardCount = 0;
    private int cardCount = 0;
    private StarProgressBar starProgressBar;
    public Card extraCard;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
        isSlotOccupied = new List<bool>(new bool[cardSlots.Count]);
        slotToCardMap = new Dictionary<RectTransform, Transform>();
        //originalPositions = new Dictionary<RectTransform, Vector2>();

        StartCoroutine(StoreOriginalPositions());
        starProgressBar = FindObjectOfType<StarProgressBar>();


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
        
        Debug.Log("___Card Clicked...");
        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card.GetComponent<RectTransform>());

        //Debug.Log("___currentSlotIndex: "+ currentSlotIndex);
        if (currentSlotIndex != -1)
        {
            //card going back to place
            Debug.Log("___currentSlotIndex: " + currentSlotIndex + "____"+ (GetSlotString().Length-1));
            //if (currentSlotIndex == GetSlotString().Length-1)
            //{
                if (card.transform.tag == "ExtraCard")
                {
                    GameObject slotsContainer = GameObject.Find("UI-Panel/Extra Cards");
                    slotsContainer.transform.SetAsLastSibling();
                }

            ResetAfterCardBack(card);
            slotsCard.Remove(card);

            card.GetComponent<RectTransform>().SetAsLastSibling();
            card.FlipImmediateBelowCards();
            if (card.GetComponent<Card>() != null)
            {
                goingBack = true;
                Sequence cardSequence = DOTween.Sequence();
                card.MoveBackToOriginalPosition();
            }
        }
        else
        {
            //card is going to bottom slot
            if (SlotManager.instance.allSlotsOccupied) return;
            PlaySound.instance.PlaySoundEffect();
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
                if (cardRect != null && card.tag == "ExtraCard")
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
                var allwild = slotsCard.FindAll(card=> card.IsWildCard()== true);
                if (allwild.Count>0)
                {
                    WordServiceContainer.HintService.OnWildClick(allwild[0], "");
                }
                for (int i = 0; i < card.GetComponent<RectTransform>().childCount; i++)
                {
                    card.GetComponent<RectTransform>().GetChild(i).localRotation = Quaternion.Euler(0, 0, 0);
                }

                Vector2 startPos = cardRect.anchoredPosition;

                cardRect.DOMove(targetPosition, animDuration).SetEase(Ease.Linear);

                Sequence verticalMotion = DOTween.Sequence();
                if (card.GetComponent<RectTransform>().tag == "ExtraCard")
                {
                    height = 800f;
                    CardManager.instance.rightSideCards.Remove(card);
                }
                else
                {
                    height = 400f;
                }
                if (card.isWildCard)
                {
                    activeTrail_WildCard = Instantiate(trailEffectPrefab, cardRect.position, Quaternion.identity);
                }

                float apexY = Mathf.Max(startPos.y, targetPosition.y) + height;
                verticalMotion.Append(cardRect.DOAnchorPosY(apexY, animDuration/2).SetEase(Ease.OutQuad));

                verticalMotion.Append(cardRect.DOMove(targetPosition, animDuration ).SetEase(Ease.OutQuad));

                cardRect.DOScale(startScale, animDuration / 4).SetEase(Ease.OutQuad);
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
                    if (activeTrail_WildCard != null)
                    {
                        if (card.isWildCard)
                        {
                            Destroy(activeTrail_WildCard, 0.3f); // Add a delay if needed for particles to finish
                            activeTrail_WildCard = null;
                        }
                    }
                });


            }
        }
        CardManager.instance.UpdateFaceUpCards(card, card.isFaceUp);
        AAA();
    }
    public void AAA()
    {
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
        
        if(GetSlotString().Length >= 4)
            Appreciations.instance.ShowAppreciation();
        GameManager.instance.hintText = "";
        GameManager.instance.foundValidWord = false;
        GameManager.instance.hintWord = "";
        GetSlotString();
        GameManager.instance.isValidWord = false;
        isSlotOccupied[0] = false;
        RemoveCardsButton.instance.SwapImage();
        GreenTabHandler.instance.HandleGreenTab(GetSlotString());
        SubmitButton.instance.SwapImage();
        DictionaryButton.instance.SwapImage();
        
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
                int prevBrillanceScore = PlayerPrefs.GetInt("BrillianceScore");
                InitManager.instance.brillianceScore = prevBrillanceScore + starProgressBar.CalculateBrillianceScore();
                PlayerPrefs.SetInt("BrillianceScore", InitManager.instance.brillianceScore);
            }
            InitManager.instance.levelCompleted = true;
            StartCoroutine(TweenExtraCards());
        }

        Vector2 targetPosition = Hud.instance.hudCard.position;
        
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
            cardSequence.OnComplete(() =>
            {
                card.gameObject.SetActive(false);
                Debug.Log("InitManager.instance.currentTarget: "+ InitManager.instance.currentTarget);
                if(InitManager.instance.currentTarget > 0)
                {
                    Debug.Log("card.tag: " + card.tag);
                    if(card.tag != "ExtraCard")
                    {
                        Debug.Log("card.tag inside: ");
                        InitManager.instance.currentTarget--;
                        LevelManager.instance.UpdateLevelUI(LevelManager.instance.levelData.levels[InitManager.instance.currentLevel - 1].levelNumber, InitManager.instance.currentTarget);
                        Debug.Log("InitManager.instance.currentTarget Hud.instance.DecreaseTarget");
                        Hud.instance.DecreaseTarget();
                    }
                        
                    
                }
                
            });

            yield return new WaitForSeconds(delayBetweenTweens);
        }
        
        slotsCard.Clear();
        

    }
    IEnumerator TweenExtraCards()
    {
        GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
        extraCardContainer.transform.SetAsLastSibling();
        Vector2 targetPosition = Hud.instance.hudStar.position;

        for (int i = CardManager.instance.extraCards.Count - 1; i >= 0; i--)
        {
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
                //Debug.Log("_______canvasGroup.alpha: "+canvasGroup.alpha);
                card.gameObject.SetActive(false);

                

            });

            yield return new WaitForSeconds(delayBetweenTweens);

        }

        StartCoroutine(LoadMenu());
    }

    IEnumerator LoadMenu()
    {

        yield return new WaitForSeconds(1);
        InitManager.instance.currentLevel++;
        if(InitManager.instance.currentLevel > 5)
        {
            InitManager.instance.currentLevel = 1;
        }
        
        Debug.Log("InitManager.instance.currentLevel: " + InitManager.instance.currentLevel);
        Initiate.Fade("Menu", Color.black, 1f);
    }
}
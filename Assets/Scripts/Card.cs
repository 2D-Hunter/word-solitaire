using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using DG.Tweening;
//[ExecuteAlways]
public class Card : MonoBehaviour
{

    public List<Card> belowCards;
    
    public RectTransform rectTransform;
    //public static Card instance;
    private SlotManager slotManager;
    public GameObject cardFace;
    public bool isFaceUp = false;
    public bool isWildCard = false;
    public bool switchToFaceDown = false;


    private bool isFlipping = false;

    //Extra Cards

    
    private float lastCardPos;
    public float originalXPos;
    private float moveDistance = 275;
    private bool isFlipped = false;
    private int originalSiblingIndex;
    float flipDuration = 0.1f;

    public CardData cardData;

    public Vector2 originalPosition;
    Sequence cardSequence;
    public Tutorial tutorial;

    private float[] originalPosOfExtraCards = { -180f, -165f, -150f, -135f, -120f, -105, -90, -75, -60, -45, -30, -15, 0, 15, 30 };

    private void Awake()
    {
        Debug.Log("______this.tag: "+ this.tag);
        if(this.tag == "ExtraCard")
        {
            rectTransform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("ExtraCard");
            int randomIndex = Random.Range(0, CardManager.instance.extraCards.Count);
            
        }
        if (FBPlayerData.instance)
        {
            if (FBPlayerData.instance.CURRENT_LEVEL == 1)
            {
                gameObject.GetComponent<Button>().enabled = false;
            }
        }
    }
    //private void OnValidate()
    //{
    //    //if (Card.instance.gameObject.tag != "ExtraCard")
    //    //{
    //    Debug.Log("________OnValidate");
    //    cardFace.SetActive(!isFaceUp);
        

    //    //}
    //}
    private void Start()
    {
        //instance = this;
         cardSequence = DOTween.Sequence();
        slotManager = FindObjectOfType<SlotManager>();
        if (this.tag != "ExtraCard")
        {
            CardManager.instance.UpdateFaceUpCards(this, isFaceUp);
        }
        Invoke("AddFaceupExtraCard", 1f);

        if(FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            if (tutorial == null)
                tutorial = FindObjectOfType<Tutorial>();
        }

    }
    
    public void MoveBackToOriginalPosition()
    {
        
        if (this.tag == "ExtraCard")
        {
            CardManager.instance.rightSideCards.Add(this);
            if (cardSequence != null && cardSequence.IsActive())
            {
                cardSequence.Kill(); // Clean up old sequence explicitly
            }

            cardSequence = DOTween.Sequence();
            cardSequence.Append(GetComponent<RectTransform>().DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad));
            cardSequence.Join(GetComponent<RectTransform>().DOScale(1f, 0.3f).SetEase(Ease.OutBack));
            cardSequence.Join(GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));
            cardSequence.OnComplete(() =>
            {
                Debug.Log("Card has reached back into the original position.");
                GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 180, 0);
                GetComponent<RectTransform>().GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);
                GetComponent<RectTransform>().GetChild(1).localRotation = Quaternion.Euler(0, -180, 0);
                GetComponent<RectTransform>().GetChild(3).localRotation = Quaternion.Euler(0, -180, 0);

            });
        }
        else
        {
            //GetComponent<RectTransform>().DOKill();
            Debug.Log("cardSequence121212");

            if (cardSequence != null && cardSequence.IsActive())
            {
                cardSequence.Kill(); // Clean up old sequence explicitly
            }

            cardSequence = DOTween.Sequence();
            cardSequence.Append(GetComponent<RectTransform>().DOAnchorPos(originalPosition, 0.2f).SetEase(Ease.OutQuad));
                cardSequence.Join(GetComponent<RectTransform>().DOScale(1f, 0.2f).SetEase(Ease.OutBack));
                cardSequence.Join(GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                 {
                     Debug.Log("sendBackAll000");
                     RemoveCardsButton.instance.sendBackAll = false;

                 });

                Invoke("SetFaceOfCard", 0.1f);
            
            
        }
    }
    void SetFaceOfCard()
    {
        Debug.Log("sendBackAll111: "+ RemoveCardsButton.instance.sendBackAll);
        if (belowCards.Count == 0 && RemoveCardsButton.instance.sendBackAll)
        {
            GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(true);
            isFaceUp = false;
        }
    }
    public void SetAsWild()
    {
        isWildCard = true;
        //rectTransform.GetChild(3).gameObject.SetActive(true);
        Debug.Log(gameObject.name + " is now a Wild card!");
    }
    public bool IsWildCard()
    {
        return isWildCard;
    }
    void AddFaceupExtraCard()
    {
        CardManager.instance.UpdateFaceUpCards(CardManager.instance.rightSideCards[0], true);
    }

    public void OnCardClick(Card card)
    {
        Debug.Log("____111");
        RemoveCardsButton.instance.sendBackAll = false;
        SlotManager.instance.goingBack = false;
        if (InitManager.instance != null)
            if (InitManager.instance.levelCompleted) return;

        

        if (this.tag == "ExtraCard")
        {
            Debug.Log("____111");
            GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
            extraCardContainer.transform.SetAsLastSibling();
            if (isFaceUp && !SlotManager.instance.allSlotsOccupied)
            {
                slotManager.OnCardClicked(this);
            }
            else
            {
                Debug.Log("____111");
                OnExtraCardClick();
            }
            
        }
        else
        {
            if (!isFaceUp) return;
            Debug.Log("InitManager.instance.tutorialCntr:  " + cardData.letter+"_____"+ FBPlayerData.instance.CURRENT_LEVEL+"_____"+ InitManager.instance.tutorialCntr);
            if(!FBPlayerData.instance.TUTORIAL_1_COMPLETED || !FBPlayerData.instance.TUTORIAL_2_COMPLETED)
            {
                if (cardData.letter == 'O' && FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 0)
                {
                    return;
                }
                if (cardData.letter == 'G' && FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 1)
                {
                    return;
                }
                if (cardData.letter == 'F' && FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 3)
                {
                    return;
                }
                if (cardData.letter == 'A' && FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 4)
                {
                    return;
                }
                if ( FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 5)
                {
                    return;
                }
                if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
                {
                    InitManager.instance.tutorialCntr++;
                    tutorial.ShowNext();
                }
            }
            
            
            isFlipping = false;
            Debug.Log("isFlipping000: " + isFlipping);
            slotManager.OnCardClicked(this);
            Debug.Log($"Card {name} was clicked!");
            Debug.Log("SlotManager.instance.goingBack: "+ SlotManager.instance.goingBack);
            if (SlotManager.instance.goingBack)
            {
                GameObject cardContainer = GameObject.Find("UI-Panel/Levels");
                cardContainer.transform.SetAsLastSibling();
                FlipImmediateBelowCards();
            }
                
        }
    }

    public void OnExtraCardClick()
    {
        Debug.Log("OnExtraCardClick: "+ FBPlayerData.instance.CURRENT_LEVEL + "____" + InitManager.instance.tutorialCntr);
        

        Card eCard = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1];
        RectTransform rectTransform = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1].GetComponent<RectTransform>();

        Debug.Log(eCard);
        Debug.Log(rectTransform);
        if (!isFaceUp)
        {
            lastCardPos = rectTransform.anchoredPosition.x;
            Debug.Log("___Last Card Pos: " + lastCardPos);
            eCard.isFlipped = true;
            eCard.isFaceUp = true;

            originalSiblingIndex = rectTransform.GetSiblingIndex();
            
            rectTransform.SetAsLastSibling();
            if (CardManager.instance.rightSideCards.Count == 0)
                moveDistance = 200;
            else if (CardManager.instance.rightSideCards.Count == 1)
                moveDistance = 220;
            if (CardManager.instance.rightSideCards.Count >= 2)
                moveDistance = 240;

            
            

            

            rectTransform.DOAnchorPosX(moveDistance, 0.3f);
            

            if (CardManager.instance.rightSideCards.Count >= 1)
            {
                GameManager.instance.ShowBackButton();
            }

            FlipExtraCard(rectTransform, eCard);
            //rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            //.OnComplete(() =>
            //{

            //    //isFaceUp = true;
            //    UpdateCardFlipping(true, eCard);
            //    rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            //        .OnComplete(() =>
            //        {
            //            isFlipping = false;
            //            //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
            //        });
            //});
            Debug.Log("______qqq");
            
        }
        else
        {
            //SlotManager.instance.OnCardClicked(rectTransform);
        }
    }
    private void FlipExtraCard(RectTransform rectTransform, Card eCard)
    {
        rectTransform.DOKill();

        Sequence sequence = DOTween.Sequence();

        rectTransform.DOAnchorPosX(moveDistance, 0.3f);
        if(CardManager.instance.extraCards.Count == 1)
        {
            GameManager.instance.ShowMoreCardsToBuy();
        }
        rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
        .OnComplete(() =>
        {
            Debug.Log("Half flip: "+ eCard);
                //isFaceUp = true;
                UpdateCardFlipping(true, eCard);
            Invoke("CardTurnSound", 0.05f);
            
            rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                .OnComplete(() =>
                {
                    Debug.Log("Half flip-2");
                    isFlipping = false;
                        //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
                    });
        });

    }
    void CardTurnSound()
    {
        Debug.Log("CardTurnSound");
        SoundManager.instance.PlaySFX("CardTurn", 0.3f);
    }


    public void FlipImmediateBelowCards()
    {
        foreach (Card belowCard in belowCards)
        {
            if (belowCard == null) continue;

            Debug.Log($"Processing below card: {belowCard.name}");

            // Check overlap conditions
            bool overlapsWithTappedCard = IsOverlapping(this, belowCard, 2f);
            bool overlapsWithOtherCards = DoesOverlapWithAnyOtherCardExcept(belowCard, this);



            Debug.Log("____>>>overlapsWithTappedCard: " + overlapsWithTappedCard + "____" + this + "____" + belowCard);
            Debug.Log("____>>>overlapsWithOtherCards: " + overlapsWithOtherCards + "____" + belowCard + "____" + this);


            //if(this.name == "Card 5" && belowCard.name == "Card 1")
            //{
            //    belowCard.FlipCard();&& 
            //}
            if (overlapsWithTappedCard && !overlapsWithOtherCards && isFaceUp)
            //if (overlapsWithTappedCard && !overlapsWithOtherCards)
            {
                Debug.Log("____FlipImmediateBelowCards-11");
                Debug.Log($"Flipping immediate below card: {belowCard.name}");
                belowCard.FlipCard(belowCard);

                //belowCard.FlipImmediateBelowCards();
            }
            else
            {
                Debug.Log("____FlipImmediateBelowCards-22");
                Debug.Log($"Card {belowCard.name} is blocked or not directly below.");
                if (SlotManager.instance.goingBack && belowCard.isFaceUp)
                    belowCard.FlipCard(belowCard);
            }
        }
    }

    


    private bool DoesOverlapWithAnyOtherCardExcept(Card targetCard, Card excludedCard)
    {
        foreach (Card otherCard in CardManager.instance.totalCardsToClear)
        {
            if (otherCard == targetCard || otherCard == excludedCard)
            {
                continue;
            }

            if (!otherCard.isFaceUp) continue;

            //Debug.Log("___Overlap detected: " + targetCard.name + "_____" + otherCard.name);
            if (IsOverlapping(targetCard, otherCard, 2f))
            {
                Debug.Log($"Overlap detected: {targetCard.name} overlaps with top-layer card {otherCard.name}");
                return true;
            }
            else
            {
                Debug.Log($"No Overlap detected: {targetCard.name} overlaps with top-layer card {otherCard.name}");
            }
        }

        return false;
    }
    private bool DoesOverlapWithAnyOtherCardExcept14(Card targetCard, Card excludedCard)
    {
        Rect targetRect = GetWorldRect(targetCard.GetComponent<RectTransform>());
        float targetCenterY = targetRect.center.y;

        foreach (Card otherCard in CardManager.instance.totalCardsToClear)
        {
            if (otherCard == targetCard || otherCard == excludedCard)
                continue;

            // Only consider cards that are visually above
            Rect otherRect = GetWorldRect(otherCard.GetComponent<RectTransform>());
            float otherCenterY = otherRect.center.y;

            if (otherCenterY <= targetCenterY)
                continue; // not above

            if (IsOverlapping(targetCard, otherCard, 5f))  // use small threshold now
            {
                Debug.Log($"Card {targetCard.name} is overlapped by higher card {otherCard.name}");
                return true;
            }
        }

        return false;
    }
    private bool IsOverlappedByAnyCard(Card cardToCheck)
    {
        foreach (Card otherCard in CardManager.instance.totalCardsToClear)
        {
            if (otherCard == cardToCheck)
                continue;

            if (IsOverlapping(otherCard, cardToCheck, 2f))
            {
                Debug.Log($"{cardToCheck.name} is overlapped by {otherCard.name}");
                return true;
            }
        }

        Debug.Log($"{cardToCheck.name} is not overlapped by any card and is in the top layer.");
        return false;
    }

    //private bool IsOverlapping(Card cardA, Card cardB)
    //{
    //    RectTransform rectA = cardA.GetComponent<RectTransform>();
    //    RectTransform rectB = cardB.GetComponent<RectTransform>();

    //    if (rectA == null || rectB == null) return false;

    //    Rect worldRectA = GetWorldRect(rectA);
    //    Rect worldRectB = GetWorldRect(rectB);

    //    Debug.Log($"Checking overlap between {cardA.name} and {cardB.name}");
    //    Debug.Log($"CardA Rect: {worldRectA}, CardB Rect: {worldRectB}");

    //    bool overlaps = worldRectA.Overlaps(worldRectB);
    //    Debug.Log($"Overlap result between {cardA.name} and {cardB.name}: {overlaps}");
    //    return overlaps;

    //}
    private bool IsOverlapping(Card cardA, Card cardB, float thresholdPercent = 2f)
    {
        RectTransform rectA = cardA.GetComponent<RectTransform>();
        RectTransform rectB = cardB.GetComponent<RectTransform>();

        if (rectA == null || rectB == null) return false;

        Rect worldRectA = GetWorldRect(rectA);
        Rect worldRectB = GetWorldRect(rectB);

        // Calculate overlap area
        Rect intersection = Rect.MinMaxRect(
            Mathf.Max(worldRectA.xMin, worldRectB.xMin),
            Mathf.Max(worldRectA.yMin, worldRectB.yMin),
            Mathf.Min(worldRectA.xMax, worldRectB.xMax),
            Mathf.Min(worldRectA.yMax, worldRectB.yMax)
        );

        // If there's no intersection, return false
        if (intersection.width <= 0 || intersection.height <= 0)
            return false;

        float overlapArea = intersection.width * intersection.height;
        float areaA = worldRectA.width * worldRectA.height;

        float overlapPercent = (overlapArea / areaA) * 100f;

        Debug.Log($"Overlap percent between {cardA.name} and {cardB.name}: {overlapPercent}%");

        return overlapPercent >= thresholdPercent;
    }
    private bool IsBlockedByAnyCard(Card targetCard)
    {
        Rect targetRect = GetWorldRect(targetCard.GetComponent<RectTransform>());

        foreach (Card otherCard in CardManager.instance.totalCardsToClear)
        {
            if (otherCard == targetCard)
                continue;

            // Make sure other card is *visually* above
            if (otherCard.transform.GetSiblingIndex() > targetCard.transform.GetSiblingIndex())
            {
                Rect otherRect = GetWorldRect(otherCard.GetComponent<RectTransform>());

                Rect intersection = Rect.MinMaxRect(
                    Mathf.Max(targetRect.xMin, otherRect.xMin),
                    Mathf.Max(targetRect.yMin, otherRect.yMin),
                    Mathf.Min(targetRect.xMax, otherRect.xMax),
                    Mathf.Min(targetRect.yMax, otherRect.yMax)
                );

                if (intersection.width > 0 && intersection.height > 0)
                {
                    float overlapArea = intersection.width * intersection.height;
                    float targetArea = targetRect.width * targetRect.height;
                    float overlapPercent = (overlapArea / targetArea) * 100f;

                    if (overlapPercent >= 5f)
                    {
                        Debug.Log($"Card {targetCard.name} is blocked by {otherCard.name} with {overlapPercent}% overlap.");
                        return true;
                    }
                }
            }
        }

        return false;
    }


    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        Debug.Log($"Corners for {rectTransform.name}: BL({corners[0]}), TL({corners[1]}), TR({corners[2]}), BR({corners[3]})");

        return new Rect(corners[0].x, corners[0].y, width, height);
        
    }
    public void FlipCard(Card card)
    {
        Debug.Log("card.belowCards.Count: " + card.belowCards.Count);
        Debug.Log("card.belowCards.Count: " + belowCards.Count);
        Debug.Log("isFlipping: " + isFlipping);
        if (isFlipping) return;
        //if (isFaceUp) return; 
        //isFlipping = true;
        float flipDuration = 0.2f;
        //for (int i = 0; i < belowCards.Count; i++)
        //{
            RectTransform rectTransform = card.GetComponent<RectTransform>();

        //isFaceUp = true;
        card.isFaceUp = true;
        rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {
                
                //isFaceUp = true;
                UpdateCardFlipping(card);
                rectTransform.DORotate(new Vector3(0, -90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                            //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
                        });
            });
        //}
    }
    private void UpdateCardFlipping(Card card)
    {

        //cardFace.SetActive(!isFaceUp);

        //for (int i = 0; i < belowCards.Count; i++)
        //{
            RectTransform rectTransform = card.GetComponent<RectTransform>();

            if (SlotManager.instance.goingBack)
            {
            
                for (int j = 0; j < 2; j++)
                {
                    rectTransform.GetChild(j).localRotation = Quaternion.Euler(0, 0, 0);
                }
                if (card.isWildCard)
                {
                    card.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(false);
                    card.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    card.cardFace.SetActive(false);
                }
            card.cardFace.SetActive(card.isFaceUp);
                card.isFaceUp = false;
            }
            else
            {
                for (int j = 0; j < 2; j++)
                {
                    //rectTransform.GetChild(j).localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (card.isWildCard)
                {
                    card.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
                    card.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    card.cardFace.SetActive(false);
                }
            cardFace.SetActive(!isFaceUp);
                isFaceUp = true;
            }
        CardManager.instance.UpdateFaceUpCards(this, isFaceUp);

        //}
    }
    private void UpdateCardFlipping(bool flipToFaceUp, Card eCard)
    {
        Debug.Log("++++++++++++++++++++++++++++++flipToFaceUp: " + flipToFaceUp+"___"+ CardManager.instance.extraCards.Count);
        //Card eCard = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1];
        RectTransform rectTransform = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1].GetComponent<RectTransform>();
        Debug.Log("++++++++++++++++++++++++++++++flipToFaceUp: " + rectTransform);
        if (flipToFaceUp)
        {
            for (int i = 0; i < GetComponent<RectTransform>().childCount; i++)
            {
                eCard.GetComponent<RectTransform>().GetChild(i).localRotation = Quaternion.Euler(0, 180, 0);
            }
                
            if(eCard.isWildCard)
            {
                eCard.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
                eCard.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                eCard.cardFace.SetActive(false);
            }

            CardManager.instance.extraCards.Remove(eCard);
            CardManager.instance.rightSideCards.Add(eCard);
            CardManager.instance.allFaceUpCards = CardManager.instance.allFaceUpCards.Except(CardManager.instance.rightSideCards).ToList();
            CardManager.instance.UpdateFaceUpCards(eCard, eCard.isFaceUp);
        }
        else
        {
            //var cardToFlip = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1];
            Debug.Log("cardToFlip: " + eCard.GetComponent<RectTransform>());
            for (int i = 0; i < eCard.GetComponent<RectTransform>().childCount; i++)
            {
                eCard.GetComponent<RectTransform>().GetChild(i).localRotation = Quaternion.Euler(0, 0, 0);
            }
            eCard.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(true);
            if (eCard.isWildCard)
            {
                eCard.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(false);
            }
        }


    }

    public void FlipCardBack()
    {
        //foreach (var card in rightSideCards)
        //{
        if (CardManager.instance.rightSideCards.Count <= 1) return;
        Debug.Log("CardManager.instance.rightSideCards.Count: " + CardManager.instance.rightSideCards.Count);
        if (CardManager.instance.extraCards.Count == 0)
        {
            GameManager.instance.HideMoreCardsToBuy();
        }
        var cardToFlip = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
        Debug.Log("cardToFlip: " + cardToFlip + "    lastCardPos: " + cardToFlip.lastCardPos);
        if(cardToFlip.GetComponent<RectTransform>().localEulerAngles.y == 0)
            cardToFlip.GetComponent<Image>().sprite = Resources.Load<Sprite>("FaceUpCard_0");
        
        cardToFlip.isFaceUp = false;
        cardToFlip.rectTransform.SetAsLastSibling();
        cardToFlip.rectTransform.DOAnchorPosX(originalPosOfExtraCards[CardManager.instance.extraCards.Count], 0.3f);
        Debug.Log("cardToFlip.GetComponent<RectTransform>().localEulerAngles.y: "+ cardToFlip.GetComponent<RectTransform>().localEulerAngles.y);
            cardToFlip.rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {
                //isFaceUp = true;
                UpdateCardFlipping(false, cardToFlip);
                cardToFlip.rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                    });
            });
            
        //int index = 0;
        if (CardManager.instance.rightSideCards.Count > 1)
        {
            var cardToMove = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
            CardManager.instance.rightSideCards.Remove(cardToMove);
            CardManager.instance.extraCards.Add(cardToMove);
            CardManager.instance.UpdateFaceUpCards(cardToMove, cardToMove.isFaceUp);
            CardManager.instance.UpdateFaceUpCards(CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1], true);
            //if (CardManager.instance.rightSideCards.Count >= 1)
            //{
            //    CardManager.instance.currentFaceupExtraCard = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
            //    CardManager.instance.UpdateFaceUpCards(CardManager.instance.currentFaceupExtraCard, CardManager.instance.currentFaceupExtraCard.isFaceUp);
            //}
        }


        //}
        Debug.Log("Hiiiii: " + CardManager.instance.rightSideCards.Count);
        if (CardManager.instance.rightSideCards.Count <= 1)
        {
            BackButton.instance.HideThis();
        }


    }

    


}
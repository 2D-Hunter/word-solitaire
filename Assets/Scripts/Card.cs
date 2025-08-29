using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using DG.Tweening;
using UnityEngine.EventSystems;

//[ExecuteAlways]
public class Card : MonoBehaviour
{
    public RectTransform thisCard;
    public List<Card> belowCards;
    public int Level;
    public bool isMoving = false;
    
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

    private CardSorting sorting;

    private void Awake()
    {
        
        if (thisCard == null)
            thisCard = GetComponent<RectTransform>();
        Debug.Log("______this.tag: "+ this.tag);
        if(this.tag == "ExtraCard")
        {
            rectTransform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("ExtraCard");
            int randomIndex = UnityEngine.Random.Range(0, CardManager.instance.extraCards.Count);
            
        }
        if (FBPlayerData.instance)
        {
            if (FBPlayerData.instance.CURRENT_LEVEL == 1)
            {
                gameObject.GetComponent<Button>().enabled = false;
            }
        }
        sorting = GetComponent<CardSorting>();
    }
    //private void OnValidate()
    //{
    //    //if (Card.instance.gameObject.tag != "ExtraCard")
    //    //{
    //    //Debug.Log("________isWildCard: " + isWildCard);
    //    cardFace.SetActive(!isFaceUp);
    //    if (isWildCard && isFaceUp)
    //    {
    //        gameObject.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(true);
    //        gameObject.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
    //    }
    //}
    private void Start()
    {

        if (FBPlayerData.instance.CURRENT_LEVEL >= 3 && this.tag == "ExtraCard")
        {
            Debug.Log("Child Count: "+GetComponent<RectTransform>().childCount);
            if(GetComponent<RectTransform>().childCount >= 5)
                GetComponent<RectTransform>().GetChild(4).gameObject.SetActive(false);
        }
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
        Debug.Log("_______isWildCarddd: " + gameObject.name+"_____" + isWildCard);
        if (isWildCard && isFaceUp)
        {
            gameObject.GetComponent<RectTransform>().GetChild(3).gameObject.SetActive(true);
            gameObject.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
        }
    }

    //public void MoveBackToOriginalPosition(Card tempCard, bool shouldcallbelow = true)
    public void MoveBackToOriginalPosition(Card tempCard, int sortingLayer, bool shouldcallbelow = true, Action onComplete = null)
    {
        
        
        if (this.tag == "ExtraCard")
        {
            Debug.Log("MoveBackToOriginalPosition Extra Card slotManager.ExtraInSlotcards "+ slotManager.ExtraInSlotcards.Count);
           // CardManager.instance.rightSideCards.AddCard(this);
           if(slotManager.ExtraInSlotcards.Count>0)
           {
                slotManager.ExtraInSlotcards.Reverse();
                CardManager.instance.rightSideCards.AddRange(slotManager.ExtraInSlotcards);
                slotManager.ExtraInSlotcards.Clear();
                for (int index = 0; index < CardManager.instance.rightSideCards.Count; index++)
                {
                    Card card = CardManager.instance.rightSideCards[index];
                    card.GetComponent<Canvas>().sortingOrder = index+1;
                    Debug.Log("MoveBackToOriginalPosition Extra Card :: "+ index + 1);
                }
           }
            

            
            if (cardSequence != null && cardSequence.IsActive())
            {
                //cardSequence.Kill(); // Clean up old sequence explicitly
            }

            var cardSequence1 = DOTween.Sequence();
            cardSequence1.Append(GetComponent<RectTransform>().DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutQuad));
            cardSequence1.Join(GetComponent<RectTransform>().DOScale(1f, 0.3f).SetEase(Ease.OutBack));
            cardSequence1.Join(GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));
            cardSequence1.OnComplete(() =>
            {
                var targetCard = this;
                isMoving = false;
                targetCard.GetComponent<CardSorting>().ResetOrder();
                
                Debug.Log("Card has reached back into the original position.");
                GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 180, 0);
                GetComponent<RectTransform>().GetChild(0).localRotation = Quaternion.Euler(0, -180, 0);
                GetComponent<RectTransform>().GetChild(1).localRotation = Quaternion.Euler(0, -180, 0);
                GetComponent<RectTransform>().GetChild(3).localRotation = Quaternion.Euler(0, -180, 0);

            });
        }
        else if (this.isWildCard)
        {
            Debug.Log("MoveBackToOriginalPosition Wild Card");

            if (cardSequence != null && cardSequence.IsActive())
            {
                //cardSequence.Kill(); // Clean up old sequence explicitly
            }

            var cardSequence2 = DOTween.Sequence();
            cardSequence2.Append(GetComponent<RectTransform>().DOAnchorPos(originalPosition, 0.2f).SetEase(Ease.OutQuad));
            cardSequence2.Join(GetComponent<RectTransform>().DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            cardSequence2.Join(GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    var targetCard = this;
                isMoving = false;
                targetCard.GetComponent<CardSorting>().ResetOrder();
                    
                    FBPlayerData.instance.TOTAL_WILD_CARD++;
                    FBPlayerData.instance.SavePlayerData();
                    FindObjectOfType<NumberOfWildCard>().UpdateWildCard();
                RemoveCardsButton.instance.sendBackAll = false;
                //Destroy(gameObject);
                //card = null;
            });
        }
        else
        {
            //GetComponent<RectTransform>().DOKill();
            Debug.Log("MoveBackToOriginalPosition Normal Card");

            if (cardSequence != null && cardSequence.IsActive())
            {
                //cardSequence.Kill(); // Clean up old sequence explicitly
            }

            var cardSequence3 = DOTween.Sequence();
            cardSequence3.Append(GetComponent<RectTransform>().DOAnchorPos(originalPosition, 0.2f).SetEase(Ease.OutQuad));
            cardSequence3.Join(GetComponent<RectTransform>().DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            cardSequence3.Join(GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    var targetCard = this;
                    Debug.Log("sendBackAll000");
                    targetCard.GetComponent<CardSorting>().ResetOrder();
                    
                    RemoveCardsButton.instance.sendBackAll = false;
                    isMoving = false;
                    if (shouldcallbelow)
                        SetFaceOfCard();

                    onComplete?.Invoke(); // ✅ THIS MUST BE HERE
    });


        }
    }
    void SetFaceOfCard()
    {
        Debug.Log("SetFaceOfCard: " + belowCards.Count+"____"+ RemoveCardsButton.instance.sendBackAll);
        if (belowCards.Count == 0 && RemoveCardsButton.instance.sendBackAll)
        {
            GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(true);
            isFaceUp = false;
        }
        if(belowCards.Count == 0)
        {
            GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
            isFaceUp = true;
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
        if(CardManager.instance.rightSideCards.Count > 0)
            CardManager.instance.UpdateFaceUpCards(CardManager.instance.rightSideCards[0], true);
    }

    public void OnCardClick(Card card)
    {
        RemoveCardsButton.instance.sendBackAll = false;
        SlotManager.instance.goingBack = false;
        if (InitManager.instance != null)
            if (InitManager.instance.levelCompleted) return;

        isMoving = true;

        if (this.tag == "ExtraCard")
        {
            GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
          
            if (isFaceUp && !SlotManager.instance.allSlotsOccupied)
            {
                slotManager.OnCardClicked(this);
            }
            else
            {
                OnExtraCardClick();
            }
            
        }
        else
        {
            if (!isFaceUp) return;
            if (!FBPlayerData.instance.TUTORIAL_1_COMPLETED || !FBPlayerData.instance.TUTORIAL_2_COMPLETED)
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
            
            
            //isFlipping = false;
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
        Debug.Log("OnExtraCardClick: " + eCard.name);
        eCard.GetComponent<Canvas>().sortingOrder = CardManager.instance.rightSideCards.Count+1;
        RectTransform rectTransform = eCard.GetComponent<RectTransform>();
        if (!isFaceUp)
        {
            lastCardPos = rectTransform.anchoredPosition.x;
            eCard.isFlipped = true;
            eCard.isFaceUp = true;
            
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
            Debug.Log($"🔎 Checking card: {belowCard.name}");
            bool overlapsWithTappedCard = IsOverlapping(this, belowCard, 2f);
            bool isBlocked = IsBlockedByOtherCards(belowCard);

            Debug.Log($"➡ {belowCard.name} isBlocked: {isBlocked}, siblingIndex: {belowCard.transform.GetSiblingIndex()}");

            if (overlapsWithTappedCard && !isBlocked && isFaceUp)
            {
                Debug.Log($"✅ Flipping {belowCard.name}");
                FlipCard(belowCard);
            }
            else
            {
                Debug.Log($"⛔ BLOCKED: {belowCard.name} will NOT flip");
                if (SlotManager.instance.goingBack && belowCard.isFaceUp)
                    belowCard.FlipCard(belowCard);
            }
        }
    }
    

    

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
    

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
    public void FlipCard(Card card)
    {
        if (card.isFlipping) return;
            card.isFlipping = true;
        Debug.Log("card.belowCards.Count: " + card.name + "______" + belowCards.Count);
        //Debug.Log("isFlipping: " + isFlipping);
        //if (isFlipping) return;
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
                        card.isFlipping = false;

                        if (!CardManager.instance.allFaceUpCards.Contains(card) && !slotManager.goingBack && !card.isWildCard)
                            CardManager.instance.allFaceUpCards.Add(card);
                        if (CardManager.instance.allFaceUpCards.Contains(card) && slotManager.goingBack)
                            CardManager.instance.allFaceUpCards.Remove(card);

                        Debug.Log("___Card flipped: " + slotManager.goingBack);
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
            CardManager.instance.rightSideCards.AddCard(eCard);
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
        Debug.Log("CardManager.instance.rightSideCards.Count: " + CardManager.instance.extraCards.Count);
        if (CardManager.instance.extraCards.Count == 0)
        {
            GameManager.instance.HideMoreCardsToBuy();
        }
        var cardToFlip = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
        Debug.Log("cardToFlip: " + cardToFlip + "    lastCardPos: " + cardToFlip.lastCardPos);
        if(cardToFlip.GetComponent<RectTransform>().localEulerAngles.y == 0)
            cardToFlip.GetComponent<Image>().sprite = Resources.Load<Sprite>("FaceUpCard_0");
        
        cardToFlip.isFaceUp = false;
        
       // cardToFlip.rectTransform.SetAsLastSibling();
        int leveIIndex = CardManager.instance.extraCards.Count;

        cardToFlip.GetComponent<Canvas>().sortingOrder = leveIIndex == 0 ? 1:leveIIndex+1;
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
            CardManager.instance.rightSideCards.RemoveCard(cardToMove);
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

    public void DisableClick()
    {
        Debug.Log("Click disabled");
        if (FBPlayerData.instance.CURRENT_LEVEL == 11)
        {
            gameObject.GetComponent<Button>().enabled = false;
        }
    }
    public void EnableClick()
    {
        Debug.Log("Click enabled");
        gameObject.GetComponent<Button>().enabled = true;
    }

    

    private bool IsSignificantOverlap(Rect a, Rect b, float thresholdPercent)
    {
        Rect overlap = Rect.MinMaxRect(
            Mathf.Max(a.xMin, b.xMin),
            Mathf.Max(a.yMin, b.yMin),
            Mathf.Min(a.xMax, b.xMax),
            Mathf.Min(a.yMax, b.yMax)
        );

        if (overlap.width <= 0 || overlap.height <= 0)
            return false;

        float overlapArea = overlap.width * overlap.height;
        float aArea = a.width * a.height;
        float overlapPercent = (overlapArea / aArea) * 100f;

        return overlapPercent >= thresholdPercent;
    }

   

    private bool IsBlockedByOtherCards(Card targetCard)
    {
        int targetIndex = targetCard.Level;//targetCard.transform.GetSiblingIndex();
        Rect targetRect = GetWorldRect(targetCard.rectTransform);

        foreach (Card otherCard in BoardManager.instance.activeCards)
        {
            if (otherCard == null || otherCard == targetCard)
                continue;

            if (!otherCard.gameObject.activeInHierarchy)
                continue;

            int otherIndex = otherCard.Level;
            Debug.Log($" 1 ❌ {targetCard.name} BLOCKED by {otherCard.name} (index {otherIndex})");
            if (otherIndex > targetIndex && !otherCard.isMoving)
            {
                Rect otherRect = GetWorldRect(otherCard.rectTransform);

                if (IsSignificantOverlap(targetRect, otherRect, 5f))
                {
                    Debug.Log($" 2❌ {targetCard.name} BLOCKED by {otherCard.name} (index {otherIndex})");
                    return true;
                }
            }
        }

        return false;
    }


}
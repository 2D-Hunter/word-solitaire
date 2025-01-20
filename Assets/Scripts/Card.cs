using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using DG.Tweening;

public class Card : MonoBehaviour
{
    public List<Card> belowCards;
    
    public RectTransform rectTransform;
    public static Card instance;
    private SlotManager slotManager;
    public GameObject cardFace;
    public bool isFaceUp = false;

    
    public List<Card> allFaceUpCards;

    private bool isFlipping = false;

    //Extra Cards

    
    private float lastCardPos;
    public float originalXPos;
    private float moveDistance = 275;
    private bool isFlipped = false;
    private int originalSiblingIndex;
    float flipDuration = 0.1f;

    private void Awake()
    {
        Debug.Log("______this.tag: "+ this.tag);
        if(this.tag == "ExtraCard")
        {
            rectTransform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("ExtraCard");
        }
    }
    private void Start()
    {
        instance = this;
        slotManager = FindObjectOfType<SlotManager>();
        
    }

    public void OnCardClick()
    {
        SlotManager.instance.goingBack = false;
        if (InitManager.instance != null)
            if (InitManager.instance.levelCompleted) return;

        

        if (this.tag == "ExtraCard")
        {
            GameObject extraCardContainer = GameObject.Find("UI-Panel/Extra Cards");
            extraCardContainer.transform.SetAsLastSibling();
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

    private void OnExtraCardClick()
    {
        
        Card eCard = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1];
        RectTransform rectTransform = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1].GetComponent<RectTransform>();


        if (!isFaceUp)
        {
            lastCardPos = rectTransform.anchoredPosition.x;
            Debug.Log("___Last Card Pos: " + lastCardPos);
            eCard.isFlipped = true;
            eCard.isFaceUp = true;

            originalSiblingIndex = rectTransform.GetSiblingIndex();
            Sequence flipSequence = DOTween.Sequence();
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

            rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {

                //isFaceUp = true;
                UpdateCardFlipping(true, eCard);
                rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                        //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
                    });
            });
            Debug.Log("______qqq");
            CardManager.instance.extraCards.Remove(eCard);
            CardManager.instance.rightSideCards.Add(eCard);
        }
        else
        {
            //SlotManager.instance.OnCardClicked(rectTransform);
        }
    }

    

    public void FlipImmediateBelowCards()
    {
        
        foreach (Card belowCard in belowCards)
        {
            if (belowCard == null) continue;

            Debug.Log($"Processing below card: {belowCard.name}");

            // Check overlap conditions
            bool overlapsWithTappedCard = IsOverlapping(this, belowCard);
            bool overlapsWithOtherCards = DoesOverlapWithAnyOtherCardExcept(belowCard, this);



            Debug.Log("____>>>overlapsWithTappedCard: " + overlapsWithTappedCard + "____" + this + "____" + belowCard);
            Debug.Log("____>>>overlapsWithOtherCards: " + overlapsWithOtherCards + "____" + belowCard + "____" + this);
            //if(this.name == "Card 5" && belowCard.name == "Card 1")
            //{
            //    belowCard.FlipCard();&& 
            //}
            if (overlapsWithTappedCard && !overlapsWithOtherCards && isFaceUp)
            {
                Debug.Log($"Flipping immediate below card: {belowCard.name}");
                belowCard.FlipCard();

                 //belowCard.FlipImmediateBelowCards();
            }
            else
            {
                Debug.Log($"Card {belowCard.name} is blocked or not directly below.");
                if(SlotManager.instance.goingBack && belowCard.isFaceUp)
                    belowCard.FlipCard();
            }
        }
    }

    private bool DoesOverlapWithAnyOtherCardExcept(Card targetCard, Card excludedCard)
    {
        foreach (Card otherCard in CardManager.instance.allCards)
        {
            if (otherCard == targetCard || otherCard == excludedCard)
            {
                continue;
            }

            if (!otherCard.isFaceUp) continue;

            if (IsOverlapping(targetCard, otherCard))
            {
                Debug.Log($"Overlap detected: {targetCard.name} overlaps with top-layer card {otherCard.name}");
                return true;
            }
        }

        return false;
    }
    private bool IsOverlappedByAnyCard(Card cardToCheck)
    {
        foreach (Card otherCard in CardManager.instance.allCards)
        {
            if (otherCard == cardToCheck)
                continue;

            if (IsOverlapping(otherCard, cardToCheck))
            {
                Debug.Log($"{cardToCheck.name} is overlapped by {otherCard.name}");
                return true;
            }
        }

        Debug.Log($"{cardToCheck.name} is not overlapped by any card and is in the top layer.");
        return false;
    }

    private bool IsOverlapping(Card cardA, Card cardB)
    {
        RectTransform rectA = cardA.GetComponent<RectTransform>();
        RectTransform rectB = cardB.GetComponent<RectTransform>();

        if (rectA == null || rectB == null) return false;

        Rect worldRectA = GetWorldRect(rectA);
        Rect worldRectB = GetWorldRect(rectB);

        Debug.Log($"Checking overlap between {cardA.name} and {cardB.name}");
        Debug.Log($"CardA Rect: {worldRectA}, CardB Rect: {worldRectB}");

        bool overlaps = worldRectA.Overlaps(worldRectB);
        Debug.Log($"Overlap result between {cardA.name} and {cardB.name}: {overlaps}");
        return overlaps;
        
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
    public void FlipCard()
    {
        Debug.Log("belowCard.Count: " + belowCards.Count);
        Debug.Log("isFaceUp: " + isFaceUp);
        if (isFlipping) return;
        //if (isFaceUp) return; 
        isFlipping = true;
        float flipDuration = 0.2f;
        //for (int i = 0; i < belowCards.Count; i++)
        //{
        RectTransform rectTransform = GetComponent<RectTransform>();
        isFaceUp = true;

        rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {

                    //isFaceUp = true;
                    UpdateCardFlipping();
                rectTransform.DORotate(new Vector3(0, -90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                            //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
                        });
            });
        //}
    }
    private void UpdateCardFlipping()
    {

        //cardFace.SetActive(!isFaceUp);

        //for (int i = 0; i < belowCards.Count; i++)
        //{
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (SlotManager.instance.goingBack)
            {
            
                for (int j = 0; j < 2; j++)
                {
                    rectTransform.GetChild(j).localRotation = Quaternion.Euler(0, 0, 0);
                }
                cardFace.SetActive(isFaceUp);
                isFaceUp = false;
            }
            else
            {
                for (int j = 0; j < 2; j++)
                {
                    //rectTransform.GetChild(j).localRotation = Quaternion.Euler(0, 180, 0);
                }
                cardFace.SetActive(!isFaceUp);
                isFaceUp = true;
            }

        //}
    }
    private void UpdateCardFlipping(bool flipToFaceUp, Card eCard)
    {
        Debug.Log("++++++++++++++++++++++++++++++flipToFaceUp: " + flipToFaceUp);
        //Card eCard = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1];
        RectTransform rectTransform = CardManager.instance.extraCards[CardManager.instance.extraCards.Count - 1].GetComponent<RectTransform>();
        if (flipToFaceUp)
        {
            for (int i = 0; i < GetComponent<RectTransform>().childCount; i++)
            {
                eCard.GetComponent<RectTransform>().GetChild(i).localRotation = Quaternion.Euler(0, 180, 0);
            }
            eCard.cardFace.SetActive(false);
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
        }


    }

    public void FlipCardBack()
    {
        //foreach (var card in rightSideCards)
        //{
        if (CardManager.instance.rightSideCards.Count <= 1) return;
        Debug.Log("CardManager.instance.rightSideCards.Count: " + CardManager.instance.rightSideCards.Count);
        var cardToFlip = CardManager.instance.rightSideCards[CardManager.instance.rightSideCards.Count - 1];
        Debug.Log("cardToFlip: " + cardToFlip + "    lastCardPos: " + cardToFlip.lastCardPos);
        if(cardToFlip.GetComponent<RectTransform>().localEulerAngles.y == 0)
            cardToFlip.GetComponent<Image>().sprite = Resources.Load<Sprite>("FaceUpCard_0");
        
        cardToFlip.isFaceUp = false;
        cardToFlip.rectTransform.SetAsLastSibling();
        cardToFlip.rectTransform.DOAnchorPosX(cardToFlip.lastCardPos, 0.3f);
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
        }


        //}
        Debug.Log("Hiiiii: " + CardManager.instance.rightSideCards.Count);
        if (CardManager.instance.rightSideCards.Count <= 1)
        {
            BackButton.instance.HideThis();
        }


    }
}
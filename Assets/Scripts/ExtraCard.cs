using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
public class ExtraCard : MonoBehaviour
{
    public static ExtraCard instance;
    public Image faceDownImage;
    //public Image faceUpImage;   
    //private float flipDuration = 0.5f;
    private float moveDistance = 275;

    private RectTransform rectTransform;
    private bool isFlipped = false;
    private int originalSiblingIndex;

    bool isFlipping = true;
    float flipDuration = 0.1f;
    public GameObject cardFace;

    public bool isFaceUp = false;
    public Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();
    private float lastCardPos;
    public float originalXPos;


    private void Start()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        faceDownImage.gameObject.SetActive(true);
        StartCoroutine(StoreOriginalPositions());
        
    }
    IEnumerator StoreOriginalPositions()
    {
        yield return new WaitForEndOfFrame();
        foreach (var card in ExtraCardManager.instance.rightSideCards)
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                originalPositions[cardRect] = cardRect.anchoredPosition;
                Debug.Log("+++++++::: orig" + originalPositions[cardRect]);
            }
        }
    }

    public void OnCardTapped()
    {
        Debug.Log("isFaceUp: " + isFaceUp);
        //if (isFaceUp) return;
        if (!isFaceUp)
        {
            lastCardPos = rectTransform.anchoredPosition.x;
            Debug.Log("___Last Card Pos: " + lastCardPos);
            isFlipped = true;
            isFaceUp = true;

            originalSiblingIndex = rectTransform.GetSiblingIndex();
            Sequence flipSequence = DOTween.Sequence();
            rectTransform.SetAsLastSibling();
            if (ExtraCardManager.instance.rightSideCards.Count == 0)
                moveDistance = 200;
            else if (ExtraCardManager.instance.rightSideCards.Count == 1)
                moveDistance = 220;
            if (ExtraCardManager.instance.rightSideCards.Count >= 2)
                moveDistance = 240;
            rectTransform.DOAnchorPosX(moveDistance, 0.3f);
            ExtraCardManager.instance.extraCards.Remove(this);
            ExtraCardManager.instance.rightSideCards.Add(this);

            if (ExtraCardManager.instance.rightSideCards.Count >= 1)
            {
                GameManager.instance.ShowBackButton();
            }

            rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {

                //isFaceUp = true;
                UpdateCardFlipping(true);
                rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                        //rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.15f).SetLoops(2, LoopType.Yoyo);
                    });
            });
        }
        else
        {
            //SlotManager.instance.OnCardClicked(rectTransform);
        }
    }
    private void UpdateCardFlipping(bool flipToFaceUp)
    {
        Debug.Log("++++++++++++++++++++++++++++++flipToFaceUp: "+ flipToFaceUp);
        if(flipToFaceUp)
        {
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                rectTransform.GetChild(i).localRotation = Quaternion.Euler(0, 180, 0);
            }
            cardFace.SetActive(false);
        }
        else
        {
            var cardToFlip = ExtraCardManager.instance.extraCards[ExtraCardManager.instance.extraCards.Count - 1];
            Debug.Log("cardToFlip: " + cardToFlip);
            for (int i = 0; i < cardToFlip.rectTransform.childCount; i++)
            {
                cardToFlip.rectTransform.GetChild(i).localRotation = Quaternion.Euler(0, 0, 0);
            }
            cardToFlip.rectTransform.GetChild(2).gameObject.SetActive(true);
        }
        
        
    }

    public void FlipCardBack()
    {
        //foreach (var card in ExtraCardManager.instance.rightSideCards)
        //{
        if (ExtraCardManager.instance.rightSideCards.Count <= 1) return;
        Debug.Log("ExtraCardManager.instance.rightSideCards.Count: "+ ExtraCardManager.instance.rightSideCards.Count);
        var cardToFlip = ExtraCardManager.instance.rightSideCards[ExtraCardManager.instance.rightSideCards.Count-1];
        Debug.Log("cardToFlip: " + cardToFlip + "    originalXPos: "+ cardToFlip.originalXPos);
        
        cardToFlip.isFaceUp = false;
        cardToFlip.rectTransform.SetAsLastSibling();
        cardToFlip.rectTransform.DOAnchorPosX(cardToFlip.originalXPos, 0.3f);
        cardToFlip.rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
            .OnComplete(() =>
            {

                //isFaceUp = true;
                UpdateCardFlipping(false);
                cardToFlip.rectTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        isFlipping = false;
                    });
            });
        //int index = 0;
        if (ExtraCardManager.instance.rightSideCards.Count > 1)
        {
            var cardToMove = ExtraCardManager.instance.rightSideCards[ExtraCardManager.instance.rightSideCards.Count - 1];
            ExtraCardManager.instance.rightSideCards.Remove(cardToMove); // Use RemoveAt for index-based removal
            ExtraCardManager.instance.extraCards.Add(cardToMove);
        }
        
            
        //}
        Debug.Log("Hiiiii: " + ExtraCardManager.instance.rightSideCards.Count);
        if (ExtraCardManager.instance.rightSideCards.Count <= 1)
        {
            BackButton.instance.HideThis();
        }


    }
}


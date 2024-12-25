using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public List<RectTransform> cardSlots;
    private Dictionary<RectTransform, Transform> slotToCardMap;
    private List<bool> isSlotOccupied;
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();


    void Start()
    {
        isSlotOccupied = new List<bool>(new bool[cardSlots.Count]);
        slotToCardMap = new Dictionary<RectTransform, Transform>();
        originalPositions = new Dictionary<RectTransform, Vector2>();

        StartCoroutine(StoreOriginalPositions());


    }
    IEnumerator StoreOriginalPositions()
    {
        yield return new WaitForEndOfFrame();
        foreach (var card in FindObjectsOfType<Card>())
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                originalPositions[cardRect] = cardRect.anchoredPosition;
                Debug.Log("+++++++::: " + originalPositions[cardRect]);
            }
        }
    }

    public void OnCardClicked(RectTransform card)
    {
        Debug.Log("___Card Clicked...");
        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card);


        if (currentSlotIndex != -1)
        {
            // the card is in a slot
            isSlotOccupied[currentSlotIndex] = false;
            slotToCardMap.Remove(cardSlots[currentSlotIndex]);
            if (originalPositions.ContainsKey(card))
            {
                Sequence cardSequence = DOTween.Sequence();

                cardSequence.Append(card.DOAnchorPos(originalPositions[card], 0.3f).SetEase(Ease.OutQuad));
                cardSequence.Join(card.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
                cardSequence.Join(card.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));
            }
        }
        else
        {
            // the card is not in a slot
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
                isSlotOccupied[emptySlotIndex] = true;
                slotToCardMap[cardSlots[emptySlotIndex]] = card;

                
                Vector2 targetPosition = cardSlots[emptySlotIndex].position;
                Vector3 targetRotation = new Vector3(0, 360, 360);

                // decide  the card moves direction depending on target slots
                float lateralOffset = cardSlots[emptySlotIndex].position.x;

                
                Sequence cardSequence = DOTween.Sequence();

                //1st Move the card upward and sideways (left or right)
                Vector3 upwardPosition = card.position + new Vector3(lateralOffset, 1.7f, 0);
                cardSequence.Append(card.DOMove(upwardPosition, 0.05f).SetEase(Ease.OutQuad));



                //move the card to the slot with rotation
                cardSequence.Append(card.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad));
                cardSequence.Join(card.DOScale(0.83f, 0.3f).SetEase(Ease.OutBack));
                cardSequence.Join(card.DORotate(targetRotation, 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

                // once animation is complete below one will be excute.
                cardSequence.OnComplete(() =>
                {
                    Debug.Log("Card has reached the slot.");
                });
            }
        }
        Debug.Log(GetSlotString());
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
                    slotString += cardData.letter; // adding the letter to the string
                }
            }
        }

        return slotString;
    }
}
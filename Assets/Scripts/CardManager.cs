using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    public List<RectTransform> cardSlots; // List to store the card slots
    private Dictionary<RectTransform, Transform> slotToCardMap; // Maps slots to cards
    private List<bool> isSlotOccupied; // Keeps track of which slots are occupied
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>(); // Tracks original card positions


    void Start()
    {
        // Initialize the occupation status of each slot
        isSlotOccupied = new List<bool>(new bool[cardSlots.Count]);
        slotToCardMap = new Dictionary<RectTransform, Transform>();
        originalPositions = new Dictionary<RectTransform, Vector2>();

        // Store original positions after layout calculation
        StartCoroutine(StoreOriginalPositions());


    }
    IEnumerator StoreOriginalPositions()
    {
        yield return new WaitForEndOfFrame(); // Wait for layout calculation
        // Store the original positions of all cards
        foreach (var card in FindObjectsOfType<Card>())
        {
            //originalPositions[card.transform] = card.transform.position;

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
        // Check if the card is in a slot
        int currentSlotIndex = cardSlots.FindIndex(slot => slotToCardMap.ContainsKey(slot) && slotToCardMap[slot] == card);


        if (currentSlotIndex != -1)
        {
            // If the card is in a slot, remove it from the slot, return it to its original position
            isSlotOccupied[currentSlotIndex] = false; // Mark the slot as empty
            slotToCardMap.Remove(cardSlots[currentSlotIndex]); // Remove mapping
            if (originalPositions.ContainsKey(card))
            {
                Sequence cardSequence = DOTween.Sequence();

                cardSequence.Append(card.DOAnchorPos(originalPositions[card], 0.3f).SetEase(Ease.OutQuad));
                cardSequence.Join(card.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
                cardSequence.Join(card.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

                //card.DOAnchorPos(originalPositions[card], 0.5f).SetEase(Ease.OutQuad); // Move back to original position
                //card.Join(card.DOScale(0.83f, 0.2f).SetEase(Ease.OutQuad)); // Reset Scaling
                //card.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutQuad); // Reset rotation
            }
        }
        else
        {
            // If the card is not in a slot, move it to the first available slot
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
                // Mark the slot as occupied and map the card to the slot
                isSlotOccupied[emptySlotIndex] = true;
                slotToCardMap[cardSlots[emptySlotIndex]] = card;

                // Get the target position and rotation
                Vector2 targetPosition = cardSlots[emptySlotIndex].position;
                Vector3 targetRotation = new Vector3(0, 360, 360);
                float lateralOffset = cardSlots[emptySlotIndex].position.x;// Decide  the card moves left or right

                // Create a sequence for the animations
                Sequence cardSequence = DOTween.Sequence();

                //Move the card upward and sideways (left or right)
                Vector3 upwardPosition = card.position + new Vector3(lateralOffset, 1.7f, 0); // Higher upward movement
                cardSequence.Append(card.DOMove(upwardPosition, 0.05f).SetEase(Ease.OutQuad));
                //card.DORotate(new Vector3(0, 0, 15), 0.3f).SetEase(Ease.OutQuad).SetRelative(true);



                //Move the card to the slot and with rotation
                cardSequence.Append(card.DOMove(targetPosition, 0.3f).SetEase(Ease.OutQuad));
                cardSequence.Join(card.DOScale(0.83f, 0.3f).SetEase(Ease.OutBack));
                cardSequence.Join(card.DORotate(targetRotation, 0.3f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));

                // callback for when the animation is complete
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

        // Iterate through the slots and build the string
        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (slotToCardMap.ContainsKey(cardSlots[i]))
            {
                // Get the card's letter from the CardData component
                CardData cardData = slotToCardMap[cardSlots[i]].GetComponent<CardData>();
                if (cardData != null)
                {
                    slotString += cardData.letter; // Add the letter to the string
                }
            }
        }

        return slotString;
    }
}
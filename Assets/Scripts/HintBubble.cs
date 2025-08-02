using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Word;

public class HintBubble : MonoBehaviour
{
    public static HintBubble instance;
    private RectTransform rectTransform;
    public TextMeshProUGUI hintTxt;
    public GameObject bubble1 = null;
    public GameObject bubble2 = null;
    public GameObject bubble3 = null;
    public GameObject bubble4 = null;
    private void Awake()
    {
        instance = this;
        bubble1.SetActive(false);
        bubble2.SetActive(false);
        bubble3.SetActive(false);
        bubble4.SetActive(false);
    }
    public void AnimateBubble()
    {
        Debug.Log("AnimateBubble: "+ SlotManager.instance.slotsCard.Count);
        if(SlotManager.instance.slotsCard.Count > 0)
        {
            Debug.Log("AnimateBubble: " + SlotManager.instance.GetSlotString() + "____"+ GameManager.instance.hintWord);
            if (SlotManager.instance.GetSlotString() == GameManager.instance.hintWord)
            {
                bubble1.SetActive(true);
                hintTxt.text = GameManager.instance.hintText;
                AnalyticsManager.Instance.TrackHintUsed(FBPlayerData.instance.CURRENT_LEVEL, GameManager.instance.hintText);
            }
            else
                bubble2.SetActive(true);
        }
        else
        {
            Debug.Log("AnimateBubble: " + GameManager.instance.foundValidWord);
            if (GameManager.instance.foundValidWord)
            {
                bubble1.SetActive(true);
                hintTxt.text = GameManager.instance.hintText;
                AnalyticsManager.Instance.TrackHintUsed(FBPlayerData.instance.CURRENT_LEVEL, GameManager.instance.hintText);
            }
            else
            {
                if (CardManager.instance.extraCards.Count <= 0)
                    bubble4.SetActive(true);
                else
                    bubble3.SetActive(true);
            }
                
        }
        
        rectTransform = GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        
        sequence.Append(
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 20, 0.3f) // Move up
                .SetEase(Ease.InOutSine).SetDelay(0.3f)
                .SetLoops(4, LoopType.Yoyo) // Loops up and down twice
        );
        sequence.AppendInterval(0.5f);
        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Word;
using DG.Tweening;
using TMPro;

public class HintButton : MonoBehaviour
{
    public RectTransform hintBubblePrefab;
    public GameObject parentObj;
    private RectTransform currentBubble;
    

    public void OnclickHint()
    {
        
        WordServiceContainer.HintService.HintClick((isfound , cards) =>
        {
            if (currentBubble != null)
            {
                Destroy(currentBubble.gameObject);
            }
            if (isfound) {
                Debug.Log("high light " + cards.Count + "____");
                currentBubble = Instantiate(hintBubblePrefab, parentObj.transform);
                currentBubble.anchoredPosition = new Vector2(384.5f, -621.5f);
                currentBubble.localScale = Vector3.zero;
                HintBubble.instance.AnimateBubble();
            }
            else
            {
                Debug.Log("Open Extra Hint ");
            }
        });
    }
}

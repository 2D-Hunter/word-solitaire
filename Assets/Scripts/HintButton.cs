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
        FBPlayerData.instance.VibrationEffect();
        SoundManager.instance.PlaySFX("HintSound", 0.3f);
        WordServiceContainer.HintService.HintClick((isfound , cards) =>
        {
            if (currentBubble != null)
            {
                Destroy(currentBubble.gameObject);
            }
            //if (isfound) {
            //    Debug.Log("high light " + cards.Count + "____");
                currentBubble = Instantiate(hintBubblePrefab, parentObj.transform);

                // Get RectTransform components
                RectTransform rt = gameObject.GetComponent<RectTransform>();
                RectTransform prefabRect = currentBubble.GetComponent<RectTransform>();
                RectTransform parentRect = parentObj.GetComponent<RectTransform>();
                Debug.Log(rt);
                Debug.Log(prefabRect);
                Debug.Log(parentRect);
                Canvas.ForceUpdateCanvases();
                // Convert button world position to UI local position
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rt.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out Vector2 localPoint);

                localPoint.y += 95;
                // Assign new position
                prefabRect.anchoredPosition = localPoint;

                //currentBubble.anchoredPosition = new Vector2(385f, 100f);
                //currentBubble.anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
                currentBubble.localScale = Vector3.zero;
                HintBubble.instance.AnimateBubble();
            //}
            //else
            //{
            //    Debug.Log("Open Extra Hint ");
            //}
        });
    }
    void InstantiateBubble()
    {

    }
}

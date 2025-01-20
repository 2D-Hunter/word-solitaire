using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    public static Hud instance;
    public RectTransform hudCard;
    public RectTransform hudStar;
    public int currentTarget;
    

    private void Start()
    {
        instance = this;
        currentTarget = CardManager.instance.totalCardToGet;
        Debug.Log("currentTarget: " + currentTarget);
    }

    public void DecreaseTarget()
    {
        Debug.Log("DecreaseTarget: ");
        int previousScore = currentTarget;
        currentTarget -= 1;
        //LevelManager.instance.currentLevelTarget.GetComponent<RectTransform>().localScale = Vector3.one * 2f;
        
        LevelManager.instance.currentLevelTarget.GetComponent<RectTransform>().DOScale(3f, 0.1f)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                LevelManager.instance.currentLevelTarget.text = LevelManager.instance.currentLevelTargetShadow.text = currentTarget.ToString();
                LevelManager.instance.currentLevelTarget.GetComponent<RectTransform>().DOScale(1f, 0.1f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                Debug.Log("Scale Up Completed!");
            });
            });
    }
}

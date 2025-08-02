using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BonusHud : MonoBehaviour
{
    public TextMeshProUGUI bonusTxt;
    public TextMeshProUGUI bonusShadowTxt;
    public GameObject rays;
    public float rotationSpeed = 30f;
    public LevelData levelData;
    public int currentTarget;
    private bool isTargetCompleted = false;
    private int displayedScore = 0;
    public GameObject txtObj;
    public GameObject txtObj2;
    public GameObject tickMark;

    public TextMeshProUGUI numberOfLetters;
    public TextMeshProUGUI numberOfLettersShadow;
    public TextMeshProUGUI totalWordToGetTxt;
    public TextMeshProUGUI totalWordToGetTxtShadow;

    public GameObject bonusGoal_Points, bonusGoal_NumberOfCards;
    public CanvasGroup canvasGroup_Tick;
    public bool bonusTargetAchieved = false;

    

    
    public BonusGoalType currentBonusGoalType;

    private void Start()
    {
        if(InitManager.instance.isReplay)
        {
            currentBonusGoalType = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-2].bonusGoalType;
        }
        else
        {
            currentBonusGoalType = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-1].bonusGoalType;
        }
        
        SetBonusGoal(currentBonusGoalType);
        tickMark.SetActive(false);
        bonusTargetAchieved = false;
        if (InitManager.instance.isReplay)
        {
            bonusTxt.text = bonusShadowTxt.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].targetPointsForBonus.ToString();
            currentTarget = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].targetPointsForBonus;
        }
        else
        {
            bonusTxt.text = bonusShadowTxt.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].targetPointsForBonus.ToString();
            currentTarget = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].targetPointsForBonus;
        }
            
    }
    void SetBonusGoal(BonusGoalType goalType)
    {
        bonusGoal_Points.SetActive(false);
        bonusGoal_NumberOfCards.SetActive(false);
        Debug.Log("_____currentBonusGoalType: " + currentBonusGoalType);
        switch (goalType)
        {
            case BonusGoalType.None:
                
                break;
            case BonusGoalType.Points:
                bonusGoal_Points.SetActive(true);
                break;
            case BonusGoalType.NumberOfCards:
                bonusGoal_NumberOfCards.SetActive(true);
                if (InitManager.instance.isReplay)
                {
                    numberOfLetters.text = numberOfLettersShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfLetters.ToString() + "+ Card";
                    totalWordToGetTxt.text = totalWordToGetTxtShadow.text = "0/" + levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfWords.ToString();
                }
                else
                {
                    numberOfLetters.text = numberOfLettersShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].numberOfLetters.ToString() + "+ Card";
                    totalWordToGetTxt.text = totalWordToGetTxtShadow.text = "0/" + levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].numberOfWords.ToString();
                }
                    
                break;
        }

    }
    void Update()
    {
        // Rotate around the Z axis
        rays.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }


    public void DecreaseTarget()
    {
        

        Debug.Log("DecreaseTarget:");

        RectTransform targetTransform = LevelManager.instance.bonusTarget.GetComponent<RectTransform>();

        targetTransform.DOScale(3f, 0.1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                targetTransform.DOScale(1f, 0.1f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    int decreaseAmount = GameManager.instance.gainedPoint;
                    int endValue = Mathf.Max(0, currentTarget - decreaseAmount); // prevent negative score

                AnimateScoreChange(currentTarget, endValue);

                    currentTarget = endValue; // update currentTarget
                    
                });
            });
    }

    void AnimateScoreChange(int startScore, int endScore)
    {
        Debug.Log($"Tween Complete! displayedScore hohoho: {displayedScore}, endScore: {endScore}");
        
        DOTween.To(() => startScore, x =>
        {
            startScore = x;
            bonusTxt.text = bonusShadowTxt.text = startScore.ToString();
            displayedScore = startScore;
        }, endScore, 1f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            bonusTxt.text = bonusShadowTxt.text = endScore.ToString();
            LevelManager.instance.bonusTarget.text =
            LevelManager.instance.bonusTargetShadow.text = endScore.ToString();
            displayedScore = endScore;
            Debug.Log($"Tween Complete! displayedScore: {displayedScore}, endScore: {endScore}");
            if (endScore <= 0 && !isTargetCompleted)
            {
                isTargetCompleted = true;
                Debug.Log("Invoke.....TargetAchievedForBonus");
                Invoke("GrantBonus", 0.1f);
            }
            
        });
    }
    public void AnimateNumberOfWordsAchieved()
    {
        if(InitManager.instance.isReplay)
            totalWordToGetTxt.text = totalWordToGetTxtShadow.text = GameManager.instance.wordCounter.ToString() + "/"+ levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfWords.ToString();
        else
            totalWordToGetTxt.text = totalWordToGetTxtShadow.text = GameManager.instance.wordCounter.ToString() + "/" + levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-1].numberOfWords.ToString();
        RectTransform targetTransform = totalWordToGetTxt.GetComponent<RectTransform>();

        targetTransform.DOScale(3f, 0.1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                targetTransform.DOScale(1f, 0.1f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    if (InitManager.instance.isReplay)
                    {
                        if (GameManager.instance.wordCounter >= levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfWords)
                            Invoke("GrantBonus", 0.3f);
                    }
                    else
                    {
                        if (GameManager.instance.wordCounter >= levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].numberOfWords)
                            Invoke("GrantBonus", 0.3f);
                    }
                        

                });
            });
    }
    void GrantBonus()
    {
        bonusTargetAchieved = true;
        

        tickMark.SetActive(true);
        canvasGroup_Tick.alpha = 0f;

        // Fade to fully visible over 1 second
        canvasGroup_Tick.DOFade(1f, 0.5f);
        RectTransform rectTransform = tickMark.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(3f, 3f, 3f);
        rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutFlash);
        if (txtObj)
            txtObj.SetActive(false);
        if (txtObj2)
            txtObj2.SetActive(false);
    }
}

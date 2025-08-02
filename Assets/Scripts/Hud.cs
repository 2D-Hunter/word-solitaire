using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    public static Hud instance;
    public RectTransform hudCard;
    public RectTransform hudStar;
    public int currentTarget;
    public BonusHud bonusHud;
    public LevelData levelData;
    
    public Image hudImage1;
    public Image hudImage2;

    private void Start()
    {
        instance = this;
        currentTarget = CardManager.instance.totalCardToGet;
        if(FBPlayerData.instance.CURRENT_LEVEL >= 26)
            bonusHud.GetComponent<BonusHud>();
        Debug.Log("currentTarget: " + currentTarget);
        LoadHUDImages();

    }
    void LoadHUDImages()
    {
        bool isHardLevel;
        if (InitManager.instance.isReplay)
             isHardLevel = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].isLevelHard;
        else
             isHardLevel = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-1].isLevelHard;

        if (isHardLevel)
        {
            hudImage1.sprite = Resources.Load<Sprite>("Hud-11");
            hudImage2.sprite = Resources.Load<Sprite>("Hud-22");
        }
        else
        {
            hudImage1.sprite = Resources.Load<Sprite>("Hud-1");
            hudImage2.sprite = Resources.Load<Sprite>("Hud-2");
        }
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
                Debug.Log("Scale Up Completed! " + SlotManager.instance.GetSlotString().Length);
                Debug.Log("_____currentBonusGoalType: " + bonusHud.currentBonusGoalType);
                switch (bonusHud.currentBonusGoalType)
                {
                    case BonusGoalType.None:

                        break;
                    case BonusGoalType.Points:
                        if (GameManager.instance.animateBonusTarget && !bonusHud.bonusTargetAchieved)
                            bonusHud.DecreaseTarget();
                        break;
                    case BonusGoalType.NumberOfCards:
                        if(InitManager.instance.isReplay)
                        {
                            if (GameManager.instance.submittedWordLength >= levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].numberOfLetters && GameManager.instance.animateBonusTarget && !bonusHud.bonusTargetAchieved)
                                bonusHud.AnimateNumberOfWordsAchieved();
                        }
                        else
                        {
                            if (GameManager.instance.submittedWordLength >= levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-1].numberOfLetters && GameManager.instance.animateBonusTarget && !bonusHud.bonusTargetAchieved)
                                bonusHud.AnimateNumberOfWordsAchieved();
                        }
                        
                        break;
                }
                GameManager.instance.animateBonusTarget = false;

            });
            });
    }
}

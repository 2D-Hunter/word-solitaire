using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalPopup : MonoBehaviour
{
    public static GoalPopup instance;
    public LevelData levelData;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupObj = null;

    public CanvasGroup playBtn = null;
    public RectTransform playBtnRectTransform = null;

    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI currentLevelShadow;
    public TextMeshProUGUI goal;
    public TextMeshProUGUI goalShadow;
    public GameObject[] starsObj;
    public CanvasGroup[] stars = null;
    public RectTransform[] starsRectTransform = null;

    //float delay = 0f;
    float delayIncrement = 0.1f;

    public GameObject mainGoal;
    public GameObject mainGoal1;
    public GameObject bonusGoal1;

    public TextMeshProUGUI goal1;
    public TextMeshProUGUI goal1Shadow;

    public TextMeshProUGUI reward;
    public TextMeshProUGUI rewardShadow;

    public TextMeshProUGUI targetForBonus;
    public TextMeshProUGUI targetForBonusShadow;

    public GameObject bonusGoal_Points, bonusGoal_NumberOfCards;

    public TextMeshProUGUI numberOfLetters;
    public TextMeshProUGUI numberOfLettersShadow;
    public TextMeshProUGUI totalWordToGetTxt;
    public TextMeshProUGUI totalWordToGetTxtShadow;
    public BonusGoalType currentBonusGoalType;
    public GameObject hardLabel = null;

    private void Awake()
    {
        instance = this;
        if(InitManager.instance.CurrentScene == "Levelup")
        {
            currentBonusGoalType = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL-2].bonusGoalType;
            if (levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].isLevelHard)
                hardLabel.SetActive(true);
            else
                hardLabel.SetActive(false);
        }
        else
        {
            
            currentBonusGoalType = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].bonusGoalType;
            if (levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].isLevelHard)
                hardLabel.SetActive(true);
            else
                hardLabel.SetActive(false);
        }
        
        SetInit();
    }
    private void SetInit()
    {
        SetBonusGoal(currentBonusGoalType);
        foreach (var star in stars)
        {
            star.alpha = 0;
        }
        foreach (var rt in starsRectTransform)
        {
            rt.localScale = Vector3.zero;
        }
        bg.alpha = 0;
        popup.alpha = 0;
        playBtn.alpha = 0;
        playBtnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        popupObj.anchoredPosition = new Vector2(0, -350);
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            currentLevel.text = currentLevelShadow.text = "Level " + (FBPlayerData.instance.CURRENT_LEVEL-1).ToString();
            if (FBPlayerData.instance.CURRENT_LEVEL > 26)
            {
                mainGoal.SetActive(false);
                mainGoal1.SetActive(true);
                bonusGoal1.SetActive(true);
                int levelIndex = FBPlayerData.instance.CURRENT_LEVEL - (InitManager.instance.CurrentScene == "Levelup" ? 2 : 1);

                // Safety check to avoid out-of-bounds access
                levelIndex = Mathf.Clamp(levelIndex, 0, levelData.levels.Count - 1);

                var level = levelData.levels[levelIndex];

                goal1.text = goal1Shadow.text = level.levelTarget.ToString();
                targetForBonus.text = targetForBonusShadow.text = level.targetPointsForBonus.ToString();
                reward.text = rewardShadow.text = $"Reward:   +{level.reward}";
            }
            else
            {
                mainGoal.SetActive(true);
                mainGoal1.SetActive(false);
                bonusGoal1.SetActive(false);
                if (InitManager.instance.CurrentScene == "Levelup")
                {
                    goal.text = goalShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].levelTarget.ToString();
                }
                else
                {
                    goal.text = goalShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].levelTarget.ToString();
                }
            }
        }
        else
        {
            currentLevel.text = currentLevelShadow.text = "Level " + FBPlayerData.instance.CURRENT_LEVEL.ToString();

            if (FBPlayerData.instance.CURRENT_LEVEL >= 26)
            {

                mainGoal.SetActive(false);
                mainGoal1.SetActive(true);
                bonusGoal1.SetActive(true);
                int levelIndex = FBPlayerData.instance.CURRENT_LEVEL-1;

                // Safety check to avoid out-of-bounds access
                levelIndex = Mathf.Clamp(levelIndex, 0, levelData.levels.Count - 1);

                var level = levelData.levels[levelIndex];

                goal1.text = goal1Shadow.text = level.levelTarget.ToString();
                targetForBonus.text = targetForBonusShadow.text = level.targetPointsForBonus.ToString();
                reward.text = rewardShadow.text = $"Reward:   +{level.reward}";
            }
            else
            {
                mainGoal.SetActive(true);
                mainGoal1.SetActive(false);
                bonusGoal1.SetActive(false);
                if (InitManager.instance.CurrentScene == "Levelup")
                {
                    goal.text = goalShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].levelTarget.ToString();
                }
                else
                {
                    goal.text = goalShadow.text = levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].levelTarget.ToString();
                }
            }


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
                if (InitManager.instance.CurrentScene == "Levelup")
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

    private void Start()
    {
        popupObj.anchoredPosition = new Vector2(0, -350f);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();
        bg.DOKill();
        popup.DOKill();
        popupObj.DOKill();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupObj.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

        playBtn.DOKill();
        playBtnRectTransform.DOKill();

        playBtn.DOFade(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.15f);
        playBtnRectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.15f);
        Invoke("AppearStars", 0.3f);

    }
    void AppearStars()
    {
        if(InitManager.instance.CurrentScene == "Levelup")
        {
            int earnedStars = GameManager.instance.earnedStarsInTheLevel;

            for (int i = 0; i < earnedStars && i < starsObj.Length; i++)
            {
                starsObj[i].SetActive(true);
            }
        }
        float delay = 0.1f;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].DOFade(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            // Scale Up
            starsRectTransform[i].DOScale(0.9f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            delay += delayIncrement; // Increase delay for next button
        }
    }
    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
            popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
            popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
        }
        else
        {
            if (HeartManager.instance.CanPlay())
            {
                bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
                popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
                popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
            }
            else
            {
                PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);
                PopupManager.instance.TogglePopup(PopupManager.instance.outOfHeartsPopup);
            }
        }
        
    }
    public void OnTapClose()
    {
        FBPlayerData.instance.VibrationEffect();
            
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            Invoke("BringStars", 0.3f);
            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(() => PopupManager.instance.ShowGoalPopup(PopupManager.instance.goalPopup));
        }
        else
            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(() => PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup));
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }
    void BringStars()
    {

        GameManager.instance.levelupStars.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }
    public void RemoveThis()
    {
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            Initiate.Fade("Game", Color.black, 1f);
        }
        else
            if (Menu.instance.overlayPanel.activeSelf)
            {
                //Initiate.Fade("Game", Color.black, 1f);
                SceneManager.LoadScene("Game");
            }
        PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);
    }
    public void StartGame()
    {
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            InitManager.instance.isReplay = true;
            Initiate.Fade("Game", Color.black, 1f);
        }
        else
        {
            InitManager.instance.receivedBonusCoins = false;
            Menu.instance.overlayPanel.SetActive(true);
            ClosePopup();
        }
        
    }
    private void OnDestroy()
    {
        bg?.DOKill();
        popup?.DOKill();
        popupObj?.DOKill();

        playBtn?.DOKill();
        playBtnRectTransform?.DOKill();

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i]?.DOKill();
            starsRectTransform[i]?.DOKill();
        }

    }
}

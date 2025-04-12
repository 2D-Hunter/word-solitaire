using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public LevelData levelData;

    public GameObject overlayPanel;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI currentLevelShadow;

    public GameObject fortuneWheel = null;
    public bool isFortuneWheelOpened = false;
    //public TextMeshProUGUI giftBoxCountText;

    //public ParticleSystem sparkle_purchasedPopup = null;

    public RectTransform settingBtn;
    public RectTransform heartHud;
    public RectTransform coinHud;

    public RectTransform buttonRect;

    private void Awake()
    {
        instance = this;
        if(InitManager.instance)
            currentLevel.text = currentLevelShadow.text = "Level "+levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].levelNumber.ToString();
        overlayPanel.SetActive(false);
        


    }
    private void Start()
    {
        PopupManager.instance.AssignUIContainer();
        //AnimateButton();
    }
    public void ShowGoalPopup()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);
        heartHud.SetAsLastSibling();

    }
    public void ShowFortuneWheel()
    {
        FBPlayerData.instance.VibrationEffect();
        fortuneWheel.SetActive(true);
    }

    public void JoinUs()
    {
        FBPlayerData.instance.VibrationEffect();
        Application.ExternalCall("JoinUs");
    }
    public void Invite()
    {
        FBPlayerData.instance.VibrationEffect();
        Application.ExternalCall("Invite");
    }
    public void Share()
    {
        FBPlayerData.instance.VibrationEffect();
        Application.ExternalCall("Share");
    }
    
    public void OpenSettingsPopupMenu()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupMenu);
    }
    public void OpenShop()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.ToggleShop();
    }
    public void TapOnHeartHud()
    {
        FBPlayerData.instance.VibrationEffect();
        if (HeartManager.instance.currentHearts >= HeartManager.instance.maxHearts)
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.heartsFullPopup);
        }
        else if(HeartManager.instance.currentHearts < HeartManager.instance.maxHearts && HeartManager.instance.currentHearts > 1)
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.moreHeartsPopup);
        }
        else if (HeartManager.instance.currentHearts <= 1)
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.moreHeartsPopup2);
        }

    }
    public void OpenDailyRewards()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.dailyRewardsPopup);
    }
    void AnimateButton()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(buttonRect.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad)) // Scale Up
                .Append(buttonRect.DOShakePosition(0.3f, 5f, 10, 90, false, true)) // Shake
                .Append(buttonRect.DOScale(1, 0.3f).SetEase(Ease.InOutQuad)) // Return to Original
                .SetLoops(-1); // Loop Forever
    }
}

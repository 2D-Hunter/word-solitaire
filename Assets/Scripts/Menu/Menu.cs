using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public LevelData levelData;

    public GameObject overlayPanel;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI currentLevelShadow;

    public GameObject fortuneWheel = null;
    public bool isFortuneWheelOpened = false;

    private void Awake()
    {
        instance = this;
        if(InitManager.instance)
            currentLevel.text = currentLevelShadow.text = "Level "+levelData.levels[InitManager.instance.currentLevel - 1].levelNumber.ToString();
        overlayPanel.SetActive(false);

        
    }
    private void Start()
    {
        PopupManager.instance.AssignUIContainer();
    }
    public void ShowGoalPopup()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);
    }
    public void ShowFortuneWheel()
    {
        fortuneWheel.SetActive(true);
    }

    public void JoinUs()
    {
        Application.ExternalCall("JoinUs");
    }
    public void Invite()
    {
        Application.ExternalCall("Invite");
    }
    public void Share()
    {
        Application.ExternalCall("Share");
    }
    
    public void OpenSettingsPopupMenu()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupMenu);
    }
    public void OpenShop()
    {
        PopupManager.instance.ToggleShop();
    }
    public void TapOnHeartHud()
    {
        if(HeartManager.instance.currentHearts >= 5)
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.heartsFullPopup);
        }
        
    }
}

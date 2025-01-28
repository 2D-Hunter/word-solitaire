using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    public LevelData levelData;

    public GameObject goalPopup;
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
    public void ShowGoalPopup()
    {
        goalPopup.SetActive(true);
        GoalPopup.instance.ShowPopup();
    }
    public void ShowFortuneWheel()
    {
        fortuneWheel.SetActive(true);
    }
}

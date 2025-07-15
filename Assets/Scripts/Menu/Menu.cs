using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Word;

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

    public TextMeshProUGUI msgTxt;
    public BackgroundManager backgroundManager;

    public GameObject[] allUI;
    public GameObject connectingToServer = null;


    private List<int> shuffledPool = new List<int>();
    private int poolIndex = 0;
    private const int startShuffleIndex = 25; // Level 41 (0-based)
    private const int endShuffleIndex = 50;   // Up to level 50 (exclusive index)
    int prefabIndex;

    private void Awake()
    {
        Debug.Log("___Menu FBPlayerData.instance.CURRENT_LEVEL: " + FBPlayerData.instance.CURRENT_LEVEL);
        if (FBPlayerData.instance.CURRENT_LEVEL > 50)
        {
            InitManager.instance.isLevelRandomized = true;

            prefabIndex = GetNextShuffledIndex() + 1; // +1 because pool is 0-based
            Debug.Log("____prefabIndex: " + prefabIndex);
            InitManager.instance.nextRandomLevel = prefabIndex;
        }

        backgroundManager.GetComponent<BackgroundManager>().OnLevelChanged(FBPlayerData.instance.CURRENT_LEVEL);
        backgroundManager.GetComponent<BackgroundManager>().UpdateNextLocationText(FBPlayerData.instance.CURRENT_LEVEL);

        InitManager.instance.CurrentScene = "Menu";
        instance = this;
        //if(InitManager.instance)
        //    currentLevel.text = currentLevelShadow.text = "Level "+levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 1].levelNumber.ToString();
        currentLevel.text = currentLevelShadow.text = "Level " + FBPlayerData.instance.CURRENT_LEVEL.ToString();
        overlayPanel.SetActive(false);
        


    }
    private void Start()
    {
        MultiplayerEventHandler.Instance.isMultiplayer = false;
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
    public void ShowMessage(string msg)
    {
        
        msgTxt.transform.SetAsLastSibling();
        msgTxt.text = msg;
        Invoke("RemoveMessage", 3f);
    }
    void RemoveMessage()
    {
        msgTxt.text = "";
    }
    public void StartMultiplayer()
    {
        foreach (GameObject item in allUI)
        {
            item.SetActive(false);
        }
        connectingToServer.SetActive(true);

        //string url = $"ws://ec2-52-43-3-186.us-west-2.compute.amazonaws.com:8770/word";
        //MultiplayerEventHandler.Instance.SubscribeMultiplayerEvents();
        //WordServiceContainer.NetworkService.Connect(url, () =>
        //{
        //    Debug.Log("Onconeect to server >>>>>>>");
        //});

        //Initiate.Fade("MultiplayerSelection", Color.black, 1f);
    }

    public LevelData.LevelInfo GetLevelInfo(int levelNumber)
    {
        int maxDefinedLevel = levelData.levels.Length;

        if (levelNumber <= maxDefinedLevel)
        {
            return levelData.levels[levelNumber - 1];
        }
        else
        {
            // Get next from shuffled pool
            int randomIndex = GetNextShuffledIndex();

            LevelData.LevelInfo original = levelData.levels[randomIndex];

            // Clone and override level number
            return new LevelData.LevelInfo
            {
                levelNumber = levelNumber,
                levelTarget = original.levelTarget,
                bonusGoalType = original.bonusGoalType,
                targetPointsForBonus = original.targetPointsForBonus,
                reward = original.reward,
                numberOfLetters = original.numberOfLetters,
                numberOfWords = original.numberOfWords,
                isLevelHard = original.isLevelHard
            };
        }
    }

    private int GetNextShuffledIndex()
    {
        // Reshuffle if first time or exhausted
        if (shuffledPool == null || poolIndex >= shuffledPool.Count)
        {
            ShufflePool();
        }

        return shuffledPool[poolIndex++];
    }

    private void ShufflePool()
    {
        shuffledPool = new List<int>();

        for (int i = startShuffleIndex; i < endShuffleIndex; i++)
        {
            shuffledPool.Add(i);
        }

        // Fisher–Yates Shuffle
        for (int i = shuffledPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffledPool[i], shuffledPool[j]) = (shuffledPool[j], shuffledPool[i]);
        }

        poolIndex = 0;
    }
}

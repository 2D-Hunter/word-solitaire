using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject[] levels;
    public LevelData levelData;

    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentLevelTextShadow;
    public TextMeshProUGUI currentLevelTarget;
    public TextMeshProUGUI currentLevelTargetShadow;
    public TextMeshProUGUI currentLevelScore;
    public TextMeshProUGUI currentLevelScoreShadow;

    private int currentLevelIndex = 0;

    void Awake()
    {
        instance = this;
        Debug.Log("InitManager.instance: "+ InitManager.instance);
        if (InitManager.instance == null)
        {
            LoadLevel(2);
        }
        else
        {
            LoadLevel(InitManager.instance.currentLevel - 1);
        }
        
    }

    public void LoadLevel(int levelIndex)
    {
        foreach (GameObject level in levels)
        {
            level.SetActive(false);
        }

        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            levels[levelIndex].SetActive(true);
            CardManager.instance.AddAllCardsToList();
            if (levelData != null && levelIndex < levelData.levels.Length)
            {
                var levelInfo = levelData.levels[levelIndex];
                UpdateLevelUI(levelIndex + 1, levelInfo.levelTarget);
                Debug.Log($"Loading Level: {levelInfo.levelNumber}");
                //InitManager.instance.currentTarget = levelInfo.levelTarget;
                if (InitManager.instance != null)
                    InitManager.instance.levelCompleted = false;
                InitManager.instance.currentTarget = levelInfo.levelTarget;
                Debug.Log("All Cards: "+ CardManager.instance.allCards.Count);
                CardManager.instance.totalCardToGet = CardManager.instance.allCards.Count - (CardManager.instance.extraCards.Count + CardManager.instance.rightSideCards.Count);
            }
            else
            {
                Debug.LogWarning("Level data is missing for this level!");
            }
        }
        else
        {
            Debug.LogError("Invalid level index!");
        }
        
    }
    public void UpdateLevelUI(int levelNumber, int levelTarget)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = $"Level: {levelNumber}";
            currentLevelTextShadow.text = $"Level: {levelNumber}";
        }

        if (currentLevelTarget != null)
        {
            currentLevelTarget.text = $"{levelTarget}";
            currentLevelTargetShadow.text = $"{levelTarget}";
        }
        if (currentLevelScore != null)
        {
            currentLevelScore.text = "0";
            currentLevelScoreShadow.text = "0";
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Length)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
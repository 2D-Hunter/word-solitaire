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

    public string levelNamePrefix = "Level-";
    public RectTransform levelsObj = null;

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
            if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
            {
                LoadLevel(FBPlayerData.instance.CURRENT_LEVEL-1);
            }
            else
            {
                LoadLevel(FBPlayerData.instance.CURRENT_LEVEL);
            }
        }
        
    }

    public void LoadLevel(int levelIndex)
    {
        if(FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            foreach (GameObject level in levels)
            {
                level.SetActive(false);
            }
        }
        else
        {
            Destroy(levels[0]);
            levels[0] = null;
            Destroy(levels[1]);
            levels[1] = null;
            //levels[0].SetActive(false);
            //levels[1].SetActive(false);
            string prefabName = levelNamePrefix + levelIndex;
            GameObject levelPrefab = Resources.Load<GameObject>("Levels/" + prefabName);
            levelPrefab.SetActive(true);
            Debug.Log("Level Prefab: " + levelPrefab);
            if (levelPrefab != null)
            {
                GameObject level = Instantiate(levelPrefab, levelsObj);
            }
            else
            {
                Debug.LogError("Level prefab not found: " + prefabName);
            }
        }

        Debug.Log("levelIndex: " + levelIndex + "_____"+ levels.Length);

        if(FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            levelIndex++;
        }
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            levels[levelIndex-1].SetActive(true);
            CardManager.instance.AddAllCardsToList();
            CardManager.instance.AddTotalCardsToClearInList();
            if (levelData != null && levelIndex < levelData.levels.Length)
            {
                var levelInfo = levelData.levels[levelIndex-1];
                UpdateLevelUI(levelIndex, levelInfo.levelTarget);
                Debug.Log($"Loading Level: {levelInfo.levelNumber}");
                //InitManager.instance.currentTarget = levelInfo.levelTarget;
                if (InitManager.instance != null)
                {
                    InitManager.instance.levelCompleted = false;
                    InitManager.instance.currentTarget = levelInfo.levelTarget;
                }
                    
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
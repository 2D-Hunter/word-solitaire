using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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

    public TextMeshProUGUI bonusTarget;
    public TextMeshProUGUI bonusTargetShadow;
    private int currentLevelIndex = 0;
    public string levelNamePrefix = "Level-";
    public RectTransform levelsObj = null;





    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
      
        Debug.Log("InitManager.instance: " + InitManager.instance);
        if (InitManager.instance == null)
        {
            LoadLevel(2);
        }
        else
        {
            if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
            {
                Debug.Log("Level Manager FBPlayerData.instance.CURRENT_LEVEL: " + FBPlayerData.instance.CURRENT_LEVEL);
                LoadLevel(FBPlayerData.instance.CURRENT_LEVEL - 1);
            }
            else
            {
                if(InitManager.instance.isReplay)
                    LoadLevel(FBPlayerData.instance.CURRENT_LEVEL-1);
                else
                    LoadLevel(FBPlayerData.instance.CURRENT_LEVEL);
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
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

            // Determine which prefab to load
            int prefabIndex = levelIndex;

            // After level 50, randomly select from levels 25–50 using the shuffled pool
           /* if (levelIndex > 50)
            {
                prefabIndex = InitManager.instance.nextRandomLevel + 1; // +1 because pool is 0-based
                Debug.Log("____prefabIndex: " + prefabIndex);
            }*/

            //BoardManager.instance.GenerateLevelByNumber(prefabIndex-1);
            /*string prefabName = levelNamePrefix + prefabIndex;
            Debug.Log("prefabName: " + prefabName);
            GameObject levelPrefab = Resources.Load<GameObject>("Levels/" + prefabName);

            if (levelPrefab != null)
            {
                GameObject level = Instantiate(levelPrefab, levelsObj);
                level.SetActive(true);
            }
            else
            {
                Debug.LogError("Level prefab not found: " + prefabName);
            }*/
        }

        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            levelIndex++;
        }

        if (levelIndex >= 0)
        {
            // Since you're not using the 'levels' array anymore for instantiated levels after level 2,
            // You can remove this part or manage instantiated levels separately.
            levels[levelIndex - 1].SetActive(true);
            CardManager.instance.AddAllCardsToList();
            CardManager.instance.AddTotalCardsToClearInList();

            if (levelData != null && levelIndex < 1000) // Arbitrary upper limit
            {
                LevelData.LevelInfo levelInfo = GetLevelInfo(levelIndex); // Uses shuffled logic

                if (FBPlayerData.instance.CURRENT_LEVEL >= 26)
                    UpdateLevelUI(levelIndex, levelInfo.levelTarget);
                else
                    UpdateLevelUI(levelIndex, levelInfo.levelTarget, levelInfo.targetPointsForBonus);

                if (InitManager.instance != null)
                {
                    InitManager.instance.levelCompleted = false;
                    InitManager.instance.currentTarget = levelInfo.levelTarget;
                }

                Debug.Log("All Cards: " + CardManager.instance.allCards.Count);

                CardManager.instance.totalCardToGet = CardManager.instance.allCards.Count
                    - (CardManager.instance.extraCards.Count + CardManager.instance.rightSideCards.Count);
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
    public void UpdateLevelUI(int levelNumber, int levelTarget, int bt = -1)
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

        // Only update bonusTarget if bt was actually passed in
        if (bt >= 0 && bonusTarget != null)
        {
            bonusTarget.text = $"{bt}";
            bonusTargetShadow.text = $"{bt}";
        }
    }
    public void UpdateCurrentTarget( int levelTarget, int bt = -1)
    {
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

        // Only update bonusTarget if bt was actually passed in
        if (bt >= 0 && bonusTarget != null)
        {
            bonusTarget.text = $"{bt}";
            bonusTargetShadow.text = $"{bt}";
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
            int randomIndex = InitManager.instance.nextRandomLevel;

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
}
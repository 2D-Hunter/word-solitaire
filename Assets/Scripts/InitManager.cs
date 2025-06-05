using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager:MonoBehaviour
{
    public static InitManager instance;

    public string version = "1.0.12";
    public int currentLevel = 3;

    
    public int currentTarget = 5;
    public bool levelCompleted = false;

    public LevelData levelData;
    public bool backFromLevelCompletion = false;
    public int brillianceScore = 0;
    public bool deleteData = false;
    public bool feedbackSubmitted = false;
    public bool startShopping = false;
    public string currentReward = "";
    public int tutorialCntr = 0;
    public int buyMoreCardsCntr = 1;
    public int moreCardsPrice = 150;
    public List<char> letters = new List<char> { 'A', 'C', 'P', 'T', 'O', 'S', 'A', 'S', 'R', 'J' };
    public List<char> letters1 = new List<char> { 'A', 'C', 'P', 'T', 'O', 'S', 'A', 'S', 'R', 'J' };
    public string CurrentScene = "Splash";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        var levelInfo = levelData.levels[currentLevel - 1];
        Debug.Log("___Level Info: " + levelInfo.levelNumber + "_____Target: "+levelInfo.levelTarget);
    }
    
}

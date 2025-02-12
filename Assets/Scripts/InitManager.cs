using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager:MonoBehaviour
{
    public static InitManager instance;
    
    public int currentLevel = 3;

    
    public int currentTarget = 5;
    public bool levelCompleted = false;

    public LevelData levelData;
    public bool backFromLevelCompletion = false;
    public int brillianceScore = 0;

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

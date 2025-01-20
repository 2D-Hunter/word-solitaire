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


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

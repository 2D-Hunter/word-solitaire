using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager:MonoBehaviour
{
    public static InitManager instance;

    public string version = "1.0.0";
    public int currentLevel = 1;
    public int nextRandomLevel;
    public bool isLevelRandomized = false;



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
    public bool isReplay = false;
    public bool receivedBonusCoins = false;
    public GameObject coinAnimPrefab;
    private GameObject coinAnimInstance;

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
    public void ShowCoinAnim()
    {
        Invoke("PlayCoinSound", 0.5f);
        
        coinAnimInstance = Instantiate(coinAnimPrefab, transform);

    }
    void PlayCoinSound()
    {
        SoundManager.instance.PlaySFX("GetCoins", 0.8f);
    }
    public IEnumerator RemoveCoinAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(coinAnimInstance);
        coinAnimInstance = null;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class Levelup : MonoBehaviour
{

    

    public CanvasGroup bg = null;
    public GameObject title = null;
    public GameObject brillance = null;
    public GameObject replayBtn = null;
    public GameObject continueBtn = null;
    public GameObject dictionaryBtn = null;

    public CanvasGroup titleCG;
    public CanvasGroup replayBtnCG;
    public CanvasGroup continueBtnCG;
    public CanvasGroup dictionaryBtnCG;

    public TextMeshProUGUI titleTxt_LevelNumber;
    public TextMeshProUGUI titleTxtShadow_LevelNumber;
    public TextMeshProUGUI titleTxt_TotalScore;
    public CanvasGroup brillanceCG;

    public TextMeshProUGUI bestWordTxt;
    public TextMeshProUGUI pointsTxt;
    public Text punchlineTxt;
    public float typingSpeed = 0.05f;

    public CanvasGroup bestWordTxtTitle;
    public CanvasGroup bestWordTxtCG;
    public CanvasGroup pointsTxtCG;
    public LevelData levelData;

    public RectTransform bonusGoalLevelup = null;
    public GameObject bonusGoalHud = null;
    public GameObject coinHud = null;

    private float finalPos;

    private void Awake()
    {
        Debug.Log("Levelup Awake");
    }
    private void Start()
    {
        AnalyticsManager.Instance.TrackLevelComplete(FBPlayerData.instance.CURRENT_LEVEL, GameManager.instance.earnedStarsInTheLevel, GameManager.instance.totalPoint);
        AnalyticsManager.Instance.TrackBestWord(FBPlayerData.instance.CURRENT_LEVEL, SlotManager.instance.bestWord, SlotManager.instance.bestScore, GameManager.instance.totalPoint);
        AnalyticsManager.Instance.TrackAllWordsCreated(GameManager.instance.foundWords);
        Debug.Log("Levelup Start");
        Debug.Log("Best Word: " + SlotManager.instance.bestWord);
        Debug.Log("Score of Best Word: " + SlotManager.instance.bestScore);
        bestWordTxtTitle.alpha = 0;
        bestWordTxtCG.alpha = 0;
        pointsTxtCG.alpha = 0;

        if ((FBPlayerData.instance.CURRENT_LEVEL - 1) <= 25)
        {
            coinHud.SetActive(false);
            bonusGoalHud.SetActive(false);
            finalPos = 225f;
        }
        else
        {
            coinHud.SetActive(true);
            bonusGoalHud.GetComponent<RectTransform>().localScale = Vector3.zero;
            bonusGoalHud.SetActive(true);
            
            finalPos = 70f;
        }
        SetInit();


        //popupRectTransform.anchoredPosition = new Vector2(0, -350f);
        // Instead of calling PreloadBackgroundForUpcomingLevel directly:
        BackgroundManager.instance.MaybePreloadNextBackground();
    }
    public void ShowPunchline()
    {
        AnimateTotalScore(GameManager.instance.totalPoint);
        int rnd = Random.Range(1, 26);
        string message = $"Only {rnd}% players made a better word!";
        StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string fullText)
    {
        punchlineTxt.text = ""; // clear first

        foreach (char c in fullText)
        {
            punchlineTxt.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    private void OnEnable()
    {
        Debug.Log("Levelup OnEnable");
        
    }
    private void SetInit()
    {
        InitManager.instance.CurrentScene = "Levelup";
        titleTxt_LevelNumber.text = titleTxtShadow_LevelNumber.text = "Level "+(FBPlayerData.instance.CURRENT_LEVEL-1).ToString();
        titleTxt_TotalScore.text = "Score 0";
        bestWordTxt.text = SlotManager.instance.bestWord;
        pointsTxt.text = SlotManager.instance.bestScore.ToString()+" pts";
        Debug.Log("Levelup SetInit: "+ FBPlayerData.instance.TOTAL_COINS);
        Debug.Log("Levelup SetInit: "+ levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].reward);

        
        

        //GameManager.instance.overlayPanel.SetActive(false);
        bg.alpha = 0;
        titleCG.alpha = 0;
        replayBtnCG.alpha = 0;
        continueBtnCG.alpha = 0;
        dictionaryBtnCG.alpha = 0;
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(title.GetComponent<RectTransform>().anchoredPosition.x, 1000);//815
        bonusGoalLevelup.anchoredPosition = new Vector2(0, -1000);//-815
        //continueBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(continueBtn.GetComponent<RectTransform>().anchoredPosition.x, -1000);//-815
        //dictionaryBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(dictionaryBtn.GetComponent<RectTransform>().anchoredPosition.x, -1000);//-815
        brillance.GetComponent<RectTransform>().localScale = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < GameManager.instance.allGameStuffs.Length; i++)
        {
            // Ensure starting conditions
            GameManager.instance.allGameStuffs[i].alpha = 1;
        }
        ShowPopup();
        
    }
    
    public void ShowPopup()
    {
        for (int i = 0; i < GameManager.instance.allGameStuffs.Length; i++)
        {
            GameManager.instance.allGameStuffs[i].DOFade(0f, 0.6f)
                .SetEase(Ease.OutExpo);
        }

        bg.DOFade(0.6f, 0.7f).SetEase(Ease.OutExpo);
        brillance.GetComponent<RectTransform>().DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack).OnComplete(AnimateOthers).SetDelay(0.7f);
    }
    void AnimateOthers()
    {
        GameManager.instance.levelupStars.SetActive(true);
        titleCG.DOFade(1f, 0.3f).SetEase(Ease.OutExpo);
        replayBtnCG.DOFade(1f, 0.3f).SetEase(Ease.OutExpo);
        continueBtnCG.DOFade(1f, 0.3f).SetEase(Ease.OutExpo);
        dictionaryBtnCG.DOFade(1f, 0.3f).SetEase(Ease.OutExpo);

        title.GetComponent<RectTransform>().DOAnchorPosY(815f, 0.3f).SetEase(Ease.OutExpo);
        bonusGoalLevelup.DOAnchorPosY(finalPos, 0.3f).SetEase(Ease.OutExpo);
        if(bonusGoalHud.activeSelf)
        {
            bonusGoalHud.GetComponent<RectTransform>().DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(CheckBonusCoins);
        }
        
        //replayBtnCG.GetComponent<RectTransform>().DOAnchorPosY(-815f, 0.3f).SetEase(Ease.OutExpo);
        //continueBtnCG.GetComponent<RectTransform>().DOAnchorPosY(-815f, 0.3f).SetEase(Ease.OutExpo);
        //dictionaryBtnCG.GetComponent<RectTransform>().DOAnchorPosY(-815f, 0.3f).SetEase(Ease.OutExpo);

        bestWordTxtTitle.DOFade(1f, 0.5f).SetEase(Ease.OutExpo).SetDelay(0.3f);
        bestWordTxtCG.DOFade(1f, 0.5f).SetEase(Ease.OutExpo).SetDelay(0.5f);
        pointsTxtCG.DOFade(1f, 0.5f).SetEase(Ease.OutExpo).SetDelay(0.7f).OnComplete(ShowPunchline);
    }
    void CheckBonusCoins()
    {
        if (levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].reward != 0 && !InitManager.instance.receivedBonusCoins)
        {
            InitManager.instance.isReplay = false;
            
            Debug.Log("FindObjectOfType<BonusHud>().bonusTargetAchieved: " + FindObjectOfType<BonusHud>().bonusTargetAchieved);
            if (FindObjectOfType<BonusHud>().bonusTargetAchieved)
            {
                InitManager.instance.receivedBonusCoins = true;
                CoinManager.instance.AddCoins(levelData.levels[FBPlayerData.instance.CURRENT_LEVEL - 2].reward);
                FBPlayerData.instance.ShowCoinAnimation();
            }
            
        }
        
    }
    public void ClosePopup()
    {
        //foreach (var button in buttons)
        //{
        //    button.enabled = false;
        //}
        //bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        //popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        //popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }
    void RemoveThis()
    {
        if (GameManager.instance.overlayPanel.activeSelf)
        {
            //Initiate.Fade("Menu", Color.black, 1f);
            HeartManager.instance.LoseHeart();
            SceneManager.LoadScene("Menu");
        }
        gameObject.SetActive(false);
    }
    public void Quit()
    {
        GameManager.instance.overlayPanel.SetActive(true);
        ClosePopup();

    }
    public void OpenDictionary()
    {
        FBPlayerData.instance.VibrationEffect();
        //titleCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        //brillanceCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        //GameManager.instance.levelupStarsCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        //replayBtnCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        //continueBtnCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        //dictionaryBtnCG.DOFade(0f, 0.3f).SetEase(Ease.OutExpo);
        Invoke("RemoveStars", 0.05f);
        PopupManager.instance.ShowDictionary(PopupManager.instance.dictionaryPopup);
    }
    void RemoveStars()
    {
        GameManager.instance.levelupStars.transform.localScale = Vector3.zero;
    }
    void BringStars()
    {
        GameManager.instance.levelupStars.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f) ;
    }
    public void ShowAd(string adPlace)
    {
        FBPlayerData.instance.VibrationEffect();
        AdTimerHandler.Instance.TryShowAd(adPlace);
    }
    public void ContinueGameAfterInterstitial()
    {
        InitManager.instance.isReplay = false;
        StartCoroutine(LoadMenu());
    }
    //public void TapOnContinue()
    //{
        
    //    InitManager.instance.isReplay = false;
    //    StartCoroutine(LoadMenu());
    //}
    IEnumerator LoadMenu()
    {
        continueBtn.GetComponent<Button>().enabled = false;
        yield return new WaitForSeconds(0f);
        if (FBPlayerData.instance.CURRENT_LEVEL == 2)
        {
            FBPlayerData.instance.TUTORIAL_2_COMPLETED = true;
        }
        //FBPlayerData.instance.CURRENT_LEVEL++;
        FBPlayerData.instance.SavePlayerData();
        //if (FBPlayerData.instance.CURRENT_LEVEL > 5)
        //{
        //    FBPlayerData.instance.CURRENT_LEVEL = 1;
        //}

        Debug.Log("FBPlayerData.instance.CURRENT_LEVEL: " + FBPlayerData.instance.CURRENT_LEVEL);
        Initiate.Fade("Menu", Color.black, 1f);

    }
    public void ReplayGameAfterInterstitial()
    {
        FBPlayerData.instance.VibrationEffect();
        Invoke("RemoveStars", 0.05f);
        PopupManager.instance.ShowGoalPopup(PopupManager.instance.goalPopup);
    }
    //public void TapOnReplay()
    //{
    //    FBPlayerData.instance.VibrationEffect();
        
    //}
    public void AnimateTotalScore(int targetScore)
    {
        int current = 0;
        DOTween.To(() => current, x => {
            current = x;
            titleTxt_TotalScore.text = "Score "+ current.ToString();
        }, targetScore, 1f)
        .SetEase(Ease.OutCubic);
    }

}

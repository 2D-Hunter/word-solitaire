using UnityEngine;
using SimpleJSON;
using System;

public class FBPlayerData : MonoBehaviour
{
    public static FBPlayerData instance;
    public string productID = "";

    
    public string BUILD_TYPE = "Facebook"; //Unity, Facebook

    [HideInInspector]
    public bool IsIos = false;

   

    [HideInInspector]
    public bool IsTesting = false;

    [HideInInspector]
    public string PlayerName;

    [HideInInspector]
    public string DeviceType;

    public Splash splash;





    [HideInInspector]
    public int CURRENT_LEVEL = 1;

    [HideInInspector]
    public bool GAME_MUSIC = true;

    [HideInInspector]
    public bool GAME_SOUND = true;

    [HideInInspector]
    public bool NO_ADS_30_DAYS = false;

    [HideInInspector]
    public string EXPIRY_DATE_30_DAYS = "";

    [HideInInspector]
    public int TOTAL_COINS = 1000;

    [HideInInspector]
    public int BRILLIANCE = 0;

    [HideInInspector]
    public int TOTAL_HEARTS = 5;

    [HideInInspector]
    public string LAST_HEART_TIME = "";

    public string LAST_REWARD_TIME = "";
    public string LAST_AD_REWARD_TIME = "";
    public int AVAILABLE_REWARDS = 5;
    public int CURRENT_REWARD_INDEX = 0;
    public bool TUTORIAL_1_COMPLETED = false;
    public bool TUTORIAL_2_COMPLETED = false;


    private void Awake()
    {
        //BUILD_TYPE = "Facebook";
        //TOTAL_COINS = 1000;
        //GAME_SOUND = false;
        //NO_ADS_30_DAYS = false;
        //IsTesting = false;
        
        //CURRENT_LEVEL = 1;
        //TUTORIAL_1_COMPLETED = true;
        Debug.Log("___________Awake BUILD_TYPE: " + BUILD_TYPE);
        Debug.Log("___________is ad removed: " + NO_ADS_30_DAYS);
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       FBPlayerData.instance.NO_ADS_30_DAYS = !ShouldShowAds();
    }

    public void SavePlayerData()
    {
        Debug.Log("Data Saved");
        string data = CURRENT_LEVEL + ":" + GAME_MUSIC + ":" + GAME_SOUND + ":" + NO_ADS_30_DAYS + ":" + TOTAL_COINS + ":" + BRILLIANCE + ":" + TOTAL_HEARTS + ":" + LAST_HEART_TIME
             + ":" + LAST_REWARD_TIME + ":" + LAST_AD_REWARD_TIME + ":" + AVAILABLE_REWARDS + ":" + CURRENT_REWARD_INDEX + ":" + TUTORIAL_1_COMPLETED + ":" + TUTORIAL_2_COMPLETED;
        Application.ExternalCall("SavePlayerData", data);

    }
    public void GetPlayerData(string data)
    {
        Debug.Log("_______________ RecievePlayerData" + data);
        JSONNode playerData;
        playerData = JSON.Parse(data);

        CURRENT_LEVEL = playerData["CURRENT_LEVEL_001"];
        GAME_MUSIC = playerData["GAME_MUSIC_001"];
        GAME_SOUND = playerData["GAME_SOUND_001"];
        NO_ADS_30_DAYS = playerData["NO_ADS_30_DAYS_001"];
        TOTAL_COINS = playerData["TOTAL_COINS_001"];
        BRILLIANCE = playerData["BRILLIANCE_001"];
        TOTAL_HEARTS = playerData["TOTAL_HEARTS_001"];
        LAST_HEART_TIME = playerData["LAST_HEART_TIME_001"];
        LAST_REWARD_TIME = playerData["LAST_REWARD_TIME_001"];
        LAST_AD_REWARD_TIME = playerData["LAST_AD_REWARD_TIME_001"];
        AVAILABLE_REWARDS = playerData["AVAILABLE_REWARDS_001"];
        CURRENT_REWARD_INDEX = playerData["CURRENT_REWARD_INDEX_001"];
        TUTORIAL_1_COMPLETED = playerData["TUTORIAL_1_COMPLETED_001"];
        TUTORIAL_2_COMPLETED = playerData["TUTORIAL_2_COMPLETED_001"];

        Debug.Log(GAME_SOUND + "_____" + NO_ADS_30_DAYS + "_____" + TOTAL_COINS + " __________ RecievePlayerData22");

        if (splash == null)
            splash = FindObjectOfType<Splash>();
        splash.LoadScene();

    }

    public void ResetAllData()
    {
        Application.ExternalCall("ClearFBData");

    }
    public void GetMyName(string myName)
    {
        Debug.Log(" __________ PlayerName:" + myName);
        PlayerName = myName;
    }

   

    public void CheckIOS(string osType)
    {
        Debug.Log("________OS: " + osType);
        if (osType == "IOS")
        {
            IsIos = true;
        }
        else
        {
            IsIos = false;
        }
    }
    public void MuteSound(bool hasFocus)
    {

        //if (hasFocus)
        //{
        //    Debug.Log("_____Mute Music()");
        //    SpotTheDifferencesGameToolkit.Scripts.Audio.MusicBase.instance.SetMusicMute();
        //}
        //else
        //{

        //    if (GAME_MUSIC)
        //    {
        //        Debug.Log("_____UnMute Music(): ");
        //        SpotTheDifferencesGameToolkit.Scripts.Audio.MusicBase.instance.SetMusicUnMute();
        //    }
        //}
    }
    public void UnMuteSound()
    {
        //Debug.Log("_____UnMute Music()");
        //if (GAME_MUSIC)
        //    SpotTheDifferencesGameToolkit.Scripts.Audio.MusicBase.instance.SetMusicUnMute();
    }
    public void MobileOrDesktop(string type)
    {
        Debug.Log("Device Type: " + type);
        DeviceType = type;
    }
    public void GetProductsAfterPurchase(string productId)
    {
        Debug.Log("Get Product After Purchase: " + productId);
        productID = productId;
        PopupManager.instance.TogglePopup(PopupManager.instance.loading);
        PopupManager.instance.ToggleShop();
        switch (productId)
        {
            case "no_ads_30_days":
            case "coins_2000":
            case "coins_6000":
            case "coins_16000":
            case "coins_34000":
            case "coins_70400":
                PopupManager.instance.TogglePopup(PopupManager.instance.purchasedItemPopup);
                break;
        }
        SavePlayerData();
    }

    public void GetRewardAfterVideoAd(string index)
    {
        Debug.Log("___CollectAdReward_AfterRewardAd: " + index);
        switch (InitManager.instance.currentReward)
        {
            case "100_coins":
            case "150_coins":
            case "250_coins":
            case "1_wild_card":
                FindObjectOfType<DailyRewards>().CollectAdReward_AfterRewardAd(int.Parse(index));
                break;
        }
    }
    public void RewardAdNotAvailable()
    {

    }
    public void VibrationEffect()
    {
        Application.ExternalCall("VibrateDevice", "10");
    }

    public void ContinueGameAfterInterstitial(string screen)
    {
        Debug.Log("___Screen: " + screen);
        switch (screen)
        {
            case "Win":
                
                break;
        }
    }
    public void PurchaseSucceeded_RemoveAds()
    {
        

    }

    public void OnApplicationFocus(bool hasFocus)
    {
        
        Debug.Log("OnApplicationFocus");
        MuteSound(!hasFocus);

    }

    void OnApplicationPause()
    {
        Debug.Log("OnApplicationPause");
        //MuteSound();
    }

    public void ShowAdsNotAvailable()
    {

        PopupManager.instance.TogglePopup(PopupManager.instance.dailyRewardsPopup);
        PopupManager.instance.TogglePopup(PopupManager.instance.noAdAvailable);
    }

    public bool ShouldShowAds()
    {
        if (PlayerPrefs.HasKey("No_Ads_30_Days"))
        {
            DateTime expiryDate = DateTime.Parse(PlayerPrefs.GetString("No_Ads_30_Days"));
            Debug.Log("ShouldShowAds: " + DateTime.UtcNow);
            Debug.Log("ShouldShowAds: " + expiryDate);
            if (DateTime.UtcNow < expiryDate)
            {
                Debug.Log("Ads are disabled until: " + expiryDate);
                return false; // Don't show ads
            }
        }
        return true; // Show ads
    }
    public void ShowPurchaseFailed()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.loading);
        PopupManager.instance.ToggleMessage(PopupManager.instance.message);
    }
}

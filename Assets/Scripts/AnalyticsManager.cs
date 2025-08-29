using System;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using System.Linq;

public class AnalyticsManager : MonoBehaviour, IGameAnalyticsATTListener
{
    public static AnalyticsManager Instance;
    private bool isAnalyticsReady = false;

    private float sessionStartTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameAnalytics.Initialize();
            isAnalyticsReady = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsAnalyticsEnabled => isAnalyticsReady;

    public void TrackSessionStart()
    {
        if (!IsAnalyticsEnabled) return;

        sessionStartTime = Time.realtimeSinceStartup;
        GameAnalytics.NewDesignEvent("session:start");
        Debug.Log("Tracked: session_start");
    }

    public void TrackSessionEnd()
    {
        if (!IsAnalyticsEnabled) return;

        float duration = Time.realtimeSinceStartup - sessionStartTime;
        GameAnalytics.NewDesignEvent("session:end", duration);
        Debug.Log("Tracked: session_end");
    }

    public void TrackLevelStart(int level)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"level_{level}");
        Debug.Log("Tracked: level_start");
    }

    public void TrackLevelComplete(int level, int stars, int score)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"level_{level}", score);
        GameAnalytics.NewDesignEvent($"level:{level}:stars", stars);
        Debug.Log("Tracked: level_complete");
    }

    //public void TrackPurchase(string productId, string price)
    //{
    //    if (!IsAnalyticsEnabled) return;

    //    if (float.TryParse(price.Replace("$", ""), out float numericPrice))
    //    {
    //        GameAnalytics.NewDesignEvent($"purchase:{productId}", numericPrice);
    //        Debug.Log("Tracked: purchase");
    //    }
    //}

    public void TrackPurchase(string productId, string localizedPrice, string currencyCode)
    {
        if (!IsAnalyticsEnabled) return;

        // Remove any non-numeric and non-decimal characters (e.g., ₹, $, €)
        string numericPriceStr = new string(localizedPrice.Where(c => char.IsDigit(c) || c == '.').ToArray());

        if (decimal.TryParse(numericPriceStr, out decimal price))
        {
            int amountInCents = (int)(price * 100m); // Convert $1.99 → 199

            GameAnalytics.NewBusinessEvent(
                currency: currencyCode,
                amount: amountInCents,
                itemType: "IAP",
                itemId: productId,
                cartType: "default"
            );

            Debug.Log($"✅ Tracked business purchase: {productId}, {currencyCode} {amountInCents / 100.0m}");
        }
        else
        {
            Debug.LogWarning($"❗ Couldn't parse price: {localizedPrice}");
        }
    }

    public void TrackWordCreated(string word, int level, int score)
    {
        if (!IsAnalyticsEnabled) return;
        GameAnalytics.NewDesignEvent($"word:created:{word}:level_{level}", score);
        //GameAnalytics.NewDesignEvent($"word:created:{word}", score);
        Debug.Log($"Tracked: word_created {word}");
    }

    public void TrackAllWordsCreated(List<string> words)
    {
        if (!IsAnalyticsEnabled) return;

        foreach (var word in words)
        {
            TrackWordCreated(word, FBPlayerData.instance.CURRENT_LEVEL, 0);
        }
        Debug.Log("Tracked all words_created");
    }

    public void TrackHintUsed(int level, string hintWord)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"hint:used:{hintWord}:level_{level}");
        Debug.Log("Tracked: hint_used");
    }

    public void TrackDictionaryOpened(string word, int level)
    {
        if (!IsAnalyticsEnabled) return;

        // eventId: dictionary:opened:<word>:level_<level>
        GameAnalytics.NewDesignEvent($"dictionary:opened:{word}:level_{level}");

        Debug.Log($"Tracked: dictionary_opened | word={word} | level={level}");
    }

    public void TrackCoinsSpent(string reason, int amount)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "coins", amount, reason, "none");
        Debug.Log("Tracked: coins_spent");
    }

    public void TrackAdShown(string adType, string placement)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"ad:shown:{adType}:{placement}");
        Debug.Log("Tracked: ad_shown");
    }
    //public void TrackAdShown(string adType, string placement)
    //{
    //    if (!IsAnalyticsEnabled) return;

    //    GAAdType adTypeEnum = ConvertToAdType(adType);
    //    GameAnalytics.NewAdEvent(GAAdAction.Show, adTypeEnum, "Facebook", placement);

    //    Debug.Log($"Tracked: ad_shown via GAAdEvent ({adType}, {placement})");
    //}

    public void TrackAdCompleted(string adType, string placement)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"ad:completed:{adType}:{placement}");
        Debug.Log("Tracked: ad_completed");
    }
    //public void TrackAdCompleted(string adType, string placement)
    //{
    //    if (!IsAnalyticsEnabled) return;

    //    GAAdType adTypeEnum = ConvertToAdType(adType);
    //    GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, adTypeEnum, "Facebook", placement);

    //    Debug.Log($"Tracked: ad_completed via GAAdEvent ({adType}, {placement})");
    //}

    public void TrackAdRewardGranted(string rewardType, int amount)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, rewardType.ToLower(), amount, "ad", "reward");
        Debug.Log("Tracked: ad_reward");
    }

    public void TrackBoosterUsed(string boosterType)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"booster:used:{boosterType}");
        Debug.Log("Tracked: booster_used");
    }

    public void TrackFreeReward(string rewardType, int amount)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, rewardType.ToLower(), amount, "free", "reward");
        Debug.Log("Tracked: free_reward");
    }

    public void TrackBestWord(int level, string word, int bestWordScore, int totalScore)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"best_word:level_{level}:{word}", bestWordScore);
        Debug.Log("Tracked: best_word");
    }

    public void TrackPopupOpened(string popupName)
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent($"popup:opened:{popupName}");
        Debug.Log("Tracked: popup_opened");
    }

    public void TestAnalytics()
    {
        if (!IsAnalyticsEnabled) return;

        GameAnalytics.NewDesignEvent("debug:test");
        Debug.Log("✅ Tracked: debug_test");
    }

    // iOS ATT compliance callbacks (no-op unless needed)
    public void GameAnalyticsATTListenerNotDetermined() { }
    public void GameAnalyticsATTListenerRestricted() { }
    public void GameAnalyticsATTListenerDenied() { }
    public void GameAnalyticsATTListenerAuthorized() { }

    private GAAdType ConvertToAdType(string type)
    {
        switch (type.ToLower())
        {
            case "rewarded":
                return GAAdType.RewardedVideo;
            case "interstitial":
                return GAAdType.Interstitial;
            case "banner":
                return GAAdType.Banner;
            default:
                return GAAdType.Undefined;
        }
    }
}
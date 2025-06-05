using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class DailyRewards : MonoBehaviour
{
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    //float delay = 0f;
    float delayIncrement = 0.15f; // Adjust delay between each button if needed

    public GameObject collectButton;      // Button for first reward
    public GameObject[] adRewardButtons; // Buttons for Ad Rewards (2nd to 5th)
    public GameObject[] lockIcons;       // Lock icons for 3rd, 4th, 5th rewards
    public GameObject[] tickMarks;       // Tick marks for completed rewards
    public GameObject oneHourTimer = null;
    public TextMeshProUGUI firstRewardTimerText;
    public TextMeshProUGUI adRewardTimerText; // Shows the next available ad reward timer
    public GameObject twelveHourTimer = null;

    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, 150f);
        ShowPopup();
    }
    private void SetInit()
    {
        bg.alpha = 0;
        popup.alpha = 0;
        foreach (var btn in btns)
        {
            btn.alpha = 0;
        }
        foreach (var btnRectTransform in btnsRectTransform)
        {
            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        popupRectTransform.anchoredPosition = new Vector2(0, 150);
    }

    void SetPopupHeight()
    {
        if (!twelveHourTimer.activeSelf)
        {
            popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 1320);
        }
        else
        {
            popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 1500);
        }
    }

    public void ShowPopup()
    {
        SetInit();
        bg.DOKill();
        popup.DOKill();
        popupRectTransform.DOKill();
        UpdateUI();
        if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME) && FBPlayerData.instance.LAST_AD_REWARD_TIME != "0")
        {
            if (!DailyRewardsManager.instance.IsAdRewardInCooldown())
            {
                OnTwelveHourTimerComplete();
            }
        }


        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupRectTransform.DOAnchorPosY(500, 0.4f).SetEase(Ease.OutBack);

        float delay = 0.05f; // Initial delay

        for (int i = 0; i < btns.Length; i++)
        {
            // Ensure starting conditions
            btns[i].alpha = 0;
            btnsRectTransform[i].localScale = Vector3.one * 0.6f;

            // Fade In
            btns[i].DOFade(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            // Scale Up
            btnsRectTransform[i].DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            delay += delayIncrement; // Increase delay for next button
        }
    }

    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }

    void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
    }

    void Update()
    {
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if(GameUtils.IsFacebookBuild())
        {
            UpdateTimer(FBPlayerData.instance.LAST_REWARD_TIME, DailyRewardsManager.instance.firstRewardCooldown, firstRewardTimerText, collectButton, true);
            UpdateTimer(FBPlayerData.instance.LAST_AD_REWARD_TIME, DailyRewardsManager.instance.adRewardCooldown, adRewardTimerText, null, false);
        }
        else
        {
            UpdateTimer(DailyRewardsManager.instance.firstRewardKey, DailyRewardsManager.instance.firstRewardCooldown, firstRewardTimerText, collectButton, true);
            UpdateTimer(DailyRewardsManager.instance.adRewardKey, DailyRewardsManager.instance.adRewardCooldown, adRewardTimerText, null, false);
        }
        
    }

    void UpdateTimer(string key, int cooldown, TextMeshProUGUI timerText, GameObject button, bool isFirstReward)
    {
        //Debug.Log("DailyRewardsManager.instance.CanCollectReward: " + DailyRewardsManager.instance.CanCollectReward(key, cooldown));
        if (!DailyRewardsManager.instance.CanCollectReward(key, cooldown))
        {
            //Debug.Log("UpdateTimer remainingTime: " + key+"______"+cooldown);
            //float remainingTime = DailyRewardsManager.instance.GetRemainingTime(key, cooldown);
            //Debug.Log("UpdateTimer remainingTime: " + remainingTime);
            int remainingTime = Mathf.FloorToInt(DailyRewardsManager.instance.GetRemainingTime(key, cooldown));
            //Debug.Log("UpdateTimer remainingSeconds: " + remainingTime);
            if (remainingTime <= 0)
            {
                // Ensure UI updates at 00:00
                remainingTime = 0;

                // Timer has finished
                //Debug.Log("isFirstReward: "+ isFirstReward);
                if (isFirstReward)
                {
                    FBPlayerData.instance.LAST_REWARD_TIME = "";
                    FBPlayerData.instance.SavePlayerData();
                    // First reward: Show collect button
                    collectButton.SetActive(true);
                    oneHourTimer.SetActive(false);
                }
                else
                {
                    OnTwelveHourTimerComplete(); // Reset ad rewards when the 12-hour timer completes
                }
            }
            else
            {
                // Display remaining time
                TimeSpan time = TimeSpan.FromSeconds(remainingTime);

                if (isFirstReward)
                {
                    // First reward timer (MMm SSs)
                    timerText.text = $"{time.Minutes}m {time.Seconds}s";
                }
                else
                {
                    // Ad reward timer (HHh MMm)
                    timerText.text = $"{time.Hours}h {time.Minutes}m";
                }
            }
        }
    }
    public void CollectFirstReward()
    {
        FBPlayerData.instance.VibrationEffect();
        InitManager.instance.currentReward = "25_coins";
        CollectFirstReward_AfterTap();
    }
    public void CollectFirstReward_AfterTap()
    {
        Debug.Log("____CollectFirstReward");
        // Call the data logic from DailyRewardsManager
        DailyRewardsManager.instance.CollectFirstReward();

        // Handle UI logic
        oneHourTimer.SetActive(true);
        collectButton.SetActive(false);
        FindObjectOfType<GiftNotification>().UpdateRewardsText();
        CoinManager.instance.AddCoins(25);
    }

    // ---------- COLLECT AD REWARD ----------
    public void CollectAdReward(int index)
    {
        
        FBPlayerData.instance.VibrationEffect();
        InitManager.instance.currentReward = index switch
        {
            0 => "100_coins",
            1 => "150_coins",
            2 => "250_coins",
            3 => "1_wild_card",
            _ => "default_reward" // Fallback for invalid indexes
        };
#if UNITY_EDITOR
        CollectAdReward_AfterRewardAd(index);
        return;
#endif
        if (GameUtils.IsFacebookBuild())
            Application.ExternalCall("ShowAd_Reward", index.ToString());
        else
            CollectAdReward_AfterRewardAd(index);

        
    }
    public void CollectAdReward_AfterRewardAd(int index)
    {
        DailyRewardsManager.instance.CollectAdReward(index);

        // Update UI
        adRewardButtons[index].SetActive(false);
        tickMarks[index].SetActive(true);
        //if(lockIcons[index])
        //    lockIcons[index].SetActive(false);
        // Unlock next reward (if exists)
        if (index + 1 < adRewardButtons.Length)
        {
            lockIcons[index].SetActive(false);
            adRewardButtons[index + 1].SetActive(true);
        }
        // ✅ Show 12-hour timer and adjust popup height
        if (index == 0)
        { 
            twelveHourTimer.SetActive(true);
            SetPopupHeight(); // Call the function to adjust height
        }
        FindObjectOfType<GiftNotification>().UpdateRewardsText();

        if(index == 0)
            CoinManager.instance.AddCoins(100);
        else if (index == 1)
            CoinManager.instance.AddCoins(150);
        else if (index == 2)
            CoinManager.instance.AddCoins(250);
        else if (index == 3)
        {
            // reward a Wild Card
            FBPlayerData.instance.TOTAL_WILD_CARD++;
            FBPlayerData.instance.SavePlayerData();
        }

    }
    // Update the UI based on the current reward state
    public void UpdateUI()
    {
        Debug.Log("UpdateUI: " + DailyRewardsManager.instance.IsAdRewardInCooldown());
        // Check if ad rewards are in cooldown
        if (DailyRewardsManager.instance.IsAdRewardInCooldown())
        {
            twelveHourTimer.SetActive(true); // Show 12-hour timer
            //int currentRewardIndex1 = DailyRewardsManager.instance.GetCurrentRewardIndex();
            //Debug.Log("_____currentRewardIndex1: " + currentRewardIndex1);
            //for (int i = 0; i < adRewardButtons.Length; i++)
            //{
            //    if (i < currentRewardIndex1) // Already collected
            //    {
            //        adRewardButtons[i].SetActive(false);
            //        tickMarks[i].SetActive(true);
            //    }
            //    else if (i == currentRewardIndex1) // Next available reward
            //    {
            //        adRewardButtons[i].SetActive(true);
            //        tickMarks[i].SetActive(false);
            //    }
            //    else // Locked rewards
            //    {
            //        adRewardButtons[i].SetActive(false);
            //        tickMarks[i].SetActive(false);
            //    }
            //}
        }
        else
        {
            twelveHourTimer.SetActive(false);
            
            for (int i = 0; i < adRewardButtons.Length; i++)
            {
                Debug.Log("UpdateUI: " + i);
                tickMarks[i].SetActive(false);
            }

        }

        // Adjust popup height
        SetPopupHeight();

        // Check if the first reward is available
        if (DailyRewardsManager.instance.IsFirstRewardAvailable())
        {
            collectButton.SetActive(true);
            oneHourTimer.SetActive(false);
        }
        else
        {
            collectButton.SetActive(false);
            oneHourTimer.SetActive(true);
        }

        // Check ad rewards availability
        int currentRewardIndex = DailyRewardsManager.instance.GetCurrentRewardIndex();
        Debug.Log("_____currentRewardIndex: " + currentRewardIndex);
        for (int i = 0; i < adRewardButtons.Length; i++)
        {
            if (i < currentRewardIndex) // Already collected
            {
                adRewardButtons[i].SetActive(false);
                tickMarks[i].SetActive(true);
            }
            else if (i == currentRewardIndex) // Next available reward
            {
                adRewardButtons[i].SetActive(true);
                tickMarks[i].SetActive(false);
            }
            else // Locked rewards
            {
                adRewardButtons[i].SetActive(false);
                tickMarks[i].SetActive(false);
            }
        }

        // Fix lock icon visibility (only for last 3 rewards: 3rd, 4th, and 5th)
        for (int i = 0; i < lockIcons.Length; i++)
        {
            int rewardIndex = i + 2; // Because lockIcons start from the 3rd reward (index 2)

            if (rewardIndex < currentRewardIndex)
            {
                lockIcons[i].SetActive(false); // Hide lock when reward is claimed
            }
            else if (rewardIndex == currentRewardIndex)
            {
                lockIcons[i].SetActive(false); // Hide lock for the next available reward
            }
            else
            {
                lockIcons[i].SetActive(true); // Show lock for future rewards
            }
        }
        if(adRewardButtons[1].activeSelf)
        {
            lockIcons[0].SetActive(false);
        }
        else if (adRewardButtons[2].activeSelf)
        {
            lockIcons[1].SetActive(false);
        }
        else if (adRewardButtons[3].activeSelf)
        {
            lockIcons[2].SetActive(false);
        }

        // Update ad reward timer if in cooldown
        Debug.Log("DailyRewardsManager.instance.IsAdRewardInCooldown: " + DailyRewardsManager.instance.IsAdRewardInCooldown());
        if (DailyRewardsManager.instance.IsAdRewardInCooldown())
        {
            float remainingTime = DailyRewardsManager.instance.GetRemainingTime(FBPlayerData.instance.LAST_AD_REWARD_TIME, DailyRewardsManager.instance.adRewardCooldown);
            TimeSpan time = TimeSpan.FromSeconds(Math.Max(remainingTime, 0));
            adRewardTimerText.text = $"{time.Hours}h {time.Minutes}m";
        }
    }
    // Reset ad rewards UI
    public void ResetAdRewardsUI()
    {
        twelveHourTimer.SetActive(false);

        // Hide all ad reward buttons
        foreach (var adButton in adRewardButtons)
        {
            adButton.SetActive(false);
        }

        // Hide all lock icons initially
        foreach (var lockIcon in lockIcons)
        {
            lockIcon.SetActive(false);
        }

        // Reset lock icons and tick marks correctly
        for (int i = 0; i < adRewardButtons.Length; i++)
        {
            if (i == 0)
            {
                // Show first ad reward button again (since it's now available)
                adRewardButtons[i].SetActive(true);
            }
            else
            {
                // Lock the next rewards again
                lockIcons[i - 1].SetActive(true);
            }

            // Tick marks remain visible for collected rewards
            tickMarks[i].SetActive(false);
        }

        // Ensure popup height goes back to normal
        SetPopupHeight();
        FindObjectOfType<GiftNotification>().UpdateRewardsText();

    }
    public void OnTwelveHourTimerComplete()
    {
        // Reset ad rewards data
        DailyRewardsManager.instance.ResetAdRewards();

        // Reset ad rewards UI
        ResetAdRewardsUI();
    }
}
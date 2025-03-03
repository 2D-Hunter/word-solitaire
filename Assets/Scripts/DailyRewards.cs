//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using TMPro;
//using System;
//using System.Linq;

//public class DailyRewards : MonoBehaviour
//{
//    public static DailyRewards instance;
//    public CanvasGroup bg = null;
//    public CanvasGroup popup = null;
//    public RectTransform popupRectTransform = null;

//    public CanvasGroup[] btns = null;
//    public RectTransform[] btnsRectTransform = null;

//    float delay = 0f;
//    float delayIncrement = 0.15f; // Adjust delay between each button if needed


//    public GameObject collectButton;      // Button for first reward
//    public GameObject[] adRewardButtons;  // Buttons for Ad Rewards (2nd to 5th)
//    public GameObject[] lockIcons;    // Lock icons for 3rd, 4th, 5th rewards
//    public GameObject[] tickMarks;    // Tick marks for completed rewards
//    public GameObject oneHourTimer = null;
//    public TextMeshProUGUI firstRewardTimerText;
//    public TextMeshProUGUI adRewardTimerText;    // Shows the next available ad reward timer
//    public GameObject twelveHourTimer = null;

//    private int firstRewardCooldown = 30;  // 1 hour in seconds  3600
//    private int adRewardCooldown = 30;    // 12 hours in seconds 43200
//    private string firstRewardKey = "LastFirstRewardTime";
//    private string adRewardKey = "LastAdRewardTime"; // Shared cooldown for all ad rewards
//    private int currentRewardIndex = 0; // Tracks the current unlock progress


//    private void Awake()
//    {
//        instance = this;
//        LoadState();
//        SetInit();
//    }
//    private void SetInit()
//    {
//        bg.alpha = 0;
//        popup.alpha = 0;
//        foreach (var btn in btns)
//        {
//            btn.alpha = 0;
//        }
//        foreach (var btnRectTransform in btnsRectTransform)
//        {
//            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
//        }
//        popupRectTransform.anchoredPosition = new Vector2(0, 150);
//        SetPopupHeight();

//    }
//    private void Start()
//    {
//        popupRectTransform.anchoredPosition = new Vector2(0, 150f);
//        ShowPopup();
//    }
//    void SetPopupHeight()
//    {
//        if (!twelveHourTimer.activeSelf)
//        {
//            popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 1320);
//        }
//        else
//        {
//            popupRectTransform.sizeDelta = new Vector2(popupRectTransform.sizeDelta.x, 1500);
//        }
//    }
//    public void ShowPopup()
//    {
//        SetInit();
//        bg.DOKill();
//        popup.DOKill();
//        popupRectTransform.DOKill();

//        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
//        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
//        popupRectTransform.DOAnchorPosY(500, 0.4f).SetEase(Ease.OutBack);

//        float delay = 0.05f; // Initial delay

//        for (int i = 0; i < btns.Length; i++)
//        {
//            // Ensure starting conditions
//            btns[i].alpha = 0;
//            btnsRectTransform[i].localScale = Vector3.one * 0.6f;

//            // Fade In
//            btns[i].DOFade(1f, 0.3f)
//                .SetEase(Ease.OutBack)
//                .SetDelay(delay);

//            // Scale Up
//            btnsRectTransform[i].DOScale(1f, 0.3f)
//                .SetEase(Ease.OutBack)
//                .SetDelay(delay);

//            delay += delayIncrement; // Increase delay for next button
//        }
//    }
//    public void ClosePopup()
//    {
//        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
//        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
//        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
//    }
//    void RemoveThis()
//    {
//        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
//    }

//    //______Reward-1

//    void Update()
//    {
//        UpdateTimers();
//    }

//    // ---------- LOAD PREVIOUS STATE ----------
//    void LoadState()
//    {
//        // ✅ Load saved `currentRewardIndex`
//        currentRewardIndex = PlayerPrefs.GetInt("CurrentRewardIndex", 0);
//        // ✅ Check if Ad Rewards are in cooldown (12-hour timer)
//        if (!CanCollectReward(adRewardKey, adRewardCooldown))
//        {
//            twelveHourTimer.SetActive(true); // Show 12-hour timer
//        }
//        else
//        {
//            twelveHourTimer.SetActive(false);
//        }

//        // ✅ Adjust Popup Height
//        SetPopupHeight();

//        Debug.Log("CanCollectReward: " + CanCollectReward(firstRewardKey, firstRewardCooldown));

//        if (CanCollectReward(firstRewardKey, firstRewardCooldown))
//        {
//            collectButton.SetActive(true);
//            oneHourTimer.SetActive(false);
//        }
//        else
//        {
//            collectButton.SetActive(false);
//            oneHourTimer.SetActive(true);
//        }

//        // Check Ad Rewards availability
//        for (int i = 0; i < adRewardButtons.Length; i++)
//        {
//            if (i < currentRewardIndex) // Already collected
//            {
//                adRewardButtons[i].SetActive(false);
//                tickMarks[i].SetActive(true);

//            }
//            else if (i == currentRewardIndex) // Next available reward
//            {
//                adRewardButtons[i].SetActive(true);
//                tickMarks[i].SetActive(false);

//            }
//            else // Locked rewards
//            {
//                adRewardButtons[i].SetActive(false);
//                tickMarks[i].SetActive(false);
//            }
//        }
//        // ✅ Fix Lock Icon Visibility (Only for last 3 rewards: 3rd, 4th, and 5th)
//        for (int i = 0; i < lockIcons.Length; i++)
//        {
//            int rewardIndex = i + 2; // Because lockIcons start from the 3rd reward (index 2)

//            if (rewardIndex < currentRewardIndex)
//            {
//                lockIcons[i].SetActive(false); // ✅ Hide lock when reward is claimed
//            }
//            else if (rewardIndex == currentRewardIndex)
//            {
//                lockIcons[i].SetActive(false); // ✅ Hide lock for the next available reward
//            }
//            else
//            {
//                lockIcons[i].SetActive(true); // ✅ Show lock for future rewards
//            }
//        }

//        if (!CanCollectReward(adRewardKey, adRewardCooldown))
//        {
//            long remainingTime = GetRemainingTime(adRewardKey, adRewardCooldown);
//            TimeSpan time = TimeSpan.FromSeconds(Math.Max(remainingTime, 0));
//        }
//    }

//    // ---------- CHECK IF REWARD CAN BE COLLECTED ----------
//    bool CanCollectReward(string key, int cooldown)
//    {
//        if (!PlayerPrefs.HasKey(key))
//            return true; // First-time users can collect

//        long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
//        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

//        bool canCollect = (currentTime - lastClaimTime) > cooldown;

//        return canCollect;
//    }

//    long GetRemainingTime(string key, int cooldown)
//    {
//        if (!PlayerPrefs.HasKey(key))
//            return 0;

//        long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
//        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
//        long elapsedTime = currentTime - lastClaimTime;
//        long remainingTime = Math.Max(cooldown - elapsedTime, 0);

//        // Debug logs to verify values
//        Debug.Log($"Last Claim Time: {lastClaimTime}");
//        Debug.Log($"Current Time: {currentTime}");
//        Debug.Log($"Elapsed Time: {elapsedTime}");
//        Debug.Log($"Remaining Time: {remainingTime}");

//        return remainingTime;
//    }



//    // ---------- COLLECT FIRST REWARD ----------
//    public void CollectFirstReward()
//    {
//        oneHourTimer.SetActive(true);
//        PlayerPrefs.SetString(firstRewardKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
//        PlayerPrefs.Save();
//        collectButton.SetActive(false);
//    }

//    // ---------- COLLECT AD REWARD ----------
//    public void CollectAdReward(int index)
//    {
//        if (index != currentRewardIndex) return; // Prevent collecting out of order

//        // Simulate watching an ad (Replace with actual ad logic)
//        Debug.Log($"Ad Reward {index + 1} Collected!");

//        // Save ad reward collection time
//        PlayerPrefs.SetString(adRewardKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

//        // ✅ Save `currentRewardIndex`
//        currentRewardIndex++;
//        PlayerPrefs.SetInt("CurrentRewardIndex", currentRewardIndex);

//        PlayerPrefs.Save();

//        // Update UI
//        adRewardButtons[index].SetActive(false);
//        tickMarks[index].SetActive(true);

//        // Unlock next reward (if exists)
//        if (index + 1 < adRewardButtons.Length)
//        {
//            lockIcons[index].SetActive(false);
//            adRewardButtons[index + 1].SetActive(true);
//        }
//        // ✅ Show 12-hour timer and adjust popup height
//        twelveHourTimer.SetActive(true);
//        SetPopupHeight(); // Call the function to adjust height
//    }

//    // ---------- UPDATE TIMERS ----------
//    void UpdateTimers()
//    {
//        UpdateTimer(firstRewardKey, firstRewardCooldown, firstRewardTimerText, collectButton, true);
//        UpdateTimer(adRewardKey, adRewardCooldown, adRewardTimerText, null, false);

//    }

//    void UpdateTimer(string key, int cooldown, TextMeshProUGUI timerText, GameObject button, bool isFirstReward)
//    {

//        if (!CanCollectReward(key, cooldown))
//        {

//            long remainingTime = GetRemainingTime(key, cooldown);
//            if (remainingTime <= 0)
//            {
//                // Ensure UI updates at 00:00
//                remainingTime = 0;

//                // Timer has finished
//                if (isFirstReward)
//                {
//                    // First reward: Show collect button
//                    collectButton.SetActive(true);
//                    oneHourTimer.SetActive(false);
//                }
//                else
//                {
//                    ResetAdRewards();  // Reset ad rewards when the 12-hour timer completes
//                }
//            }
//            else
//            {
//                // Display remaining time
//                TimeSpan time = TimeSpan.FromSeconds(remainingTime);

//                if (isFirstReward)
//                {
//                    // First reward timer (MMm SSs)
//                    timerText.text = $"{time.Minutes:D2}m {time.Seconds:D2}s";
//                }
//                else
//                {
//                    // Ad reward timer (HHh MMm)
//                    timerText.text = $"{time.Hours:D2}h {time.Minutes:D2}m";
//                }
//            }
//        }
//    }
//    void ResetAdRewards()
//    {
//        twelveHourTimer.SetActive(false);
//        // Reset reward index
//        currentRewardIndex = 0;

//        // Hide all ad reward buttons (they will be shown correctly later)
//        foreach (var adButton in adRewardButtons)
//        {
//            adButton.SetActive(false);
//        }

//        // Hide all lock icons initially
//        foreach (var lockIcon in lockIcons)
//        {
//            lockIcon.SetActive(false);
//        }

//        // Reset lock icons and tick marks correctly
//        for (int i = 0; i < adRewardButtons.Length; i++)
//        {
//            if (i == 0)
//            {
//                // Show first ad reward button again (since it's now available)
//                adRewardButtons[i].SetActive(true);
//            }
//            else
//            {
//                // Lock the next rewards again
//                lockIcons[i - 1].SetActive(true);
//            }

//            // Tick marks remain visible for collected rewards
//            tickMarks[i].SetActive(false);
//        }

//        // Ensure popup height goes back to normal
//        SetPopupHeight();
//    }

//}


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

    float delay = 0f;
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
        UpdateTimer(DailyRewardsManager.instance.firstRewardKey, DailyRewardsManager.instance.firstRewardCooldown, firstRewardTimerText, collectButton, true);
        UpdateTimer(DailyRewardsManager.instance.adRewardKey, DailyRewardsManager.instance.adRewardCooldown, adRewardTimerText, null, false);
    }

    void UpdateTimer(string key, int cooldown, TextMeshProUGUI timerText, GameObject button, bool isFirstReward)
    {
        if (!DailyRewardsManager.instance.CanCollectReward(key, cooldown))
        {
            long remainingTime = DailyRewardsManager.instance.GetRemainingTime(key, cooldown);
            if (remainingTime <= 0)
            {
                // Ensure UI updates at 00:00
                remainingTime = 0;

                // Timer has finished
                if (isFirstReward)
                {
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
        // Call the data logic from DailyRewardsManager
        DailyRewardsManager.instance.CollectFirstReward();

        // Handle UI logic
        oneHourTimer.SetActive(true);
        collectButton.SetActive(false);
        FindObjectOfType<GiftNotification>().UpdateRewardsText();

    }

    // ---------- COLLECT AD REWARD ----------
    public void CollectAdReward(int index)
    {
        DailyRewardsManager.instance.CollectAdReward(index);

        // Update UI
        adRewardButtons[index].SetActive(false);
        tickMarks[index].SetActive(true);
        lockIcons[index].SetActive(false);
        // Unlock next reward (if exists)
        if (index + 1 < adRewardButtons.Length)
        {
            lockIcons[index].SetActive(false);
            adRewardButtons[index + 1].SetActive(true);
        }
        // ✅ Show 12-hour timer and adjust popup height
        twelveHourTimer.SetActive(true);
        SetPopupHeight(); // Call the function to adjust height
        FindObjectOfType<GiftNotification>().UpdateRewardsText();
    }
    // Update the UI based on the current reward state
    public void UpdateUI()
    {
        Debug.Log("UpdateUI: " + DailyRewardsManager.instance.IsAdRewardInCooldown());
        // Check if ad rewards are in cooldown
        if (DailyRewardsManager.instance.IsAdRewardInCooldown())
        {
            twelveHourTimer.SetActive(true); // Show 12-hour timer
            //int currentRewardIndex = DailyRewardsManager.instance.GetCurrentRewardIndex();
            //Debug.Log("_____currentRewardIndex: " + currentRewardIndex);
            //for (int i = 0; i < adRewardButtons.Length; i++)
            //{
            //    if (i < currentRewardIndex) // Already collected
            //    {
            //        adRewardButtons[i].SetActive(false);
            //        tickMarks[i].SetActive(true);
            //    }
            //    else if (i == currentRewardIndex) // Next available reward
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

        // Update ad reward timer if in cooldown
        if (DailyRewardsManager.instance.IsAdRewardInCooldown())
        {
            long remainingTime = DailyRewardsManager.instance.GetRemainingTime(DailyRewardsManager.instance.adRewardKey, DailyRewardsManager.instance.adRewardCooldown);
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
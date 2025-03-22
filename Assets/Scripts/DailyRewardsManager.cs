using UnityEngine;
using System;

public class DailyRewardsManager : MonoBehaviour
{
    public static DailyRewardsManager instance;

    
    public int firstRewardCooldown = 3600;  // 1 hour in seconds (3600)

    
    public int adRewardCooldown = 43200;    // 12 hours in seconds (43200)
    public string firstRewardKey = "LastFirstRewardTime";
    public string adRewardKey = "LastAdRewardTime";
    private int currentRewardIndex = 0;   // Tracks the current unlock progress

    private int availableRewards = 5; // Start with 5 rewards

    private bool isTimerReset = false;
    private string availableRewardsKey = "AvailableRewards";


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make it persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }

        LoadState();
        CheckTimerOnAppStart();
    }
    private void Update()
    {
        // Check if the 1-hour timer has completed
        if (!CanCollectReward(firstRewardKey, firstRewardCooldown))
        {
            long remainingTime = GetRemainingTime(firstRewardKey, firstRewardCooldown);
            Debug.Log("DailyRewardsManager: " + remainingTime);

            if (remainingTime <= 0 && !isTimerReset)
            {
                IncreaseAvailableRewards(); // Reset available rewards to 5
                isTimerReset = true;  // Mark the timer as reset
                Debug.Log("Timer completed. Available rewards reset to 5.");
            }
        }
        else
        {
            // Reset the flag when the timer is active again
            isTimerReset = false;
        }
    }
    // Check the 1-hour timer when the app starts
    private void CheckTimerOnAppStart()
    {
        Debug.Log("CheckTimerOnAppStart: " + CanCollectReward(firstRewardKey, firstRewardCooldown));
        if (!CanCollectReward(firstRewardKey, firstRewardCooldown))
        {
            long remainingTime = GetRemainingTime(firstRewardKey, firstRewardCooldown);
            if (remainingTime <= 0)
            {
                IncreaseAvailableRewards(); // Reset available rewards to 5
            }
        }
        else
        {
            Debug.Log("1-hour timer: Reward can be collected (no cooldown).");

            // If the timer has completed, increase available rewards
            if(GameUtils.IsFacebookBuild())
            {
                string lastClaimTimeString = FBPlayerData.instance.LAST_REWARD_TIME;

                if (!string.IsNullOrEmpty(lastClaimTimeString) && long.TryParse(lastClaimTimeString, out long lastClaimTime))
                {
                    long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    long elapsedTime = currentTime - lastClaimTime;

                    if (elapsedTime >= firstRewardCooldown)
                    {
                        Debug.Log("1-hour timer already completed. Increasing available rewards.");
                        IncreaseAvailableRewards(); // Increase available rewards
                    }
                }
                else
                {
                    Debug.LogWarning($"LAST_REWARD_TIME is missing or invalid: {lastClaimTimeString}");
                }
            }
            else
            {
                if (PlayerPrefs.HasKey(firstRewardKey))
                {
                    string lastClaimTimeString = PlayerPrefs.GetString(firstRewardKey);
                    if (long.TryParse(lastClaimTimeString, out long lastClaimTime))
                    {
                        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        long elapsedTime = currentTime - lastClaimTime;

                        if (elapsedTime >= firstRewardCooldown)
                        {
                            Debug.Log("1-hour timer already completed. Increasing available rewards.");
                            IncreaseAvailableRewards(); // Increase available rewards
                        }
                    }
                    else
                    {
                        Debug.LogError($"Invalid value for key {firstRewardKey}: {lastClaimTimeString}");
                    }
                }
            }
            
        }

        // Check the 12-hour timer (if needed)
        if (!CanCollectReward(adRewardKey, adRewardCooldown))
        {
            long remainingTime = GetRemainingTime(adRewardKey, adRewardCooldown);
            Debug.Log($"12-hour timer remaining time: {remainingTime}");
            if (remainingTime <= 0)
            {
                IncreaseAvailableRewards(); // Reset available rewards to 5
            }
        }
        else
        {
            Debug.Log("12-hour timer: Reward can be collected (no cooldown).");
            
            // If the timer has completed, increase available rewards
            if(GameUtils.IsFacebookBuild())
            {
                string lastClaimTimeString = FBPlayerData.instance.LAST_AD_REWARD_TIME;

                if (!string.IsNullOrEmpty(lastClaimTimeString) && long.TryParse(lastClaimTimeString, out long lastClaimTime))
                {
                    long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    long elapsedTime = currentTime - lastClaimTime;

                    if (elapsedTime >= adRewardCooldown)
                    {
                        Debug.Log("12-hour timer already completed. Increasing available rewards.");
                        ResetAdRewards(); // Increase available rewards
                    }
                }
                else
                {
                    Debug.LogWarning($"LAST_AD_REWARD_TIME is missing or invalid: {lastClaimTimeString}");
                }
            }
            else
            {
                if (PlayerPrefs.HasKey(adRewardKey))
                {
                    string lastClaimTimeString = PlayerPrefs.GetString(adRewardKey);
                    if (long.TryParse(lastClaimTimeString, out long lastClaimTime))
                    {
                        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        long elapsedTime = currentTime - lastClaimTime;

                        if (elapsedTime >= adRewardCooldown)
                        {
                            Debug.Log("12-hour timer already completed. Increasing available rewards.");
                            ResetAdRewards(); // Increase available rewards
                        }
                    }
                    else
                    {
                        Debug.LogError($"Invalid value for key {adRewardKey}: {lastClaimTimeString}");
                    }
                }
            }
            
        }
    }


    // Load the current reward index and cooldown state
    private void LoadState()
    {
        if (GameUtils.IsFacebookBuild())
        {
            currentRewardIndex = FBPlayerData.instance.CURRENT_REWARD_INDEX;
            availableRewards = FBPlayerData.instance.AVAILABLE_REWARDS;
        }
        else
        {
            currentRewardIndex = PlayerPrefs.GetInt("CurrentRewardIndex", 0);
            availableRewards = PlayerPrefs.GetInt(availableRewardsKey, 5);
        }
    }

    // Check if a reward can be collected
    public bool CanCollectReward(string key, int cooldown)
    {
        string tempKey = "";
        if(key == firstRewardKey)
        {
            tempKey = FBPlayerData.instance.LAST_REWARD_TIME;
        }
        else if (key == adRewardKey)
        {
            tempKey = FBPlayerData.instance.LAST_AD_REWARD_TIME;
        }
        if (GameUtils.IsFacebookBuild())
        {
            if (string.IsNullOrEmpty(FBPlayerData.instance.LAST_REWARD_TIME) || string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME))
            {
                return true; // First-time users can collect
            }

            long lastClaimTime = long.Parse(tempKey);
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            bool canCollect = (currentTime - lastClaimTime) > cooldown;

            return canCollect;
        }
        else
        {
            if (!PlayerPrefs.HasKey(key))
                return true; // First-time users can collect

            long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            bool canCollect = (currentTime - lastClaimTime) > cooldown;

            return canCollect;
        }
        
    }

    // Get the remaining time for a reward
    public long GetRemainingTime(string key, int cooldown)
    {
        string tempKey = "";
        if (key == firstRewardKey)
        {
            tempKey = FBPlayerData.instance.LAST_REWARD_TIME;
        }
        else if (key == adRewardKey)
        {
            tempKey = FBPlayerData.instance.LAST_AD_REWARD_TIME;
        }
        if (GameUtils.IsFacebookBuild())
        {
            if (string.IsNullOrEmpty(FBPlayerData.instance.LAST_REWARD_TIME) || string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME))
            {
                return 0; // First-time users can collect
            }

            long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsedTime = currentTime - lastClaimTime;
            long remainingTime = Math.Max(cooldown - elapsedTime, 0);
            Debug.Log("remainingTime: " + remainingTime);
            return remainingTime;
        }
        else
        {
            if (!PlayerPrefs.HasKey(key))
                return 0;

            long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsedTime = currentTime - lastClaimTime;
            long remainingTime = Math.Max(cooldown - elapsedTime, 0);
            Debug.Log("remainingTime: " + remainingTime);
            return remainingTime;
        }
    }
    // Get the current reward index
    public int GetCurrentRewardIndex()
    {
        if(GameUtils.IsFacebookBuild())
            return FBPlayerData.instance.CURRENT_REWARD_INDEX;
        else
            return currentRewardIndex;
    }

    // Check if ad rewards are in cooldown
    public bool IsAdRewardInCooldown()
    {
        return !CanCollectReward(adRewardKey, adRewardCooldown);
    }

    // Check if the first reward is available
    public bool IsFirstRewardAvailable()
    {
        return CanCollectReward(firstRewardKey, firstRewardCooldown);
    }

    // Collect the first reward
    public void CollectFirstReward()
    {
        if (GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.LAST_REWARD_TIME = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetString(firstRewardKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            PlayerPrefs.Save();
        }

        
        DecreaseAvailableRewards();
    }

    // Collect an ad reward
    public void CollectAdReward(int index)
    {
        if (index != currentRewardIndex) return; // Prevent collecting out of order

        // Save ad reward collection time
        if(GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.LAST_AD_REWARD_TIME = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetString(adRewardKey, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
        }
        

        // Update current reward index
        currentRewardIndex++;
        if(GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.CURRENT_REWARD_INDEX = currentRewardIndex;
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetInt("CurrentRewardIndex", currentRewardIndex);
            PlayerPrefs.Save();
        }
        
        DecreaseAvailableRewards(); // Decrease available rewards by 1

    }


    // Reset ad rewards (e.g., when the 12-hour timer completes)
    public void ResetAdRewards()
    {
        currentRewardIndex = 0;
        if(GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.CURRENT_REWARD_INDEX = currentRewardIndex;
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetInt("CurrentRewardIndex", currentRewardIndex);
            PlayerPrefs.Save();
        }
        
        IncreaseAvailableRewards();
    }
    

    // Get the number of available rewards
    public int GetAvailableRewards()
    {
        return availableRewards;
    }

    // Decrease the number of available rewards by 1
    public void DecreaseAvailableRewards()
    {
        if (availableRewards > 0)
        {
            availableRewards--;
            if (GameUtils.IsFacebookBuild())
            {
                FBPlayerData.instance.AVAILABLE_REWARDS = availableRewards;
                FBPlayerData.instance.SavePlayerData();
            }
            else
            {
                PlayerPrefs.SetInt(availableRewardsKey, availableRewards); // Save to PlayerPrefs
                PlayerPrefs.Save();
            }
            
        }
    }
    public void IncreaseAvailableRewards()
    {
        if (availableRewards < 5)
        {
            availableRewards++;
            if (GameUtils.IsFacebookBuild())
            {
                FBPlayerData.instance.AVAILABLE_REWARDS = availableRewards;
                FBPlayerData.instance.SavePlayerData();
            }
            else
            {
                PlayerPrefs.SetInt(availableRewardsKey, availableRewards); // Save to PlayerPrefs
                PlayerPrefs.Save();
            }
        }
    }

    // Reset the number of available rewards to 5
    public void ResetAvailableRewards()
    {
        availableRewards = 4;
    }
}
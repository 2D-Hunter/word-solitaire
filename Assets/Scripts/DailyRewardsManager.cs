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

        
    }
    private void Start()
    {
        Debug.Log("availableRewards: " + availableRewards);
        LoadState();
        CheckTimerOnAppStart();
    }
    private void Update()
    {
        //Debug.Log("availableRewards: " + availableRewards);
        if (GameUtils.IsFacebookBuild())
        {
            // Check if the 1-hour timer has completed
            //Debug.Log("OO_OO "+FBPlayerData.instance.LAST_REWARD_TIME);
            //Debug.Log("OO_OO " + firstRewardCooldown);
            //Debug.Log("OO_OO " + CanCollectReward(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown));
            if (!CanCollectReward(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown))
            {
                float remainingTime = GetRemainingTime(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown);
                int remainingTime1 = Mathf.FloorToInt(remainingTime);
                    Debug.Log("DailyRewardsManager: " + availableRewards);

                if (remainingTime1 <= 0 && !isTimerReset)
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
        else
        {
            // Check if the 1-hour timer has completed
            if (!CanCollectReward(firstRewardKey, firstRewardCooldown))
            {
                float remainingTime = GetRemainingTime(firstRewardKey, firstRewardCooldown);
                Debug.Log("DailyRewardsManager: " + remainingTime);

                if (remainingTime <= 0 && !isTimerReset)
                {
                    FBPlayerData.instance.LAST_REWARD_TIME = "";
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
        
    }
    // Check the 1-hour timer when the app starts
    private void CheckTimerOnAppStart()
    {
        Debug.Log("CheckTimerOnAppStart: " + FBPlayerData.instance.LAST_REWARD_TIME);
        if(GameUtils.IsFacebookBuild())
        {
            if (!CanCollectReward(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown))
            {
                float remainingTime = GetRemainingTime(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown);
                int remainingTime1 = Mathf.FloorToInt(remainingTime);
                if (remainingTime1 <= 0)
                {
                    IncreaseAvailableRewards(); // Reset available rewards to 5
                }
            }
            else
            {
                Debug.Log("1-hour timer: Reward can be collected (no cooldown).");

                // If the timer has completed, increase available rewards
                    string lastClaimTimeString = FBPlayerData.instance.LAST_REWARD_TIME;
                Debug.Log("lastClaimTimeString: "+ lastClaimTimeString);
                Debug.Log("lastClaimTimeString: "+ string.IsNullOrEmpty(lastClaimTimeString));
                Debug.Log("lastClaimTimeString: "+ lastClaimTimeString != "");
                Debug.Log("lastClaimTimeString: "+ lastClaimTimeString != null);
                Debug.Log("lastClaimTimeString: "+ lastClaimTimeString != "0");
                //if (!string.IsNullOrEmpty(lastClaimTimeString) && long.TryParse(lastClaimTimeString, out long lastClaimTime))
                //{
                if (!string.IsNullOrEmpty(lastClaimTimeString) && lastClaimTimeString != "0")
                {
                    DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(lastClaimTimeString));
                    TimeSpan elapsedTime = DateTime.UtcNow - lastTime;

                    TimeSpan cooldownSpan = TimeSpan.FromSeconds(firstRewardCooldown);

                    float remainingTime = Math.Max((float)cooldownSpan.TotalSeconds - (float)elapsedTime.TotalSeconds, 0);
                    int remainingTime1 = Mathf.FloorToInt(remainingTime);

                    Debug.Log("CheckTimerOnAppStart: " + remainingTime1 + "_____" + firstRewardCooldown);
                    if (remainingTime1 <= firstRewardCooldown)
                        {
                            Debug.Log("1-hour timer already completed. Increasing available rewards.");
                            FBPlayerData.instance.LAST_REWARD_TIME = "";
                            IncreaseAvailableRewards(); // Increase available rewards
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"LAST_REWARD_TIME is missing or invalid: {lastClaimTimeString}");
                    }

            }
            // 12-Hour Ad Reward Timer CHECK
            Debug.Log("CheckTimerOnAppStart: " + IsAdRewardInCooldown());
            if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME) && FBPlayerData.instance.LAST_AD_REWARD_TIME != "0")
            {
                if (!IsAdRewardInCooldown())
                {
                    Debug.Log("12-hour ad reward cooldown finished. Resetting ad rewards...");
                    FBPlayerData.instance.LAST_AD_REWARD_TIME = "";
                    ResetAdRewards();
                }
            }
                
        }
        else
        {
            if (!CanCollectReward(firstRewardKey, firstRewardCooldown))
            {
                float remainingTime = GetRemainingTime(firstRewardKey, firstRewardCooldown);
                if (remainingTime <= 0)
                {
                    IncreaseAvailableRewards(); // Reset available rewards to 5
                }
            }
            else
            {
                Debug.Log("1-hour timer: Reward can be collected (no cooldown).");

                // If the timer has completed, increase available rewards
                if (GameUtils.IsFacebookBuild())
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
        //Debug.Log("Key  Cooldown   " + key + "      " + cooldown);
        string tempKey = "";
        //if (key == firstRewardKey)
        //{
        //    tempKey = FBPlayerData.instance.LAST_REWARD_TIME;
        //}
        //else if (key == adRewardKey)
        //{
        //    tempKey = FBPlayerData.instance.LAST_AD_REWARD_TIME;
        //}
        if (GameUtils.IsFacebookBuild())
        {
            //if (string.IsNullOrEmpty(FBPlayerData.instance.LAST_REWARD_TIME) || string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME))
            //{
            //    return true; // First-time users can collect
            //}
            //if (string.IsNullOrEmpty(key) || key == "" || key == null || key == "0")
            //{
            //    return true;
            //}
            //Debug.Log("Keyyy: " + key);
            if (string.IsNullOrWhiteSpace(key) || !long.TryParse(key, out long parsedKey))
            {
                //Debug.LogWarning("Invalid or missing key, allowing reward. key: " + key);
                return true;
            }
            DateTime lastTime = DateTime.FromBinary(parsedKey);
            //Debug.Log("tempKey: " + key);
            //DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(key));
            TimeSpan timeElapsed = DateTime.UtcNow - lastTime;
            TimeSpan cooldownSpan = TimeSpan.FromSeconds(cooldown);
            bool canCollect = timeElapsed > cooldownSpan;
            //Debug.Log("____: " + timeElapsed);
            //Debug.Log("____: "+ cooldownSpan);
            //long lastClaimTime = long.Parse(tempKey);
            //long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            //bool canCollect = (currentTime - lastClaimTime) > cooldown;
            //Debug.Log("canCollect: " + canCollect);
            return canCollect;
        }
        else
        {
            //Debug.Log(key + "wwwww:: "+ !PlayerPrefs.HasKey(key));
            if (!PlayerPrefs.HasKey(key))
                return true; // First-time users can collect

            
            long lastClaimTime = long.Parse(PlayerPrefs.GetString(key));
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            bool canCollect = (currentTime - lastClaimTime) > cooldown;
            //Debug.Log(lastClaimTime + "}}}}}}}");
            //Debug.Log(currentTime + "}}}}}}}");
            //Debug.Log(canCollect + "}}}}}}}");
            return canCollect;
        }
        
    }

    // Get the remaining time for a reward
    public float GetRemainingTime(string key, int cooldown)
    {
        string tempKey = "";
        //if (key == firstRewardKey)
        //{
        //    tempKey = FBPlayerData.instance.LAST_REWARD_TIME;
        //}
        //else if (key == adRewardKey)
        //{
        //    tempKey = FBPlayerData.instance.LAST_AD_REWARD_TIME;
        //}
        if (GameUtils.IsFacebookBuild())
        {
            //if (string.IsNullOrEmpty(FBPlayerData.instance.LAST_REWARD_TIME) || string.IsNullOrEmpty(FBPlayerData.instance.LAST_AD_REWARD_TIME))
            //{
            //    return 0; // First-time users can collect
            //}
            //Debug.Log("_____tempKey: " + key);
            if (string.IsNullOrEmpty(key) || key == "" || key == null || key == "0")
            {
                return 0;
            }
            

            DateTime lastClaimTime = DateTime.FromBinary(Convert.ToInt64(key));
            TimeSpan elapsedTime = DateTime.UtcNow - lastClaimTime;
            TimeSpan cooldownSpan = TimeSpan.FromSeconds(cooldown);
            
            float remainingTime = Math.Max((float)cooldownSpan.TotalSeconds - (float)elapsedTime.TotalSeconds, 0);
            int remainingTime1 = Mathf.FloorToInt(remainingTime);
            //Debug.Log("remainingTime1111: " + remainingTime1);
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
        if (GameUtils.IsFacebookBuild())
        {
            Debug.Log("IsAdRewardInCooldown: " + FBPlayerData.instance.LAST_AD_REWARD_TIME);
            return !CanCollectReward(FBPlayerData.instance.LAST_AD_REWARD_TIME, adRewardCooldown);
        }
        else
            return !CanCollectReward(adRewardKey, adRewardCooldown);
    }

    // Check if the first reward is available
    public bool IsFirstRewardAvailable()
    {
        if (GameUtils.IsFacebookBuild())
            return CanCollectReward(FBPlayerData.instance.LAST_REWARD_TIME, firstRewardCooldown);
        else
            return CanCollectReward(firstRewardKey, firstRewardCooldown);
    }

    // Collect the first reward
    public void CollectFirstReward()
    {
        if (GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.LAST_REWARD_TIME = DateTime.UtcNow.ToBinary().ToString();
            Debug.Log("FBPlayerData.instance.LAST_REWARD_TIME: "+ FBPlayerData.instance.LAST_REWARD_TIME);
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
        Debug.Log("____CollectAdReward: "+index);
        if (index != currentRewardIndex) return; // Prevent collecting out of order
        Debug.Log("____CollectAdReward: ");
        // Save ad reward collection time
        if (GameUtils.IsFacebookBuild())
        {
            if(index == 0)
                FBPlayerData.instance.LAST_AD_REWARD_TIME = DateTime.UtcNow.ToBinary().ToString();
            Debug.Log("FBPlayerData.instance.LAST_AD_REWARD_TIME: " + FBPlayerData.instance.LAST_AD_REWARD_TIME);
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
        availableRewards = availableRewards+4;
        if (GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.LAST_AD_REWARD_TIME = "";
            FBPlayerData.instance.CURRENT_REWARD_INDEX = currentRewardIndex;
            FBPlayerData.instance.AVAILABLE_REWARDS = availableRewards;
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetInt("CurrentRewardIndex", currentRewardIndex);
            PlayerPrefs.Save();
        }
        
        //IncreaseAvailableRewards();
    }
    

    // Get the number of available rewards
    public int GetAvailableRewards()
    {
        return availableRewards;
    }

    // Decrease the number of available rewards by 1
    public void DecreaseAvailableRewards()
    {
        Debug.Log("availableRewards: " + availableRewards);
        if (availableRewards > 0)
        {
            availableRewards--;
            Debug.Log("availableRewards: " + availableRewards);
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
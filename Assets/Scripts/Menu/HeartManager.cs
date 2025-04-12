using UnityEngine;
using System;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
    [HideInInspector]
    public int maxHearts = 5;
    [HideInInspector]
    public int currentHearts;
    private float heartRegenTime = 1800f; // 30 minutes per heart (1800 seconds)

    private string lastHeartTimeKey = "LastHeartTime";
    private string heartsKey = "PlayerHearts";

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
        LoadHearts();
        RegenerateHearts();
    }

    private void Update()
    {
        UpdateHeartTimer();
    }

    private void LoadHearts()
    {
        if (GameUtils.IsFacebookBuild())
            currentHearts = FBPlayerData.instance.TOTAL_HEARTS;
        else
            currentHearts = PlayerPrefs.GetInt(heartsKey, maxHearts);
    }

    private void SaveHearts()
    {
        if (GameUtils.IsFacebookBuild())
        {
            FBPlayerData.instance.TOTAL_HEARTS = currentHearts;
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetInt(heartsKey, currentHearts);
            PlayerPrefs.Save();
        }
    }

    private void RegenerateHearts()
    {
        if (currentHearts >= maxHearts)
        {
            if (GameUtils.IsFacebookBuild())
            {
                if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME))
                {
                    FBPlayerData.instance.LAST_HEART_TIME = "";
                    FBPlayerData.instance.SavePlayerData();
                    Debug.Log("Hearts full. LAST_HEART_TIME reset for Facebook Instant Games.");
                }
            }
            else
                PlayerPrefs.DeleteKey(lastHeartTimeKey); // No need to track time if full
            return;
        }
        if (GameUtils.IsFacebookBuild())
        {
            Debug.Log("FBPlayerData.instance.LAST_HEART_TIME: " + FBPlayerData.instance.LAST_HEART_TIME);

            if (FBPlayerData.instance.LAST_HEART_TIME != "")
            {
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(FBPlayerData.instance.LAST_HEART_TIME)));
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime; // Use UTC time for consistency

                int heartsToRegain = (int)(timeElapsed.TotalSeconds / heartRegenTime);
                float leftoverTime = (float)timeElapsed.TotalSeconds % heartRegenTime;

                if (heartsToRegain > 0)
                {
                    int newHeartValue = Mathf.Min(currentHearts + heartsToRegain, maxHearts);
                    int heartsGained = newHeartValue - currentHearts;
                    currentHearts = newHeartValue;
                    SaveHearts();

                    Debug.Log($"Hearts gained: {heartsGained}, Current Hearts: {currentHearts}");

                    if (currentHearts < maxHearts)
                    {
                        // Keep leftover seconds for the next heart regen timer
                        DateTime nextHeartTime = DateTime.UtcNow.AddSeconds(-leftoverTime);
                        FBPlayerData.instance.LAST_HEART_TIME = nextHeartTime.ToBinary().ToString();
                    }
                    else
                    {
                        FBPlayerData.instance.LAST_HEART_TIME = ""; // No need to track when full
                    }

                    FBPlayerData.instance.SavePlayerData();
                }
            }
            else
            {
                Debug.LogWarning("LAST_HEART_TIME is missing or invalid. No heart regen applied.");
            }
        }
        else
        {
            if (PlayerPrefs.HasKey(lastHeartTimeKey))
            {
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(lastHeartTimeKey)));
                TimeSpan timeElapsed = DateTime.Now - lastTime;

                int heartsToRegain = (int)(timeElapsed.TotalSeconds / heartRegenTime);
                float leftoverTime = (float)timeElapsed.TotalSeconds % heartRegenTime;

                // Only increase 1 heart at a time
                if (heartsToRegain > 0)
                {
                    currentHearts = Mathf.Min(currentHearts + 1, maxHearts);
                    SaveHearts();

                    // If hearts are still less than max, set new last heart regen time
                    if (currentHearts < maxHearts)
                    {
                        DateTime nextHeartTime = DateTime.Now.AddSeconds(-leftoverTime); // Keep remaining seconds
                        PlayerPrefs.SetString(lastHeartTimeKey, nextHeartTime.ToBinary().ToString());
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey(lastHeartTimeKey); // No need to track if full
                    }
                }
            }
        }
    }

    public void RefillHearts(int amount)
    {
        if (amount == 5)
            currentHearts = maxHearts;
        else
            currentHearts = currentHearts + 1;
        SaveHearts();

        if (currentHearts >= maxHearts)
        {
            if(GameUtils.IsFacebookBuild())
            {
                if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME))
                {
                    FBPlayerData.instance.LAST_HEART_TIME = "";
                    FBPlayerData.instance.SavePlayerData();
                    Debug.Log("Hearts full. LAST_HEART_TIME reset for Facebook Instant Games.");
                }
            } 
            else
                PlayerPrefs.DeleteKey(lastHeartTimeKey);
        }
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            SaveHearts();

            // ✅ Only start a new timer if no timer is currently running
            if (GameUtils.IsFacebookBuild())
            {
                if(currentHearts < maxHearts)
                {
                    FBPlayerData.instance.LAST_HEART_TIME = DateTime.Now.ToBinary().ToString();
                    FBPlayerData.instance.SavePlayerData();
                }
            }
            else
            {
                if (currentHearts < maxHearts && !PlayerPrefs.HasKey(lastHeartTimeKey))
                {
                    PlayerPrefs.SetString(lastHeartTimeKey, DateTime.Now.ToBinary().ToString());
                    PlayerPrefs.Save();
                }
            }
            
        }
    }

    public bool CanPlay()
    {
        return currentHearts > 0;
        //return false;
    }

    private void UpdateHeartTimer()
    {
        if (currentHearts >= maxHearts) return;
        if (GameUtils.IsFacebookBuild())
        {
            if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) &&
                long.TryParse(FBPlayerData.instance.LAST_HEART_TIME, out long lastHeartTimeBinary))
            {
                DateTime lastTime = DateTime.FromBinary(lastHeartTimeBinary);
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime; // Use UTC for WebGL & Facebook
                float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;

                if (remainingTime <= 0)
                {
                    RegenerateHearts();

                    if (currentHearts < maxHearts) // Only reset time if hearts are not full
                    {
                        FBPlayerData.instance.LAST_HEART_TIME = DateTime.UtcNow.ToBinary().ToString();
                        FBPlayerData.instance.SavePlayerData();
                    }
                    else
                    {
                        FBPlayerData.instance.LAST_HEART_TIME = "";
                        FBPlayerData.instance.SavePlayerData();
                    }

                    Debug.Log("Hearts regenerated. LAST_HEART_TIME updated.");
                }
            }
        }
        else
        {
            if (PlayerPrefs.HasKey(lastHeartTimeKey))
            {
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(lastHeartTimeKey)));
                TimeSpan timeElapsed = DateTime.Now - lastTime;
                float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;

                if (remainingTime <= 0)
                {
                    RegenerateHearts();
                }
            }
        }
    }
}
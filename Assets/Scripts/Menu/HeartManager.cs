using UnityEngine;
using System;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
    [HideInInspector]
    public int maxHearts = 5;
    [HideInInspector]
    public int currentHearts;
    private float heartRegenTime =1800f; // 30 minutes per heart (1800 seconds)

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
        Debug.Log("HeartManager: " + FBPlayerData.instance.TOTAL_HEARTS);
        LoadHearts();
        RegenerateHearts();
    }

    private void Update()
    {
        UpdateHeartTimer();
    }

    private void LoadHearts()
    {
        Debug.Log("LoadHearts: "+ FBPlayerData.instance.TOTAL_HEARTS);
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
        Debug.Log("RegenerateHearts: " + currentHearts);
        if (currentHearts >= maxHearts)
        {
            if (GameUtils.IsFacebookBuild())
            {
                if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) || FBPlayerData.instance.LAST_HEART_TIME != "" || FBPlayerData.instance.LAST_HEART_TIME != null || FBPlayerData.instance.LAST_HEART_TIME != "0")
                {
                    FBPlayerData.instance.LAST_HEART_TIME = "";
                    FBPlayerData.instance.SavePlayerData();
                    Debug.Log("Hearts full. LAST_HEART_TIME reset.");
                }
            }
            else
                PlayerPrefs.DeleteKey(lastHeartTimeKey); // No need to track time if full
            return;
        }
        Debug.Log("Hiiii Regenerate Hearts");
        if (GameUtils.IsFacebookBuild())
        {
            Debug.Log("FBPlayerData.instance.LAST_HEART_TIME: " + FBPlayerData.instance.LAST_HEART_TIME);

            if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) || FBPlayerData.instance.LAST_HEART_TIME != "" || FBPlayerData.instance.LAST_HEART_TIME != null || FBPlayerData.instance.LAST_HEART_TIME != "0")
            {
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(FBPlayerData.instance.LAST_HEART_TIME));
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime; // Use UTC time for consistency

                int heartsToRegain = (int)(timeElapsed.TotalSeconds / heartRegenTime);
                float leftoverTime = (float)timeElapsed.TotalSeconds % heartRegenTime;
                Debug.Log("____heartsToRegain: " + heartsToRegain);
                if (heartsToRegain > 0)
                {
                    int newHeartValue = Mathf.Min(currentHearts + heartsToRegain, maxHearts);
                    int heartsGained = newHeartValue - currentHearts;
                    currentHearts = newHeartValue;
                    SaveHearts();

                    Debug.Log($" Current Hearts: {currentHearts}");

                    if (currentHearts < maxHearts)
                    {
                        // Keep leftover seconds for the next heart regen timer
                        DateTime newHeartTime = DateTime.UtcNow;
                        //DateTime nextHeartTime = DateTime.UtcNow.AddSeconds(-leftoverTime);
                        FBPlayerData.instance.LAST_HEART_TIME = newHeartTime.ToBinary().ToString();
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
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime;

                int heartsToRegain = (int)(timeElapsed.TotalSeconds / heartRegenTime);
                float leftoverTime = (float)timeElapsed.TotalSeconds % heartRegenTime;
                Debug.Log("____heartsToRegain: "+ heartsToRegain);
                // Only increase 1 heart at a time
                if (heartsToRegain > 0)
                {
                    int newHeartValue = Mathf.Min(currentHearts + heartsToRegain, maxHearts);
                    int heartsGained = newHeartValue - currentHearts;
                    currentHearts = newHeartValue;
                    SaveHearts();

                    // Update last heart time only if not full
                    if (currentHearts < maxHearts)
                    {
                        DateTime newHeartTime = DateTime.UtcNow; ;
                        PlayerPrefs.SetString(lastHeartTimeKey, newHeartTime.ToBinary().ToString());
                    }
                    else
                    {
                       PlayerPrefs.DeleteKey(lastHeartTimeKey);
                    }

                    //// If hearts are still less than max, set new last heart regen time
                    //if (currentHearts < maxHearts)
                    //{
                    //    DateTime nextHeartTime = DateTime.UtcNow.AddSeconds(-leftoverTime); // Keep remaining seconds
                    //    PlayerPrefs.SetString(lastHeartTimeKey, nextHeartTime.ToBinary().ToString());
                    //}
                    //else
                    //{
                    //    PlayerPrefs.DeleteKey(lastHeartTimeKey); // No need to track if full
                    //}
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
                if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) || FBPlayerData.instance.LAST_HEART_TIME != "" || FBPlayerData.instance.LAST_HEART_TIME != null || FBPlayerData.instance.LAST_HEART_TIME != "0")
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
                Debug.Log("LoseHeart: " + currentHearts + maxHearts);
                Debug.Log("LoseHeart: " + FBPlayerData.instance.LAST_HEART_TIME);
                if (currentHearts < maxHearts && (string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) || FBPlayerData.instance.LAST_HEART_TIME == "" || FBPlayerData.instance.LAST_HEART_TIME == null || FBPlayerData.instance.LAST_HEART_TIME == "0"))
                {
                    FBPlayerData.instance.LAST_HEART_TIME = DateTime.UtcNow.ToBinary().ToString();
                    Debug.Log("FBPlayerData.instance.LAST_HEART_TIME: " + FBPlayerData.instance.LAST_HEART_TIME);
                    FBPlayerData.instance.SavePlayerData();
                }
            }
            else
            {
                if (currentHearts < maxHearts && !PlayerPrefs.HasKey(lastHeartTimeKey))
                {
                    PlayerPrefs.SetString(lastHeartTimeKey, DateTime.UtcNow.ToBinary().ToString());
                    Debug.Log("lastHeartTimeKey: "+ lastHeartTimeKey);
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
            if (!string.IsNullOrEmpty(FBPlayerData.instance.LAST_HEART_TIME) || FBPlayerData.instance.LAST_HEART_TIME != "" || FBPlayerData.instance.LAST_HEART_TIME != null || FBPlayerData.instance.LAST_HEART_TIME != "0")
            {
                //Debug.Log("UpdateHeartTimer: " + FBPlayerData.instance.LAST_HEART_TIME);

                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(FBPlayerData.instance.LAST_HEART_TIME));
                
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime; // Use UTC time for consistency
                float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;
                //Debug.Log("UpdateHeartTimer: " + lastTime);
                //Debug.Log("UpdateHeartTimer: " + timeElapsed);
                //Debug.Log("UpdateHeartTimer: " + remainingTime);
                if (remainingTime <= 0)
                {
                    RegenerateHearts();

                    //if (currentHearts < maxHearts) // Only reset time if hearts are not full
                    //{
                    //    long unixTime1 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    //    FBPlayerData.instance.LAST_HEART_TIME = unixTime1.ToString();
                    //    FBPlayerData.instance.SavePlayerData();
                    //}
                    //else
                    //{
                    //    FBPlayerData.instance.LAST_HEART_TIME = "";
                    //    FBPlayerData.instance.SavePlayerData();
                    //}

                    Debug.Log("Hearts regenerated. LAST_HEART_TIME updated.");
                }
            }
        }
        else
        {
            if (PlayerPrefs.HasKey(lastHeartTimeKey))
            {
                Debug.Log("UpdateHeartTimer: " + PlayerPrefs.GetString(lastHeartTimeKey));
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(lastHeartTimeKey)));
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime;
                float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;
                Debug.Log("UpdateHeartTimer: " + lastTime);
                Debug.Log("UpdateHeartTimer: " + timeElapsed);
                Debug.Log("UpdateHeartTimer: " + remainingTime);

                if (remainingTime <= 0)
                {
                    RegenerateHearts();
                }
            }
        }
    }
}
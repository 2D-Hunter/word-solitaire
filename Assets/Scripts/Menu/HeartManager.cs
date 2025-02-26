using UnityEngine;
using System;

public class HeartManager : MonoBehaviour
{
    public static HeartManager instance;
    [HideInInspector]
    public int maxHearts = 5;
    [HideInInspector]
    public int currentHearts;
    private float heartRegenTime = 60f; // 30 minutes per heart (1800 seconds)

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
        currentHearts = PlayerPrefs.GetInt(heartsKey, maxHearts);
    }

    private void SaveHearts()
    {
        PlayerPrefs.SetInt(heartsKey, currentHearts);
        PlayerPrefs.Save();
    }

    private void RegenerateHearts()
    {
        if (currentHearts >= maxHearts)
        { 
            PlayerPrefs.DeleteKey(lastHeartTimeKey); // No need to track time if full
            return;
        }

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

    public void RefillHearts(int amount)
    {
        if (amount == 5)
            currentHearts = maxHearts;
        else
            currentHearts = currentHearts + 1;
        SaveHearts();

        if(currentHearts >= maxHearts)
            PlayerPrefs.DeleteKey(lastHeartTimeKey);
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            SaveHearts();

            // ✅ Only start a new timer if no timer is currently running
            if (currentHearts < maxHearts && !PlayerPrefs.HasKey(lastHeartTimeKey))
            {
                PlayerPrefs.SetString(lastHeartTimeKey, DateTime.Now.ToBinary().ToString());
                PlayerPrefs.Save();
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
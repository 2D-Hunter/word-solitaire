using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HeartUI : MonoBehaviour
{
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI heartTextShadow;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTextShadow;
    private float heartRegenTime = 1800f;

    private void Update()
    {
        int currentHearts;
        if (GameUtils.IsFacebookBuild())
            currentHearts = FBPlayerData.instance.TOTAL_HEARTS;
        else
            currentHearts = PlayerPrefs.GetInt("PlayerHearts", HeartManager.instance.maxHearts);

        heartText.text = heartTextShadow.text = currentHearts.ToString();
        if (OutOfHeartsPopup.instance)
        {
            OutOfHeartsPopup.instance.heartText.text = OutOfHeartsPopup.instance.heartTextShadow.text = currentHearts.ToString();
        }
        else if (MoreHeartsPopup.instance)
        {
            MoreHeartsPopup.instance.heartText.text = MoreHeartsPopup.instance.heartTextShadow.text = currentHearts.ToString();
        }
        else if (MoreHeartsPopup2.instance)
        {
            MoreHeartsPopup2.instance.heartText.text = MoreHeartsPopup2.instance.heartTextShadow.text = currentHearts.ToString();
        }

        if (currentHearts >= HeartManager.instance.maxHearts)
        {
            if (MoreHeartsPopup.instance)
            {
                MoreHeartsPopup.instance.HeartsAreFull();
            }
            else if (MoreHeartsPopup2.instance)
            {
                MoreHeartsPopup2.instance.HeartsAreFull();
            }
            timerText.text = timerTextShadow.text = "FULL";
            return;
        }
        if (GameUtils.IsFacebookBuild())
        {
            string lastHeartTimeString = FBPlayerData.instance.LAST_HEART_TIME;

            if (!string.IsNullOrEmpty(lastHeartTimeString) &&
                long.TryParse(lastHeartTimeString, out long lastHeartTimeBinary))
            {
                DateTime lastTime = DateTime.FromBinary(lastHeartTimeBinary);
                TimeSpan timeElapsed = DateTime.UtcNow - lastTime; // Use UTC for WebGL accuracy
                float remainingTime = Mathf.Max(0f, heartRegenTime - (float)timeElapsed.TotalSeconds);

                if (remainingTime > 0)
                {
                    TimeSpan t = TimeSpan.FromSeconds(remainingTime);
                    string formattedTime = string.Format("{0}m {1}s", t.Minutes, t.Seconds);
                    string popupFormattedTime = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);

                    if (timerText != null && timerTextShadow != null)
                    {
                        timerText.text = timerTextShadow.text = formattedTime;
                    }

                    if (OutOfHeartsPopup.instance)
                    {
                        OutOfHeartsPopup.instance.timerText.text =
                        OutOfHeartsPopup.instance.timerTextShadow.text = popupFormattedTime;
                    }
                    else if (MoreHeartsPopup.instance)
                    {
                        MoreHeartsPopup.instance.timerText.text =
                        MoreHeartsPopup.instance.timerTextShadow.text = popupFormattedTime;
                    }
                    else if (MoreHeartsPopup2.instance)
                    {
                        MoreHeartsPopup2.instance.timerText.text =
                        MoreHeartsPopup2.instance.timerTextShadow.text = popupFormattedTime;
                    }
                }
                else
                {
                    if (timerText != null && timerTextShadow != null)
                    {
                        timerText.text = timerTextShadow.text = "FULL";
                    }
                }
            }
            else
            {
                Debug.LogWarning("LAST_HEART_TIME is missing or invalid.");
            }
        }
        else
        {
            if (PlayerPrefs.HasKey("LastHeartTime"))
            {
                DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("LastHeartTime")));
                TimeSpan timeElapsed = DateTime.Now - lastTime;
                float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;

                if (remainingTime > 0)
                {
                    TimeSpan t = TimeSpan.FromSeconds(remainingTime);
                    timerText.text = timerTextShadow.text = string.Format("{0}m {1}s", t.Minutes, t.Seconds);
                    if (OutOfHeartsPopup.instance)
                    {
                        OutOfHeartsPopup.instance.timerText.text = OutOfHeartsPopup.instance.timerTextShadow.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
                    }
                    else if (MoreHeartsPopup.instance)
                    {
                        MoreHeartsPopup.instance.timerText.text = MoreHeartsPopup.instance.timerTextShadow.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
                    }
                    else if (MoreHeartsPopup2.instance)
                    {
                        MoreHeartsPopup2.instance.timerText.text = MoreHeartsPopup2.instance.timerTextShadow.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
                    }
                }
                else
                {
                    timerText.text = timerTextShadow.text = "FULL";
                }
            }
        }
        
    }
}
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
        int currentHearts = PlayerPrefs.GetInt("PlayerHearts", HeartManager.instance.maxHearts);
        heartText.text = heartTextShadow.text = currentHearts.ToString();
        if (OutOfHeartsPopup.instance)
        {
            OutOfHeartsPopup.instance.heartText.text = OutOfHeartsPopup.instance.heartTextShadow.text = currentHearts.ToString();
        }

        if (currentHearts >= HeartManager.instance.maxHearts)
        {
            timerText.text = timerTextShadow.text = "FULL";
            return;
        }

        if (PlayerPrefs.HasKey("LastHeartTime"))
        {
            DateTime lastTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("LastHeartTime")));
            TimeSpan timeElapsed = DateTime.Now - lastTime;
            float remainingTime = heartRegenTime - (float)timeElapsed.TotalSeconds;

            if (remainingTime > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(remainingTime);
                timerText.text = timerTextShadow.text = string.Format("{0}m {1}s", t.Minutes, t.Seconds);
                if(OutOfHeartsPopup.instance)
                {
                    OutOfHeartsPopup.instance.timerText.text = OutOfHeartsPopup.instance.timerTextShadow.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
                }
            }
            else
            {
                timerText.text = timerTextShadow.text = "FULL";
            }
        }
    }
}
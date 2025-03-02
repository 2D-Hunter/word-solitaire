using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GiftNotification : MonoBehaviour
{
    public TextMeshProUGUI rewardsText; // Reference to the textbox in the menu screen
    public TextMeshProUGUI rewardsTextShadow; // Reference to the textbox in the menu screen

    private void Start()
    {
        // Update the rewards text when the menu loads
        UpdateRewardsText();
    }
    private void Update()
    {
        // Continuously check the timer and update the rewards text
        UpdateRewardsText();
    }


    // Update the rewards text
    public void UpdateRewardsText()
    {
        if (DailyRewardsManager.instance != null)
        {
            int availableRewards = DailyRewardsManager.instance.GetAvailableRewards();
            rewardsText.text = rewardsTextShadow.text = availableRewards.ToString();
        }
        else
        {
            Debug.LogWarning("DailyRewardsManager instance is null. Make sure it is initialized.");
        }
    }
}

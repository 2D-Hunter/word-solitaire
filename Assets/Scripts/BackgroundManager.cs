using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class BackgroundManager : MonoBehaviour
{
    public Image backgroundImage; // Assign in Inspector

    // Define level thresholds for background changes
    private int[] levelThresholds = { 0, 11, 26, 41, 61, 81, 101, 126, 151, 176, 201, 226, 251, 276, 301, 326, 351, 376, 401, 426, 451, 476, 501 };

    public TextMeshProUGUI nextLocationText; // Assign in Inspector
    public TextMeshProUGUI nextLocationTextShadow; // Assign in Inspector


    void Start()
    {
        //LoadBackgroundForLevel(1); // Start at level 1
    }

    public void OnLevelChanged(int level)
    {
        LoadBackgroundForLevel(level);
        
    }

    private void LoadBackgroundForLevel(int level)
    {
        int bgIndex = GetBackgroundIndex(level);
        string imageName = "Background/bg-" + (bgIndex + 1); // bg1, bg2, etc.
        Debug.Log("imageName: " + imageName);
        Sprite bgSprite = Resources.Load<Sprite>(imageName);

        if (bgSprite != null && backgroundImage != null)
        {
            backgroundImage.sprite = bgSprite;
        }
        else
        {
            Debug.LogWarning("Background not found: " + imageName);
        }
    }

    private int GetBackgroundIndex(int level)
    {
        // Finds the highest threshold that is <= current level
        int index = 0;
        for (int i = 0; i < levelThresholds.Length; i++)
        {
            if (level >= levelThresholds[i])
                index = i;
            else
                break;
        }
        return index;
    }
    public void UpdateNextLocationText(int level)
    {
        int nextMilestone = -1;

        for (int i = 0; i < levelThresholds.Length; i++)
        {
            if (level < levelThresholds[i])
            {
                nextMilestone = levelThresholds[i];
                break;
            }
        }

        if (nextLocationText != null)
        {
            if (nextMilestone != -1)
                nextLocationText.text = nextLocationTextShadow.text = "Next Location at " + nextMilestone;
            else
                nextLocationText.text = nextLocationTextShadow.text = "Final Location Reached";
        }
    }
    
}
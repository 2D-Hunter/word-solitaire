using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenTabHandler : MonoBehaviour
{
    public static GreenTabHandler instance;
    public RectTransform rectTransform;
    public Image gTabImg;
    public List<float> gTabWidth;
    public List<float> gTabPPUM;

    public List<GameObject> scoreMultiplier = null;

    Vector2 size;

    private void Start()
    {
        instance = this;
        size = rectTransform.sizeDelta;
        size.x = gTabWidth[0];
        rectTransform.sizeDelta = size;
        gTabImg.pixelsPerUnitMultiplier = gTabPPUM[0];
    }

    public void HandleGreenTab(string word)
    {
        
        Debug.Log("Word Length: " + word.Length);
        if(GameManager.instance.isValidWord)
        {
            size.x = gTabWidth[word.Length - 1];
            rectTransform.sizeDelta = size;
            gTabImg.pixelsPerUnitMultiplier = gTabPPUM[word.Length - 1];
        }
        else
        {
            size.x = 0;
            rectTransform.sizeDelta = size;
        }
        SwitchMultiplier(word.Length);
    }
    public void SwitchMultiplier(int i)
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2) return;
        foreach (var multiplier in scoreMultiplier)
        {
            multiplier.SetActive(false);
        }
        
        if (i <= 2) return;
        Debug.Log("______SwitchMultiplier: "+(i-1));
        if (GameManager.instance.isValidWord)
        {
            Debug.Log("______SwitchMultiplier: "+ scoreMultiplier[i - 3]);
            scoreMultiplier[i-3].SetActive(true);
            GameManager.instance.scoreMultiplier = i - 1;
        }
    }
}

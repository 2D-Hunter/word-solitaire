using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Splash : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Splash: " + FBPlayerData.instance.TOTAL_HEARTS);
#if UNITY_EDITOR
        FBPlayerData.instance.CURRENT_LEVEL = 500;
        LoadScene();
#endif


    }
    public void LoadScene()
    {
        
        Debug.Log("FBPlayerData.instance.CURRENT_LEVEL: " + FBPlayerData.instance.CURRENT_LEVEL);
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
            Invoke("LoadGame", 3f);
        else
            Invoke("LoadMenu", 3f);
    }
    void LoadMenu()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.bgmSource.clip, true, 10f);
        Initiate.Fade("Menu", Color.black, 1f);
    }
    void LoadGame()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.bgmSource.clip, true, 10f);
        Initiate.Fade("Game", Color.black, 1f);
    }
    public void ResetAllData()
    {
        Application.ExternalCall("ClearFBData");

    }
}

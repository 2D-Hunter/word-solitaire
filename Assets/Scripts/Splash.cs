using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.bgmSource.clip, true, 10f);
        if(FBPlayerData.instance.CURRENT_LEVEL == 1)
            Invoke("LoadGame", 3f);
        else
            Invoke("LoadMenu", 3f);
    }
    void LoadMenu()
    {
        Initiate.Fade("Menu", Color.black, 1f);
    }
    void LoadGame()
    {
        Initiate.Fade("Game", Color.black, 1f);
    }
}

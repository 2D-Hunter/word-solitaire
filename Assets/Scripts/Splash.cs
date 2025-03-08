using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadMenu", 3f);
    }
    void LoadMenu()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.bgmSource.clip, true, 10f);
        Initiate.Fade("Menu", Color.black, 1f);
    }
}

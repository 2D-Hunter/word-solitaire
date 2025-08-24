using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Splash : MonoBehaviour
{
    public GameObject loadingAnim = null;
    private void Start()
    {
        Debug.Log("Splash: " + FBPlayerData.instance.TOTAL_HEARTS);

#if UNITY_EDITOR
        FBPlayerData.instance.CURRENT_LEVEL = 17;
        StartCoroutine(LoadSceneRoutine());   // 🔑 Run coroutine
#else
        StartCoroutine(LoadSceneRoutine());   // 🔑 Run coroutine
#endif
    }

    public IEnumerator LoadSceneRoutine()
    {
        // Small wait to show splash logo
        yield return new WaitForSeconds(3f);
        loadingAnim.SetActive(true);
        bool bgLoaded = false;
        // 🔑 Preload the background for the current level
        if (BackgroundManager.instance != null)
        {
            yield return StartCoroutine(
                BackgroundManager.instance.PreloadCurrentLevelBackground(FBPlayerData.instance.CURRENT_LEVEL)
            );
            bgLoaded = true;
        }
        
        yield return new WaitUntil(() => bgLoaded && LoadConfig.instance.isAllConfigLoaded);

        Debug.Log("✅ All resources loaded → Proceeding to next scene");

        // Decide where to go
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 || FBPlayerData.instance.CURRENT_LEVEL == 2)
            LoadGame();
        else
            LoadMenu();
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
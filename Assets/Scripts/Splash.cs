using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Splash : MonoBehaviour
{
    public GameObject loadingAnim = null;

    private void Start()
    {
        Debug.Log("Splash: " + FBPlayerData.instance.TOTAL_HEARTS);

#if UNITY_EDITOR
        FBPlayerData.instance.CURRENT_LEVEL = 1; // very hard-94, hard-35
        StartCoroutine(LoadSceneRoutine());
#endif

    }

    public IEnumerator LoadSceneRoutine()
    {
        // Small wait to show splash logo
        yield return new WaitForSeconds(3f);
        loadingAnim.SetActive(true);

        // 🔑 Background preload with timeout
        if (BackgroundManager.instance != null)
        {
            yield return RunWithTimeout(
                BackgroundManager.instance.PreloadCurrentLevelBackground(FBPlayerData.instance.CURRENT_LEVEL),
                8f, // timeout in seconds
                () => Debug.LogWarning("⏳ Background preload timeout")
            );
        }

        // 🔑 Config load with timeout
        yield return RunWithTimeout(
            WaitUntilConfigLoaded(),
            8f,
            () => Debug.LogWarning("⏳ Config load timeout")
        );

        Debug.Log("✅ Proceeding to next scene (timeout-safe)");

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

    // ---------------------- Timeout Helpers ----------------------

    private IEnumerator RunWithTimeout(IEnumerator routine, float timeout, System.Action onTimeout = null)
    {
        bool finished = false;

        Coroutine runner = StartCoroutine(RunRoutine(routine, () => finished = true));

        float elapsed = 0f;
        while (!finished && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!finished)
        {
            onTimeout?.Invoke();
            StopCoroutine(runner); // cancel routine
        }
    }

    private IEnumerator RunRoutine(IEnumerator routine, System.Action onComplete)
    {
        yield return routine;
        onComplete?.Invoke();
    }

    private IEnumerator WaitUntilConfigLoaded()
    {
        yield return new WaitUntil(() => LoadConfig.instance.isAllConfigLoaded);
    }
}
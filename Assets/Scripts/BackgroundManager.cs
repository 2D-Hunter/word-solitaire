
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance;

    [Header("AWS Settings")]
    public string awsBaseUrl = "https://2dhunter.s3.us-west-2.amazonaws.com/word-solitaire-go/fb/backgrounds/";

    [Header("Fallback")]
    public Sprite defaultBackground;   // ✅ assign in Inspector

    private Dictionary<string, Sprite> loadedBackgrounds = new Dictionary<string, Sprite>();
    private Dictionary<int, Sprite> cachedBackgrounds = new Dictionary<int, Sprite>();
    private Image backgroundImage;

    private int[] levelThresholds = { 0, 11, 26, 41, 61, 81, 101, 126, 151, 176, 201, 226, 251, 276, 301, 326, 351, 376, 401, 426, 451, 476, 501 };

    public TextMeshProUGUI nextLocationText;
    public TextMeshProUGUI nextLocationTextShadow;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ Preload for current level (used in Splash)
    public IEnumerator PreloadCurrentLevelBackground(int level)
    {
        yield return PreloadBackgroundForLevel(level);
    }

    // ✅ Preload for upcoming level (used on LevelUp, Continue)
    public void PreloadBackgroundForUpcomingLevel(int level)
    {
        StartCoroutine(PreloadBackgroundForLevel(level));
    }
    public void MaybePreloadNextBackground()
    {
        if (!InitManager.instance.isReplay)
        {
            int nextLevel = FBPlayerData.instance.CURRENT_LEVEL + 1;
            Debug.Log("Preloading upcoming background for level " + nextLevel);
            StartCoroutine(PreloadCurrentLevelBackground(nextLevel));
        }
        else
        {
            Debug.Log("Replay → skipping preload of next bg");
        }
    }

    private IEnumerator PreloadBackgroundForLevel(int level)
    {
        int bgIndex = GetBackgroundIndex(level);
        string fileName = "bg-" + (bgIndex + 1) + ".jpg";

        if (loadedBackgrounds.ContainsKey(fileName))
        {
            Debug.Log("Already preloaded: " + fileName);
            yield break;
        }

        string url = awsBaseUrl + fileName;
        Debug.Log("Preloading background: " + url);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                loadedBackgrounds[fileName] = sprite;
                cachedBackgrounds[bgIndex] = sprite;

                Debug.Log("✅ Preloaded background: " + fileName);
            }
            else
            {
                Debug.LogError("❌ Failed to preload background: " + uwr.error);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Background image
        var bgObj = GameObject.FindGameObjectWithTag("BackgroundImage");
        backgroundImage = bgObj ? bgObj.GetComponent<Image>() : null;

        // Rebind next-location texts
        var nextTxtObj = GameObject.FindGameObjectWithTag("NextLocationText");
        nextLocationText = nextTxtObj ? nextTxtObj.GetComponent<TextMeshProUGUI>() : null;

        var nextShadowObj = GameObject.FindGameObjectWithTag("NextLocationTextShadow");
        nextLocationTextShadow = nextShadowObj ? nextShadowObj.GetComponent<TextMeshProUGUI>() : null;

        int currentLevel = FBPlayerData.instance.CURRENT_LEVEL;

        // ✅ Handle replay → show previous background
        if (InitManager.instance.isReplay)
        {
            Sprite prevBg = GetCachedBackground(currentLevel - 1);
            if (backgroundImage && prevBg)
            {
                backgroundImage.sprite = prevBg;
                Debug.Log("Replay → applied previous background ✅");
            }
        }
        else
        {
            // Apply cached background if available
            Sprite cached = GetCachedBackground(currentLevel);
            if (backgroundImage && cached)
            {
                backgroundImage.sprite = cached;
                Debug.Log("Applied cached background instantly ✅");
            }
            else
            {
                Debug.Log("No cache found → downloading...");
                OnLevelChanged(currentLevel);
            }
        }

        // Update milestone label (Next Location at …)
        UpdateNextLocationText(currentLevel);
    }

    public Sprite GetCachedBackground(int level)
    {
        int bgIndex = GetBackgroundIndex(level);
        if (cachedBackgrounds.ContainsKey(bgIndex))
            return cachedBackgrounds[bgIndex];
        return null;
    }

    public void OnLevelChanged(int level)
    {
        // 🚨 Skip background change if replaying
        if (InitManager.instance != null && InitManager.instance.isReplay)
        {
            Debug.Log("Replay mode → keep current background");
            return;
        }

        int bgIndex = GetBackgroundIndex(level);
        string fileName = "bg-" + (bgIndex + 1) + ".jpg";

        if (loadedBackgrounds.ContainsKey(fileName))
        {
            ApplyBackground(fileName);
        }
        else
        {
            StartCoroutine(DownloadAndApplyBackground(fileName, bgIndex));
        }

        UpdateNextLocationText(level);
    }

    private int GetBackgroundIndex(int level)
    {
        int index = 0;
        for (int i = 0; i < levelThresholds.Length; i++)
        {
            if (level >= levelThresholds[i]) index = i;
            else break;
        }
        return index;
    }

    private void ApplyBackground(string fileName)
    {
        if (backgroundImage != null)
        {
            if (loadedBackgrounds.ContainsKey(fileName))
            {
                backgroundImage.sprite = loadedBackgrounds[fileName];
            }
            else
            {
                // ✅ fallback
                backgroundImage.sprite = defaultBackground;
                Debug.LogWarning("Using default background (ApplyBackground) ❗");
            }
        }
    }

    private IEnumerator DownloadAndApplyBackground(string fileName, int bgIndex)
    {
        string url = awsBaseUrl + fileName;
        Debug.Log("Downloading background: " + url);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download background: " + uwr.error);
                // ✅ fallback immediately
                if (backgroundImage != null && defaultBackground != null)
                {
                    backgroundImage.sprite = defaultBackground;
                    Debug.Log("Applied default background due to error");
                }
            }
            else
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                loadedBackgrounds[fileName] = sprite;
                cachedBackgrounds[bgIndex] = sprite;

                ApplyBackground(fileName);
            }
        }
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
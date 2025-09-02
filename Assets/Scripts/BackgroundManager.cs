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
    public Sprite defaultBackground;   // assign in Inspector (optional)

    private Dictionary<string, Sprite> loadedBackgrounds = new Dictionary<string, Sprite>();
    private Dictionary<int, Sprite> cachedBackgrounds = new Dictionary<int, Sprite>();
    public Image backgroundImage;
    private BackgroundFitter fitter;

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

    // Preload for current level (used in Splash)
    public IEnumerator PreloadCurrentLevelBackground(int level)
    {
        yield return PreloadBackgroundForLevel(level);
    }

    // Preload for upcoming level
    public void PreloadBackgroundForUpcomingLevel(int level)
    {
        StartCoroutine(PreloadBackgroundForLevel(level));
    }

    public void MaybePreloadNextBackground()
    {
        if (!InitManager.instance.isReplay)
        {
            int nextLevel = FBPlayerData.instance.CURRENT_LEVEL + 1;
            Debug.Log($"[BG] MaybePreloadNextBackground level={nextLevel}");
            StartCoroutine(PreloadCurrentLevelBackground(nextLevel));
        }
        else
        {
            Debug.Log("[BG] Replay → skipping preload of next bg");
        }
    }

    private IEnumerator PreloadBackgroundForLevel(int level)
    {
        int bgIndex = GetBackgroundIndex(level);
        string fileName = $"bg-{bgIndex + 1}.jpg";

        Debug.Log($"[BG] Preload request level={level} -> bgIndex={bgIndex} file={fileName}");

        if (loadedBackgrounds.ContainsKey(fileName))
        {
            Debug.Log($"[BG] Already preloaded: {fileName}");
            yield break;
        }

        string url = awsBaseUrl + fileName;
        Debug.Log($"[BG] Preloading background: {url}");

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                loadedBackgrounds[fileName] = sprite;
                cachedBackgrounds[bgIndex] = sprite;

                Debug.Log($"[BG] ✅ Preloaded background: {fileName} (bgIndex={bgIndex})");
            }
            else
            {
                Debug.LogWarning($"[BG] ❌ Failed to preload background '{fileName}': {uwr.error}");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var bgObj = GameObject.FindGameObjectWithTag("BackgroundImage");
        backgroundImage = bgObj ? bgObj.GetComponent<Image>() : null;
        fitter = bgObj ? bgObj.GetComponent<BackgroundFitter>() : null;

        int currentLevel = FBPlayerData.instance.CURRENT_LEVEL;
        int bgIndex = GetBackgroundIndex(currentLevel);
        string fileName = $"bg-{bgIndex + 1}.jpg";

        Debug.Log($"[BG] OnSceneLoaded: scene={scene.name}, CURRENT_LEVEL={currentLevel}, bgIndex={bgIndex}, fileName={fileName}, backgroundImage={(backgroundImage ? backgroundImage.name : "null")}, fitter={(fitter ? "yes" : "no")}");

        // Replay: show previous cached background if present (you intentionally used previous level for replay)
        if (InitManager.instance.isReplay)
        {
            Sprite prevBg = GetCachedBackground(currentLevel - 1);
            if (prevBg != null)
            {
                Debug.Log("[BG] Replay -> applying previous background from cache");
                if (fitter) fitter.ApplySprite(prevBg); else backgroundImage.sprite = prevBg;
            }
            else
            {
                Debug.Log("[BG] Replay -> no previous cached background");
            }
            UpdateNextLocationText(currentLevel);
            return;
        }

        // Normal flow: apply cached if available, otherwise trigger download but DO NOT overwrite with default immediately.
        Sprite cached = GetCachedBackground(currentLevel);
        if (cached != null)
        {
            Debug.Log("[BG] Applying cached background.");
            if (fitter) fitter.ApplySprite(cached); else backgroundImage.sprite = cached;
        }
        else if (loadedBackgrounds.ContainsKey(fileName))
        {
            Debug.Log("[BG] Applying loaded (preloaded) background.");
            if (fitter) fitter.ApplySprite(loadedBackgrounds[fileName]); else backgroundImage.sprite = loadedBackgrounds[fileName];
        }
        else
        {
            Debug.Log("[BG] No cache or preloaded sprite found — starting download. (Will not overwrite current background until download completes)");
            StartCoroutine(DownloadAndApplyBackground(fileName, bgIndex));
        }

        UpdateNextLocationText(currentLevel);
    }

    public Sprite GetCachedBackground(int level)
    {
        int bgIndex = GetBackgroundIndex(level);
        if (cachedBackgrounds.ContainsKey(bgIndex))
            return cachedBackgrounds[bgIndex];
        return null;
    }

    // Revised GetBackgroundIndex — iterate from end so highest threshold wins
    private int GetBackgroundIndex(int level)
    {
        for (int i = levelThresholds.Length - 1; i >= 0; i--)
        {
            if (level >= levelThresholds[i]) return i;
        }
        return 0;
    }

    public void OnLevelChanged(int level)
    {
        if (InitManager.instance != null && InitManager.instance.isReplay)
        {
            Debug.Log("[BG] OnLevelChanged called while in replay mode -> ignoring");
            return;
        }

        int bgIndex = GetBackgroundIndex(level);
        string fileName = $"bg-{bgIndex + 1}.jpg";
        Debug.Log($"[BG] OnLevelChanged: level={level} -> file={fileName}");

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

    private void ApplyBackground(string fileName)
    {
        if (!backgroundImage)
        {
            Debug.LogWarning("[BG] ApplyBackground called but backgroundImage reference is null");
            return;
        }

        Sprite s = loadedBackgrounds.ContainsKey(fileName) ? loadedBackgrounds[fileName] : defaultBackground;
        if (s == null)
        {
            Debug.LogWarning($"[BG] ApplyBackground: no sprite for {fileName} and no defaultBackground set.");
            return;
        }

        Debug.Log($"[BG] ApplyBackground -> applying {fileName} via fitter? {(fitter != null)}");
        if (fitter) fitter.ApplySprite(s);
        else backgroundImage.sprite = s;
    }

    private IEnumerator DownloadAndApplyBackground(string fileName, int bgIndex)
    {
        string url = awsBaseUrl + fileName;
        Debug.Log($"[BG] Downloading background: {url}");

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[BG] Failed to download background {fileName}: {uwr.error}");

                // Only apply default if there is currently no background set (avoid flipping to default unexpectedly)
                if (backgroundImage != null && backgroundImage.sprite == null && defaultBackground != null)
                {
                    Debug.Log("[BG] Applying default background because no sprite is currently set.");
                    if (fitter) fitter.ApplySprite(defaultBackground);
                    else backgroundImage.sprite = defaultBackground;
                }
            }
            else
            {
                var tex = DownloadHandlerTexture.GetContent(uwr);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                loadedBackgrounds[fileName] = sprite;
                cachedBackgrounds[bgIndex] = sprite;

                Debug.Log($"[BG] Download complete: {fileName} stored in cache (bgIndex={bgIndex}).");

                // Only apply the sprite if the current level still maps to this bgIndex — avoids race when level changed mid-download.
                int currentLevel = FBPlayerData.instance.CURRENT_LEVEL;
                int currentIndex = GetBackgroundIndex(currentLevel);
                if (currentIndex == bgIndex)
                {
                    Debug.Log($"[BG] Current level still requires this BG (index {bgIndex}) -> applying now.");
                    if (fitter) fitter.ApplySprite(sprite);
                    else backgroundImage.sprite = sprite;
                }
                else
                {
                    Debug.Log($"[BG] Current level ({currentLevel}) no longer uses this sprite (expected index {currentIndex}). Not applying, but cached for future.");
                }
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
        Debug.Log("nextMilestone " + nextMilestone);
        InitManager.instance.nextMilestone = nextMilestone;
        if (nextLocationText != null && nextLocationTextShadow != null)
        {
            if (nextMilestone != -1)
                nextLocationText.text = nextLocationTextShadow.text = "Next Location at " + nextMilestone;
            else
                nextLocationText.text = nextLocationTextShadow.text = "Final Location Reached";
        }
    }
    public void RegisterBackground(Image bg)
    {
        backgroundImage = bg;
    }
}
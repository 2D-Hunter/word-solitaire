using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

namespace StarChestCreator          
{
public class StarManager : MonoBehaviour
{
    public static StarManager instance;
    public GameObject starPrefab;  // Reference to the star prefab used for the UI
    public Transform starParent;   // Parent transform for instantiated stars

    private static float remainingTime; // Static remaining time for global access

    public TextMeshProUGUI starText;  // UI text displaying the number of stars collected
    public Image starFillBar;         // UI fill bar showing progress towards the next target of stars
    public int targetStars = 20;      // Initial target number of stars
    public float starMultiplier = 2.0f;    // Multiplier to increase the target each time

    private static int totalStars = 0;  // Total stars collected across sessions
    private static int currentStars = 0;  // Stars collected towards the current target

    public ChestManager chestManager; // Reference to the ChestManager for interaction

    private bool chestReadyToOpen = false; // Flag indicating whether the chest is ready to be opened

    public GameObject starBarGameObject; // Reference to the Star Bar UI GameObject

    [Header("Star Time Settings")]
    public float threeStarThreshold = 50f; // Time threshold for earning 3 stars
    public float twoStarThreshold = 30f;   // Time threshold for earning 2 stars
    public float oneStarThreshold = 1f;    // Time threshold for earning 1 star

    void Awake()
    {
        instance = this;
        LoadStars(); // Load stars from saved data
        UpdateUI();  // Update the UI to reflect current star progress
    }

    void Start()
    {
        InstantiateStars(); // Instantiate the star objects but keep them hidden initially
    }

    // Instantiate the star objects but do not activate them yet
    private void InstantiateStars()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject star = Instantiate(starPrefab, starParent);
            star.SetActive(false); // Keep stars hidden until needed
        }
    }

    // Hide all instantiated stars
    public void HideStars()
    {
        foreach (Transform child in starParent)
        {
            child.gameObject.SetActive(false);  // Hide all stars
        }
    }

    // Hide the entire star bar UI
    public void HideStarBar()
    {
        if (starBarGameObject != null)
        {
            starBarGameObject.SetActive(false);
        }
    }

    // Show the star bar UI
    public void ShowStarBar()
    {
        if (starBarGameObject != null)
        {
            starBarGameObject.SetActive(true);
        }
    }

    // Load star data from PlayerPrefs
    void LoadStars()
    {
        totalStars = PlayerPrefs.GetInt("TotalStars", 0);       // Toplam yıldızları yükle
        currentStars = PlayerPrefs.GetInt("CurrentStars", 0);   // Mevcut yıldızları yükle
        targetStars = PlayerPrefs.GetInt("TargetStars", targetStars); // Hedef yıldız sayısını yükle
        chestReadyToOpen = PlayerPrefs.GetInt("ChestReadyToOpen", 0) == 1; // Sandık durumunu yükle
    }

    // Yıldız ilerleme çubuğu ve UI'yi güncelleyen metod
    public void UpdateUI(bool animateFillBar = true, System.Action onFillComplete = null)
    {
        // Yıldız metnini güncelle
        if (starText != null)
        {
            starText.text = $"{currentStars}/{targetStars}";
        }

        // Hedef doluluk oranını hesapla
        float targetFillAmount = (float)currentStars / targetStars;

        // Barın animasyonlu şekilde dolmasını sağlayacak mı?
        if (animateFillBar)
        {
            StartCoroutine(SmoothFillBar(targetFillAmount, onFillComplete));
        }
        else
        {
            // Animasyon olmadan doluluk oranını ayarla
            if (starFillBar != null)
            {
                starFillBar.fillAmount = targetFillAmount;
            }
            onFillComplete?.Invoke();
        }
    }


    // Coroutine to smoothly fill the star bar over time
    // Yıldız barını yavaşça dolduran coroutine
    private IEnumerator SmoothFillBar(float targetFillAmount, System.Action onFillComplete = null)
    {
        float currentFillAmount = starFillBar.fillAmount;
        float duration = 0.5f;  // Animasyon süresi
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            if (starFillBar != null)
            {
                // Barı doldur
                starFillBar.fillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            }
            yield return null;
        }

        // Doluluk oranı ayarlandıktan sonra callback'i çalıştır
        if (starFillBar != null)
        {
            starFillBar.fillAmount = targetFillAmount;
        }
        onFillComplete?.Invoke();
    }


    // Check if the collected stars exceed the target, and prepare the chest if ready
    public void CheckTarget()
    {
        bool chestJustBecameReady = false;

        // Continue checking until currentStars is less than targetStars
        while (currentStars >= targetStars)
        {
            currentStars -= targetStars;  // Subtract the target stars from current stars
            chestReadyToOpen = true;      // Mark chest as ready to open
            chestJustBecameReady = true;

            // Move chest to the target position using the ChestAnimator
            ChestAnimator.instance.MoveChestToTarget();

            // Set the new target number of stars (with a multiplier)
            targetStars = Mathf.CeilToInt(targetStars * starMultiplier);
        }

        // Save progress if the chest just became ready
        if (chestJustBecameReady)
        {
            SaveStars();  // Save stars and target data
        }
    }

    // Check if the chest is ready to be opened
    public bool IsChestReady()
    {
        return chestReadyToOpen;
    }

    // Set the remaining time (static method)
    public static void SetTime(float time)
    {
        remainingTime = time; // Set the remaining time globally
    }

    // Update the star display based on the remaining time
    public static void UpdateStarDisplay()
    {
        if (instance == null) return;

        // Get the remaining time from GameTimeManager
        float remainingTime = GameTimeManager.GetRemainingTime();

        // Calculate the number of stars based on the remaining time
        int starCount = instance.CalculateStars(remainingTime);
        instance.ShowStarsWithDelay(starCount);
    }

    // Calculate the number of stars based on remaining time thresholds
    public int CalculateStars(float remainingTime)
    {
        if (remainingTime >= threeStarThreshold)
            return 3;
        else if (remainingTime >= twoStarThreshold && remainingTime < threeStarThreshold)
            return 2;
        else if (remainingTime >= oneStarThreshold && remainingTime < twoStarThreshold)
            return 1;
        else
            return 0;
    }

    // Show stars on the screen with a delay between each one
    private void ShowStarsWithDelay(int starCount)
    {
        HideStars(); // Hide all stars before showing them
        StartCoroutine(ActivateStarsWithDelay(starCount, 0.5f)); // Show stars with a delay
    }

    // Coroutine to activate stars one by one with a delay
    private IEnumerator ActivateStarsWithDelay(int starCount, float delay)
    {
        for (int i = 0; i < starCount; i++)
        {
            if (i < starParent.childCount)
            {
                // Get the star object and activate it
                Transform star = starParent.GetChild(i);
                PositionStar(star.gameObject, i, starCount); // Position the star in the UI
                star.gameObject.SetActive(true); // Show the star
                yield return new WaitForSeconds(delay);  // Wait before showing the next star
            }
        }
    }

    // Position the stars horizontally in the star bar container
    private void PositionStar(GameObject star, int index, int totalStars)
    {
        RectTransform parentRect = starParent.GetComponent<RectTransform>();

        // Calculate the total width of all stars and the spacing between them
        float containerWidth = parentRect.rect.width;
        float starWidth = 100f; // Assume each star is 100 units wide
        float totalWidth = (starWidth * totalStars) + (50f * (totalStars - 1)); // Add 50 units space between stars

        // Calculate the starting X position (centered)
        float startX = -(totalWidth / 2) + (starWidth / 2);

        // Calculate X position for this specific star
        float xPos = startX + (index * (starWidth + 50f)); // Add 50 units between stars

        // Set the star's position in the UI
        RectTransform starRect = star.GetComponent<RectTransform>();
        starRect.anchoredPosition = new Vector2(xPos, 0); // Center vertically
    }

    // Get the number of stars based on the remaining time and update totals
    // Kalan süreye göre toplanan yıldızları hesapla ve kaydet
    public int GetCollectedStars(bool updateUIImmediately = true)
    {
        //float remainingTime = GameTimeManager.GetRemainingTime(); // Get remaining time
        int starsCollected = CalculateStars(55);       // Calculate stars
        currentStars += starsCollected;                           // Add to current stars
        totalStars += starsCollected;                             // Add to total stars
        SaveStars();                                              // Save stars

        if (updateUIImmediately)
        {
            UpdateUI(); // Update the UI
        }

        return starsCollected;
    }

    // Yıldız verilerini PlayerPrefs'e kaydet
    void SaveStars()
    {
        PlayerPrefs.SetInt("TotalStars", totalStars);       // Toplam yıldızları kaydet
        PlayerPrefs.SetInt("CurrentStars", currentStars);   // Mevcut yıldızları kaydet
        PlayerPrefs.SetInt("TargetStars", targetStars);     // Hedef yıldız sayısını kaydet
        PlayerPrefs.SetInt("ChestReadyToOpen", chestReadyToOpen ? 1 : 0); // Sandığın açılma durumunu kaydet
        PlayerPrefs.Save(); // Verileri kalıcı hale getir
    }

    // Mark the chest as opened and reset the chest ready state
    public void ChestOpened()
    {
        chestReadyToOpen = false; // Reset chest readiness
        PlayerPrefs.SetInt("ChestReadyToOpen", chestReadyToOpen ? 1 : 0); // Save chest status
        PlayerPrefs.Save();
        UpdateUI(); // Update the UI to reflect changes
    }
}
}
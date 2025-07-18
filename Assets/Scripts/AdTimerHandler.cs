using UnityEngine;

public class AdTimerHandler : MonoBehaviour
{
    public static AdTimerHandler Instance;

    [Header("Interstitial Timing")]
    public float minAdInterval = 20f; // 2 minutes
    public float maxAdInterval = 80f; // 3 minutes

    private float adTimer;
    private float nextAdTime;
    public bool shouldShowAd = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ResetAdTimer();
    }

    void Update()
    {
        if (shouldShowAd) return;
        adTimer += Time.deltaTime;

        if (adTimer >= nextAdTime)
        {
            shouldShowAd = true;
            ResetAdTimer();
        }
        //Debug.Log("____Timer: "+ adTimer+"_____: "+nextAdTime);
    }

    void ResetAdTimer()
    {
        adTimer = 0;
        nextAdTime = Random.Range(minAdInterval, maxAdInterval + 1);
        Debug.Log($"⏱ Next ad scheduled in {nextAdTime:F0} seconds");
    }
    public void TryShowAd(string adPlacement)
    {
        if (shouldShowAd)
        {
            Application.ExternalCall("ShowAd_Interstitial", adPlacement);
        }
    }
}
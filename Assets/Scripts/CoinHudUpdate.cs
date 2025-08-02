using UnityEngine;
using TMPro;
using DG.Tweening;

public class CoinHUDUpdate : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI coinTextShadow;

    [Header("Tween Settings")]
    [Tooltip("Seconds per 100 coins of change.")]
    public float secondsPer100 = 0.15f;
    public float minDuration = 0.15f;
    public float maxDuration = 1.0f;
    public Ease ease = Ease.OutCubic;

    [Header("Increase Delay")]
    [Tooltip("Start delay when coins INCREASE. No delay is applied on decreases.")]
    private float increaseStartDelay = 1.4f;
    public bool popOnChange = true;

    private int _displayedCoins;
    private Tweener _coinTween;

    void OnEnable()
    {
        if (CoinManager.instance != null)
        {
            CoinManager.instance.OnCoinsUpdated.AddListener(UpdateCoinDisplay);
            _displayedCoins = CoinManager.instance.GetCoins();
            SetText(_displayedCoins);
        }
    }

    void OnDisable()
    {
        if (CoinManager.instance != null)
        {
            CoinManager.instance.OnCoinsUpdated.RemoveListener(UpdateCoinDisplay);
        }
        KillTween();
    }

    void UpdateCoinDisplay(int newAmount)
    {
        if (coinText == null) return;
        if (newAmount == _displayedCoins) return;

        // Compute duration by delta
        int delta = Mathf.Abs(newAmount - _displayedCoins);
        float duration = Mathf.Clamp((delta / 100f) * secondsPer100, minDuration, maxDuration);

        // Apply delay ONLY when increasing
        bool isIncrease = newAmount > _displayedCoins;
        float startDelay = isIncrease ? increaseStartDelay : 0f;

        // Restart tween with proper delay & target
        KillTween();

        _coinTween = DOVirtual.Int(_displayedCoins, newAmount, duration, v =>
        {
            _displayedCoins = v;
            SetText(_displayedCoins);
        })
        .SetDelay(startDelay)
        .SetEase(ease)
        .OnStart(() =>
        {
            if (!popOnChange) return;

            // Pop starts when tween actually starts (after delay)
            coinText.transform.DOKill();
            coinText.transform.localScale = Vector3.one;
            coinText.transform.DOScale(1.15f, 0.12f).OnComplete(() =>
                coinText.transform.DOScale(1f, 0.18f)
            );

            if (coinTextShadow != null)
            {
                coinTextShadow.transform.DOKill();
                coinTextShadow.transform.localScale = Vector3.one;
                coinTextShadow.transform.DOScale(1.15f, 0.12f).OnComplete(() =>
                    coinTextShadow.transform.DOScale(1f, 0.18f)
                );
            }
        })
        .SetLink(gameObject); // auto-kill with this GameObject
    }

    private void SetText(int value)
    {
        string s = value.ToString("N0");
        if (coinText) coinText.text = s;
        if (coinTextShadow) coinTextShadow.text = s;
    }

    private void KillTween()
    {
        if (_coinTween != null && _coinTween.IsActive())
        {
            _coinTween.Kill();
            _coinTween = null;
        }
    }
}
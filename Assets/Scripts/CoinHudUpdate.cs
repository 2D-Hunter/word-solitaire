using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinHUDUpdate : MonoBehaviour
{
    public TextMeshProUGUI coinText = null;
    public TextMeshProUGUI coinTextShadow = null;

    void Start()
    {
        if (CoinManager.instance != null)
        {
            CoinManager.instance.OnCoinsUpdated.AddListener(UpdateCoinDisplay);
            UpdateCoinDisplay(CoinManager.instance.GetCoins()); // Initialize UI
        }
    }

    void UpdateCoinDisplay(int coinAmount)
    {
        coinText.text = coinTextShadow.text = coinAmount.ToString();
    }

    void OnDestroy()
    {
        if (CoinManager.instance != null)
        {
            CoinManager.instance.OnCoinsUpdated.RemoveListener(UpdateCoinDisplay);
        }
    }
}
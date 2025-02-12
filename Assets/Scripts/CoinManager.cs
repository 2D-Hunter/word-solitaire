using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public int totalCoins;
    

    public UnityEvent<int> OnCoinsUpdated = new UnityEvent<int>(); // Event for UI update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCoins();
    }


    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins();
        OnCoinsUpdated.Invoke(amount);
    }

    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            SaveCoins();
            Debug.Log("SpednCoins");
            OnCoinsUpdated.Invoke(totalCoins); // Notify UI elements
            return true;
        }
        return false;
    }

    public int GetCoins()
    {
        return totalCoins;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("total_coins", totalCoins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        totalCoins = PlayerPrefs.GetInt("total_coins", 1000);
    }
}
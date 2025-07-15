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

        //LoadCoins();
    }


    public void AddCoins(int amount)
    {
        Debug.Log("AddCoins: "+ FBPlayerData.instance.TOTAL_COINS + "_____"+amount);
        FBPlayerData.instance.TOTAL_COINS += amount;
        Debug.Log("AddCoins: " + FBPlayerData.instance.TOTAL_COINS);
        SaveCoins();
        OnCoinsUpdated.Invoke(FBPlayerData.instance.TOTAL_COINS);
    }

    public bool SpendCoins(int amount)
    {
        Debug.Log("SpendCoins: "+ FBPlayerData.instance.TOTAL_COINS);
        Debug.Log("SpendCoins: "+ amount);
        if (FBPlayerData.instance.TOTAL_COINS >= amount)
        {
            FBPlayerData.instance.TOTAL_COINS -= amount;
            SaveCoins();
            
            OnCoinsUpdated.Invoke(FBPlayerData.instance.TOTAL_COINS); // Notify UI elements
            return true;
        }
        return false;
    }

    public int GetCoins()
    {
        
        if (GameUtils.IsFacebookBuild())
            return FBPlayerData.instance.TOTAL_COINS;
        else
            return FBPlayerData.instance.TOTAL_COINS;
    }

    private void SaveCoins()
    {
        Debug.Log("SaveCoins: " + GameUtils.IsFacebookBuild());
        if(GameUtils.IsFacebookBuild())
        {
            //FBPlayerData.instance.TOTAL_COINS = totalCoins;
            FBPlayerData.instance.SavePlayerData();
        }
        else
        {
            PlayerPrefs.SetInt("total_coins", totalCoins);
            PlayerPrefs.Save();
        }
        
    }

    private void LoadCoins()
    {
        if (GameUtils.IsFacebookBuild())
        {
            totalCoins = FBPlayerData.instance.TOTAL_COINS;
        }
        else
        {
            totalCoins = PlayerPrefs.GetInt("total_coins", 1000);
        }
        
    }
}
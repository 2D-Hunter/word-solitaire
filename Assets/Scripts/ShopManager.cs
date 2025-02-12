using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopPrefab; // Assign the Shop UI Prefab
    public RectTransform canvasTransform; // Assign the Canvas in Inspector

    private GameObject currentShopUI; // Reference to the instantiated UI

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

    }

    
    public void ToggleShop()
    {
        if (currentShopUI == null) // If shop is not open, instantiate it
        {
            currentShopUI = Instantiate(shopPrefab, canvasTransform);
        }
        else // If shop is open, close it
        {
            Destroy(currentShopUI);
            currentShopUI = null;
        }
    }
}
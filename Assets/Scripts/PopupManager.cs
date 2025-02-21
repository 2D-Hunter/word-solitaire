using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject goalPopup;
    public GameObject settingPopupMenu;
    public GameObject shop;
    public GameObject heartsFullPopup;
    public GameObject outOfHeartsPopup;

    private GameObject currentPopup;
    private GameObject currentPopup2;
    public RectTransform uiContainer;

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
    private void Start()
    {
        AssignUIContainer();
    }

    void AssignUIContainer()
    {
        uiContainer = GameObject.Find("UI-Panel").GetComponent<RectTransform>();
    }


    public void TogglePopup(GameObject prefab)
    {
        if(currentPopup == null)
        {
            currentPopup = Instantiate(prefab, uiContainer);
        }
        else
        {
            Destroy(currentPopup);
            currentPopup = null;
        }
    }
    public void ToggleShop()
    {
        if (currentPopup2 == null)
        {
            currentPopup2 = Instantiate(shop, uiContainer);
        }
        else
        {
            Destroy(currentPopup2);
            currentPopup2 = null;
        }
    }
}

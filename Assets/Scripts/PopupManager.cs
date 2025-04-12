using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject goalPopup;
    public GameObject settingPopupMenu;
    public GameObject settingPopupGame;
    public GameObject shop;
    public GameObject heartsFullPopup;
    public GameObject moreHeartsPopup;
    public GameObject moreHeartsPopup2;
    public GameObject outOfHeartsPopup;
    public GameObject deleteAccountPopup;
    public GameObject loading;
    public GameObject accountDeletedPopup;
    public GameObject privacyPolicy;
    public GameObject feedbackPopup;
    public GameObject feedbackSubmitted;
    public GameObject quitPopup;
    public GameObject dailyRewardsPopup;
    public GameObject purchasedItemPopup;
    public GameObject noAdAvailable;
    public GameObject dictionaryPopup;
    public GameObject message;

    private GameObject currentPopup;
    private GameObject currentPopup2;
    private GameObject currentPopup3;
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
    

    public void AssignUIContainer()
    {
        Debug.Log("AssignUIContainer");
        uiContainer = GameObject.Find("UI-Panel").GetComponent<RectTransform>();
    }


    public void TogglePopup(GameObject prefab)
    {
        if(currentPopup == null)
        {
            SoundManager.instance.PlaySFX("PopupAppear", 0.2f);
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
            SoundManager.instance.PlaySFX("PopupAppear");
            currentPopup2 = Instantiate(shop, uiContainer);
        }
        else
        {
            Destroy(currentPopup2);
            currentPopup2 = null;
        }
    }
    public void ToggleMessage(GameObject prefab)
    {
        if (currentPopup3 == null)
        {
            currentPopup3 = Instantiate(prefab, uiContainer);
        }
        else
        {
            Destroy(currentPopup3);
            currentPopup3 = null;
        }
    }
}

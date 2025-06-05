using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoreCards : MonoBehaviour
{
    public TextMeshProUGUI price;
    public TextMeshProUGUI priceShadow;
    public GameObject coin = null;
    public GameObject videoIcon = null;

    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        if(GameUtils.IsFacebookBuild())
        {
            coin.SetActive(false);
            videoIcon.SetActive(true);
        }
        else
        {
            coin.SetActive(true);
            videoIcon.SetActive(false);
        }
        Debug.Log("InitManager.instance.buyMoreCardsCntr: " + InitManager.instance.buyMoreCardsCntr);
        if (InitManager.instance.buyMoreCardsCntr == 1)
            InitManager.instance.moreCardsPrice = 150;
        else if (InitManager.instance.buyMoreCardsCntr == 2)
            InitManager.instance.moreCardsPrice = 250;
        else
            InitManager.instance.moreCardsPrice += 250;

        price.text = priceShadow.text = InitManager.instance.moreCardsPrice.ToString();
    }
}

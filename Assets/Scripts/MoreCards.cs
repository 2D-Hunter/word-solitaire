using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoreCards : MonoBehaviour
{
    public TextMeshProUGUI price;
    public TextMeshProUGUI priceShadow;

    private void Awake()
    {
        
    }
    private void OnEnable()
    {
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

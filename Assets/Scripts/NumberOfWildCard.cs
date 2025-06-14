using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberOfWildCard : MonoBehaviour
{
    public GameObject hud;
    public TextMeshProUGUI availableWildCard;

    // Start is called before the first frame update
    void Start()
    {
        UpdateWildCard();
    }
    public void UpdateWildCard()
    {
        if (FBPlayerData.instance.TOTAL_WILD_CARD <= 0)
            hud.SetActive(false);
        else
            hud.SetActive(true);
        availableWildCard.text = FBPlayerData.instance.TOTAL_WILD_CARD.ToString();
        
    }

}

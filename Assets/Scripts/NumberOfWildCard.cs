using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberOfWildCard : MonoBehaviour
{
    public TextMeshProUGUI availableWildCard;

    // Start is called before the first frame update
    void Start()
    {
        UpdateWildCard();
    }
    public void UpdateWildCard()
    {
        availableWildCard.text = FBPlayerData.instance.TOTAL_WILD_CARD.ToString();
    }


}

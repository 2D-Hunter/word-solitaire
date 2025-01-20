using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraCardManager : MonoBehaviour
{
    public static ExtraCardManager instance;
    public List<ExtraCard> extraCards;
    public List<ExtraCard> rightSideCards;

    private void Start()
    {
        instance = this;
        //if (rightSideCards.Count > 0)
        //{
        //    foreach (var card in rightSideCards)
        //    {
        //        card.GetComponent<RectTransform>().GetChild(2).gameObject.SetActive(false);
        //    }
        //}
    }

}

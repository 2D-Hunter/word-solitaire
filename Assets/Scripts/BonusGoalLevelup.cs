using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGoalLevelup : MonoBehaviour
{
    public GameObject bonusGoal = null;

    void Start()
    {
        if((FBPlayerData.instance.CURRENT_LEVEL-1) <= 25)
        {
            bonusGoal.SetActive(false);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 225f);
        }
        else
        {
            bonusGoal.SetActive(true);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 70f);
        }
    }
}

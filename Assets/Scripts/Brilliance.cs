using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Brilliance : MonoBehaviour
{
    public TextMeshProUGUI brillianceScoreTxt = null;
    public TextMeshProUGUI brillianceScoreShadowTxt = null;
    // Start is called before the first frame update
    void Start()
    {
        if(GameUtils.IsFacebookBuild())
            brillianceScoreTxt.text = brillianceScoreShadowTxt.text = FBPlayerData.instance.BRILLIANCE.ToString();
        else
            brillianceScoreTxt.text = brillianceScoreShadowTxt.text = PlayerPrefs.GetInt("BrillianceScore").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

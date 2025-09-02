using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerSelection : MonoBehaviour
{
    
    public TextMeshProUGUI entryFeesTxt;
    public TextMeshProUGUI rewardsTxt;
    public TextMeshProUGUI rewardsTxtShadow;


    public RectTransform settingBtn;
    public RectTransform heartHud;
    public RectTransform coinHud;

    public BackgroundManager backgroundManager;

    private void Awake()
    {

        //backgroundManager.GetComponent<BackgroundManager>().OnLevelChanged(FBPlayerData.instance.CURRENT_LEVEL);
        //backgroundManager.GetComponent<BackgroundManager>().UpdateNextLocationText(FBPlayerData.instance.CURRENT_LEVEL);

        InitManager.instance.CurrentScene = "MultiplayerSelection";
    }
    private void Start()
    {
        PopupManager.instance.AssignUIContainer();
        //AnimateButton();
    }
}

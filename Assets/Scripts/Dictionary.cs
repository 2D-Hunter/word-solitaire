using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Dictionary : MonoBehaviour
{
    public static Dictionary instance;
    public TextMeshProUGUI title = null;
    public TextMeshProUGUI titleShadow = null;
    public TextMeshProUGUI title2 = null;
    public TextMeshProUGUI title2Shadow = null;
    public GameObject loading = null;
    public GameObject makeWords = null;
    public GameObject wordnikIcon = null;
    public TextMeshProUGUI definition = null;

    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupObj = null;
    


    private void Awake()
    {
        instance = this;
        bg.alpha = 0;
        popup.alpha = 0;
        popupObj.anchoredPosition = new Vector2(0, -350f);
        makeWords.SetActive(false);
        wordnikIcon.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void Start()
    {
        //loading.SetActive(false);
        popupObj.anchoredPosition = new Vector2(0, -350f);



        //wordnikDefinition.SearchWord(SlotManager.instance.GetSlotString().ToLower());
        ShowPopup();

    }
    public void ShowPopup()
    {
        if(GameManager.instance.isValidWord)
        {
            title.text = SlotManager.instance.GetSlotString().ToUpper();
            titleShadow.text = SlotManager.instance.GetSlotString().ToUpper();
            title2.text = SlotManager.instance.GetSlotString().ToUpper();
            title2Shadow.text = SlotManager.instance.GetSlotString().ToUpper();

        }
        else
        {
            title.text = "";
            titleShadow.text = "";
            title2.text = "";
            title2Shadow.text = "";
        }
        
        
        bg.DOFade(0.4f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupObj.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);
        if(GameManager.instance.isValidWord)
        {
            makeWords.SetActive(false);
            wordnikIcon.SetActive(true);
            WordnikDefinition.instance.FetchDefinition(SlotManager.instance.GetSlotString().ToLower());
        }
        else
        {
            makeWords.SetActive(true);
            wordnikIcon.SetActive(false);
            loading.SetActive(false);
            definition.text = "";
        }
        
    }
    public void ClosePopup()
    {
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupObj.DOAnchorPosY(-283, 0.4f).SetEase(Ease.InBack);
    }
    void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.dictionaryPopup);
    }
}

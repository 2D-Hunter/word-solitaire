using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalPopup : MonoBehaviour
{
    public static GoalPopup instance;
    public LevelData levelData;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupObj = null;
    public GameObject outOfHeartsPopup = null;

    public CanvasGroup playBtn = null;
    public RectTransform playBtnRectTransform = null;

    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI currentLevelShadow;
    public TextMeshProUGUI goal;
    public TextMeshProUGUI goalShadow;

    private void Awake()
    {
        instance = this;
        SetInit();
    }
    private void SetInit()
    {
        bg.alpha = 0;
        popup.alpha = 0;
        playBtn.alpha = 0;
        playBtnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        popupObj.anchoredPosition = new Vector2(0, -350);

        currentLevel.text = currentLevelShadow.text = "Level " + levelData.levels[InitManager.instance.currentLevel - 1].levelNumber.ToString();
        goal.text = goalShadow.text = levelData.levels[InitManager.instance.currentLevel - 1].levelTarget.ToString();
    }

    private void Start()
    {
        popupObj.anchoredPosition = new Vector2(0, -350f);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupObj.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

        playBtn.DOFade(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.15f);
        playBtnRectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.15f);

    }
    public void ClosePopup()
    {
        if (HeartManager.instance.CanPlay())
        {
            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
            popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
            popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
        }
        else
        {
            outOfHeartsPopup.SetActive(true);
            OutOfHeartsPopup.instance.ShowPopup();
        }
        
    }
    public void OnTapClose()
    {
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(() => PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup));
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }
    public void RemoveThis()
    {
        if (Menu.instance.overlayPanel.activeSelf)
        {
            //Initiate.Fade("Game", Color.black, 1f);
            SceneManager.LoadScene("Game");
        }
        gameObject.SetActive(false);
        
    }
    public void StartGame()
    {
        Menu.instance.overlayPanel.SetActive(true);
        ClosePopup();
        
    }
}

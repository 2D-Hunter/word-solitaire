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

    public CanvasGroup playBtn = null;
    public RectTransform playBtnRectTransform = null;

    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI currentLevelShadow;
    public TextMeshProUGUI goal;
    public TextMeshProUGUI goalShadow;

    public CanvasGroup[] stars = null;
    public RectTransform[] starsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.1f;

    private void Awake()
    {
        instance = this;
        SetInit();
    }
    private void SetInit()
    {
        foreach (var star in stars)
        {
            star.alpha = 0;
        }
        foreach (var rt in starsRectTransform)
        {
            rt.localScale = Vector3.zero;
        }
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
        Invoke("AppearStars", 0.3f);

    }
    void AppearStars()
    {
        float delay = 0.1f;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].DOFade(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            // Scale Up
            starsRectTransform[i].DOScale(0.9f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            delay += delayIncrement; // Increase delay for next button
        }
    }
    public void ClosePopup()
    {
        FBPlayerData.instance.VibrationEffect();
        if (HeartManager.instance.CanPlay())
        {
            bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
            popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
            popupObj.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
        }
        else
        {
            PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);
            PopupManager.instance.TogglePopup(PopupManager.instance.outOfHeartsPopup);
        }
        
    }
    public void OnTapClose()
    {
        FBPlayerData.instance.VibrationEffect();
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
        PopupManager.instance.TogglePopup(PopupManager.instance.goalPopup);


    }
    public void StartGame()
    {
        Menu.instance.overlayPanel.SetActive(true);
        ClosePopup();
        
    }
}

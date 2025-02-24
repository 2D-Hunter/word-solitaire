using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsPopupGame : MonoBehaviour
{
    public static SettingsPopupGame instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;
    public TextMeshProUGUI versionText;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.15f; // Adjust delay between each button if needed


    private void Awake()
    {
        instance = this;
        versionText.text = InitManager.instance.version;
        SetInit();
    }
    private void SetInit()
    {
        bg.alpha = 0;
        popup.alpha = 0;
        foreach (var btn in btns)
        {
            btn.alpha = 0;
        }
        foreach (var btnRectTransform in btnsRectTransform)
        {
            btnRectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        popupRectTransform.anchoredPosition = new Vector2(0, -350);

    }
    private void Start()
    {
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();
        bg.DOKill();
        popup.DOKill();
        popupRectTransform.DOKill();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupRectTransform.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

        float delay = 0.05f; // Initial delay

        for (int i = 0; i < btns.Length; i++)
        {
            // Ensure starting conditions
            btns[i].alpha = 0;
            btnsRectTransform[i].localScale = Vector3.one * 0.6f;

            // Fade In
            btns[i].DOFade(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            // Scale Up
            btnsRectTransform[i].DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            delay += delayIncrement; // Increase delay for next button
        }
    }
    public void ClosePopup()
    {
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }
    void RemoveThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
    }

    public void Share()
    {
        Application.ExternalCall("Share");
    }
    public void Restart()
    {

    }
    public void Quit()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
        PopupManager.instance.TogglePopup(PopupManager.instance.quitPopup);
    }

}

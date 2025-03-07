using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class QuitPopup : MonoBehaviour
{
    
    public static QuitPopup instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    float delay = 0f;
    float delayIncrement = 0.15f; // Adjust delay between each button if needed
    public RectTransform heart; // Assign your heart image's RectTransform
    private float scaleAmount = 0.25f; // How much it expands
    private float duration = 0.1f; // Duration of one beat
    private float gapBetweenBeats = 5.0f; // Time before the next heartbeat cycle
    public Button[] buttons = null;

    private void Awake()
    {
        Debug.Log("Quit Popup Awake");
        instance = this;
        
    }
    private void OnEnable()
    {
        Debug.Log("Quit Popup OnEnable");
        SetInit();
    }
    private void SetInit()
    {
        
        GameManager.instance.overlayPanel.SetActive(false);
        foreach (var button in buttons)
        {
            button.enabled = true;
        }
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
        Debug.Log("Quit Popup Start");
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);
        Invoke("StartHeartBeat", 2f);
        ShowPopup();
    }
    public void ShowPopup()
    {
        SetInit();

        bg.DOFade(0.6f, 0.6f).SetEase(Ease.OutBack);
        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupRectTransform.DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);

        float delay = 0f; // Initial delay

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
        FBPlayerData.instance.VibrationEffect();
        foreach (var button in buttons)
        {
            button.enabled = false;
        }
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupRectTransform.DOAnchorPosY(-350, 0.4f).SetEase(Ease.InBack);
    }
    void RemoveThis()
    {
        if (GameManager.instance.overlayPanel.activeSelf)
        {
            //Initiate.Fade("Menu", Color.black, 1f);
            HeartManager.instance.LoseHeart();
            SceneManager.LoadScene("Menu");
        }
        PopupManager.instance.TogglePopup(PopupManager.instance.quitPopup);
    }
    public void Return()
    {
        FBPlayerData.instance.VibrationEffect();
        PopupManager.instance.TogglePopup(PopupManager.instance.quitPopup);
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupGame);
    }
    public void Quit()
    {
        GameManager.instance.overlayPanel.SetActive(true);
        ClosePopup();

    }

    void StartHeartBeat()
    {
        Sequence heartbeatSequence = DOTween.Sequence();

        heartbeatSequence.Append(heart.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // First beat
                         .Append(heart.DOScale(0.22f, duration).SetEase(Ease.InQuad)) // Back to normal
                         .Append(heart.DOScale(scaleAmount, duration).SetEase(Ease.OutQuad)) // Second beat
                         .Append(heart.DOScale(0.22f, duration).SetEase(Ease.InQuad)) // Back to normal
                         .AppendInterval(Random.Range(2f, 5f)) // Wait before next heartbeat
                         .SetLoops(-1); // Repeat infinitely
    }

}

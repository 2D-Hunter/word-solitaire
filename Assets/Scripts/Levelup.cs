using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class Levelup : MonoBehaviour
{

    public static Levelup instance;
    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupRectTransform = null;

    public CanvasGroup[] btns = null;
    public RectTransform[] btnsRectTransform = null;

    //float delay = 0f;
    float delayIncrement = 0.15f; // Adjust delay between each button if needed
    
    //private float scaleAmount = 0.25f; // How much it expands
    //private float duration = 0.1f; // Duration of one beat
    //private float gapBetweenBeats = 5.0f; // Time before the next heartbeat cycle
    public Button[] buttons = null;

    private void Awake()
    {
        Debug.Log("Levelup Awake");
        instance = this;

    }
    private void OnEnable()
    {
        Debug.Log("Levelup OnEnable");
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
        Debug.Log("Levelup Start");
        popupRectTransform.anchoredPosition = new Vector2(0, -350f);
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
        gameObject.SetActive(false);
    }
    public void Quit()
    {
        GameManager.instance.overlayPanel.SetActive(true);
        ClosePopup();

    }

}

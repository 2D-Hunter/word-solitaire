using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PrivacyPolicy : MonoBehaviour
{
    public CanvasGroup popup;

    private void Start()
    {
        popup.alpha = 0;

        popup.DOFade(1, 0.5f);
    }
    public void CloseThis()
    {
        PopupManager.instance.TogglePopup(PopupManager.instance.privacyPolicy);
        PopupManager.instance.TogglePopup(PopupManager.instance.settingPopupMenu);
    }
}

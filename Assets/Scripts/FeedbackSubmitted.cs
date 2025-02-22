using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FeedbackSubmitted : MonoBehaviour
{
    public CanvasGroup msg;
    private void Start()
    {
        Invoke("RemoveMessage", 3f);
    }
    void RemoveMessage()
    {
        msg.DOFade(0, 1f).OnComplete(RemoveThis);
    }
    void RemoveThis()
    {
        InitManager.instance.feedbackSubmitted = false;
        PopupManager.instance.ToggleMessage(PopupManager.instance.feedbackSubmitted);
    }
}

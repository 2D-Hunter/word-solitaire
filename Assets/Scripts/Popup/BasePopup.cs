using UnityEngine;
using DG.Tweening;

public class BasePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // Helps with fading in/out
    [SerializeField] private Transform popupTransform; // Helps with scaling animation
    [SerializeField] private float animationDuration = 0.3f;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (popupTransform == null) popupTransform = transform;

        // Initially set to hidden
        canvasGroup.alpha = 0;
        popupTransform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.DOFade(1, animationDuration);
        popupTransform.DOScale(1, animationDuration).SetEase(Ease.OutBack);
    }

    public virtual void Hide()
    {
        canvasGroup.DOFade(0, animationDuration);
        popupTransform.DOScale(0, animationDuration).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
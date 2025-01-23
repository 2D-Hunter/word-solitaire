using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour
{
    private float scaleDownFactor = 0.95f;
    private float animationDuration = 0.2f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = new Vector3(1, 1, 1);
    }

    public void OnButtonPress()
    {
        transform.DOScale(originalScale * scaleDownFactor, animationDuration).SetEase(Ease.OutQuad);
    }

    public void OnButtonRelease()
    {
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);
    }
}
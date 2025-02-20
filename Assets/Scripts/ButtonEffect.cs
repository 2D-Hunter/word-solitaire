using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour
{
    private float scaleDownFactor = 0.9f;
    private float animationDuration = 0.1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = new Vector3(1, 1, 1);
    }

    public void OnButtonPress()
    {
        transform.DOScale(originalScale * scaleDownFactor, animationDuration);
    }

    public void OnButtonRelease()
    {
        transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutElastic);
    }
}
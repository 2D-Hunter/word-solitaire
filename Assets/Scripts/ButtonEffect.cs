

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour
{
    [SerializeField] private bool effectEnabled = true;

    private float scaleDownFactor = 0.9f;
    private float animationDuration = 0.1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = new Vector3(1, 1, 1);
    }

    public void OnButtonPress()
    {
        if (!effectEnabled) return;

        transform.DOScale(originalScale * scaleDownFactor, animationDuration);
    }

    public void OnButtonRelease()
    {
        if (!effectEnabled) return;

        transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutElastic);
    }

    private void OnDestroy()
    {
        transform?.DOKill();
    }

    public void DisableEffect()
    {
        effectEnabled = false;
    }
    public void EnableEffect()
    {
        effectEnabled = true;
    }
}
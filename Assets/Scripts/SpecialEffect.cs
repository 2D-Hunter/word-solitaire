using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpecialEffect : MonoBehaviour
{
    public RectTransform shimmer; // Assign the shimmer image
    public float duration = 4f; // Time to move across
    public float delayBetweenLoops = 2f; // Delay before repeating
    public float shimmerWidth = 600f; // Width of shimmer travel area
    private float initialDelay = 3f; // Delay after menu appears

    private Vector2 startPos;
    private Vector2 endPos;

    void Start()
    {
        startPos = new Vector2(-shimmerWidth, shimmer.anchoredPosition.y);
        endPos = new Vector2(shimmerWidth, shimmer.anchoredPosition.y);

        shimmer.anchoredPosition = startPos;

        StartCoroutine(LoopShimmer());
    }

    private IEnumerator LoopShimmer()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            shimmer.anchoredPosition = startPos;

            shimmer.gameObject.SetActive(true);
            shimmer.DOAnchorPos(endPos, duration).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(duration);
            shimmer.gameObject.SetActive(false);

            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }
}

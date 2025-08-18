using UnityEngine;
using DG.Tweening;
using TMPro;

public class HintBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintTxt;
    [SerializeField] private GameObject bubble1 = null;
    [SerializeField] private GameObject bubble2 = null;
    [SerializeField] private GameObject bubble3 = null;
    [SerializeField] private GameObject bubble4 = null;
    private static int topSortingOrder = 100; // start high enough
    private Canvas bubbleCanvas;

    private RectTransform rt;
    private Sequence seq;

    void Awake()
    {
        rt = (RectTransform)transform;
        rt.localScale = Vector3.zero;
        SetAll(false);

        bubbleCanvas = GetComponent<Canvas>();
        if (bubbleCanvas == null)
        {
            bubbleCanvas = gameObject.AddComponent<Canvas>();
        }

        bubbleCanvas.overrideSorting = true; // allow independent sorting
    }
    private void Start()
    {
        BringToFront();
    }

    void OnDisable()
    {
        // Kill any running tweens on this instance
        seq?.Kill();
        rt?.DOKill();
        DOTween.Kill(rt, true);
        DOTween.Kill(gameObject, true);
    }

    void OnDestroy()
    {
        // Extra safety if destroyed externally
        seq?.Kill();
        rt?.DOKill();
        DOTween.Kill(rt, true);
        DOTween.Kill(gameObject, true);
    }

    private void SetAll(bool on)
    {
        if (bubble1) bubble1.SetActive(on);
        if (bubble2) bubble2.SetActive(on);
        if (bubble3) bubble3.SetActive(on);
        if (bubble4) bubble4.SetActive(on);
    }

    public void AnimateBubble()
    {
        // Stop previous animation on THIS instance
        seq?.Kill();
        rt.DOKill();

        // Choose which bubble to show
        SetAll(false);

        if (SlotManager.instance.slotsCard.Count > 0)
        {
            if (SlotManager.instance.GetSlotString() == GameManager.instance.hintWord)
            {
                if (bubble1) bubble1.SetActive(true);
                if (hintTxt) hintTxt.text = GameManager.instance.hintText;
                AnalyticsManager.Instance.TrackHintUsed(FBPlayerData.instance.CURRENT_LEVEL, GameManager.instance.actualHintWord);
            }
            else
            {
                if (bubble2) bubble2.SetActive(true);
            }
        }
        else
        {
            if (GameManager.instance.foundValidWord)
            {
                if (bubble1) bubble1.SetActive(true);
                if (hintTxt) hintTxt.text = GameManager.instance.hintText;
                AnalyticsManager.Instance.TrackHintUsed(FBPlayerData.instance.CURRENT_LEVEL, GameManager.instance.actualHintWord);
            }
            else
            {
                if (CardManager.instance.extraCards.Count <= 0) { if (bubble4) bubble4.SetActive(true); }
                else { if (bubble3) bubble3.SetActive(true); }
            }
        }

        float upY = rt.anchoredPosition.y + 20f;

        seq = DOTween.Sequence()
            .Append(rt.DOScale(1f, 0.25f).SetEase(Ease.OutBack))
            .Append(rt.DOAnchorPosY(upY, 0.3f).SetEase(Ease.InOutSine).SetLoops(4, LoopType.Yoyo))
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                // Guard in case something already killed/destroyed us
                if (this && gameObject) Destroy(gameObject);
            });

        seq.Play();
    }
    public void BringToFront()
    {
        topSortingOrder++;
        bubbleCanvas.sortingOrder = topSortingOrder;

        // Optional: ensure still interactable
        //if (!GetComponent<GraphicRaycaster>())
        //
    }
}
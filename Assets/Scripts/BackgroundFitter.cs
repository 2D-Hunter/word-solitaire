using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundFitter : MonoBehaviour
{
    public enum ScaleMode { Cover, Fit }
    [Header("How the image should fill the screen")]
    public ScaleMode mode = ScaleMode.Cover;

    private Image bgImage;
    private RectTransform rt;
    private Canvas rootCanvas;

    void Awake()
    {
        bgImage = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
        if (bgImage != null) bgImage.preserveAspect = true;

        // Make sure we’re centered so sizeDelta can control actual size
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    void Start() => FitNextFrame();
    void OnEnable() => FitNextFrame();

    // Call this after you change the sprite
    public void ApplySprite(Sprite s)
    {
        if (bgImage == null) return;
        bgImage.sprite = s;
        FitNextFrame();
    }

    public void Fit()
    {
        if (bgImage == null || bgImage.sprite == null || rt == null) return;

        // Use the root canvas rect (more reliable than Screen for UI sizing)
        RectTransform canvasRT = rootCanvas ? rootCanvas.GetComponent<RectTransform>() : null;
        Vector2 targetSize = canvasRT ? canvasRT.rect.size : new Vector2(Screen.width, Screen.height);
        float targetW = targetSize.x;
        float targetH = targetSize.y;

        // Use sprite.rect (pixels), not bounds
        float imgW = bgImage.sprite.rect.width;
        float imgH = bgImage.sprite.rect.height;

        float widthScale = targetW / imgW;
        float heightScale = targetH / imgH;

        float scale = (mode == ScaleMode.Cover)
            ? Mathf.Max(widthScale, heightScale)   // fill, crop extra
            : Mathf.Min(widthScale, heightScale);  // fit inside, no crop

        rt.sizeDelta = new Vector2(imgW * scale, imgH * scale);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
    }

    // Wait one frame so Canvas/Resolution settles before fitting
    private void FitNextFrame() => StartCoroutine(_FitNextFrame());
    private System.Collections.IEnumerator _FitNextFrame()
    {
        yield return null;
        Fit();
    }
}
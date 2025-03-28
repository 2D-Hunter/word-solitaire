using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public RectTransform handRectTransform;
    private float moveDistance = 50f; // Distance to move in X
    private float duration1 = 0.7f;

    private RectTransform cardTransform;
    public RectTransform cardTransform1;
    public RectTransform cardTransform2;  // Assign the card's RectTransform in the Inspector
    private float scaleFactor = 1.2f;     // How much the card scales up
    private float rotationAmount = 12f;   // Rotation angle for zig-zag effect
    private float duration = 0.5f;        // Animation duration

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isAnimating = false;
    public GameObject cardG = null;
    public GameObject cardO = null;
    public GameObject cardF = null;
    public GameObject cardA = null;
    public GameObject cardR = null;
    Vector2 initialPosHand;
    public GameObject infoPanel;
    public GameObject infoPanel2;
    public TextMeshProUGUI infoPanelText;
    public GameObject alphaPatch;


    private void Start()
    {

        handRectTransform.gameObject.SetActive(false);
        infoPanel2.SetActive(false);
        infoPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        Invoke("ShowInfoPanel", 1f);
        cardG.GetComponent<Button>().enabled = false;
        cardO.GetComponent<Button>().enabled = false;
        cardF.GetComponent<Button>().enabled = false;
        cardA.GetComponent<Button>().enabled = false;
        cardR.GetComponent<Button>().enabled = false;
        SetHandPosition(-211f, handRectTransform.anchoredPosition.y);
        if (FBPlayerData.instance.CURRENT_LEVEL == 1 && InitManager.instance.tutorialCntr == 0)
        {
            cardTransform = cardTransform1.GetComponent<RectTransform>();
            cardG.GetComponent<Button>().enabled = true;
            infoPanelText.text = "<b><size=110%>'Tap cards'</size></b> to make words! Make <b><size=110%>'GO'.</size></b>";
        }
        else if (FBPlayerData.instance.CURRENT_LEVEL == 2 && InitManager.instance.tutorialCntr == 0)
        {
            cardTransform = cardTransform1.GetComponent<RectTransform>();
            cardG.GetComponent<Button>().enabled = true;
            infoPanelText.text = "The <b><size=110%>'Draw Pile'</size></b> gives you\nbonus letter cards!\nMake <b><size=110%>'JUG'.</size></b>";
        }
    }
    void ShowInfoPanel()
    {
        infoPanel.GetComponent<RectTransform>().DOScale(0.95f, 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                handRectTransform.gameObject.SetActive(true);
            });
    }

    private void Update()
    {
        //Debug.Log(cardG.GetComponent<Button>().enabled+"_____"+ cardO.GetComponent<Button>().enabled);
    }

    public void AnimateCard()
    {
        if (isAnimating || cardTransform == null) return;
        Debug.Log("Animate Card");
        originalScale = cardTransform.localScale;
        originalRotation = cardTransform.localRotation;
        isAnimating = true;
        cardTransform.SetAsLastSibling();
        // Scale up
        cardTransform.DOScale(originalScale * scaleFactor, duration * 0.5f).SetEase(Ease.OutQuad);

        // Zig-zag rotation
        cardTransform.DORotate(new Vector3(0, 0, rotationAmount), duration * 0.25f)
            .SetEase(Ease.InOutSine)
            .SetLoops(4, LoopType.Yoyo)
            .ChangeStartValue(new Vector3(0, 0, -rotationAmount))
            .OnComplete(() =>
            {
                cardTransform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad);
            });

        cardTransform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.InQuad).SetDelay(0.3f)
            .OnComplete(() =>
            {
                isAnimating = false;
            });
    }
    public void ShowNext()
    {
        isAnimating = false;
        DOTween.KillAll();
        cardTransform.localRotation = Quaternion.Euler(0, 0, 0);
        Debug.Log("InitManager.instance.tutorialCntr: " + InitManager.instance.tutorialCntr);
        if (InitManager.instance.tutorialCntr == 1)
        {
            cardTransform = cardTransform2.GetComponent<RectTransform>();
            cardG.GetComponent<Button>().enabled = false;
            cardO.GetComponent<Button>().enabled = true;
            //handRectTransform.anchoredPosition = new Vector2(-69f, handRectTransform.anchoredPosition.y);
            SetHandPosition(-69f, handRectTransform.anchoredPosition.y);
        }
        else if (InitManager.instance.tutorialCntr == 2)
        {
            cardG.GetComponent<Button>().enabled = false;
            cardO.GetComponent<Button>().enabled = false;
            GameManager.instance.submitBtn.SetActive(true);
            handRectTransform.localRotation = Quaternion.Euler(0, 0, 218f);
            SetHandPosition(-305f, -652f);
            infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            infoPanelText.text = "Now tap the\n<b><size=110%>'Submit Button'.</size></b>";
            cardTransform = null;

        }
        else if (InitManager.instance.tutorialCntr == 3)
        {
            handRectTransform.gameObject.SetActive(false);
            cardF.GetComponent<Button>().enabled = false;
            cardA.GetComponent<Button>().enabled = true;
            cardTransform = cardA.GetComponent<RectTransform>();
        }
        else if (InitManager.instance.tutorialCntr == 4)
        {
            cardA.GetComponent<Button>().enabled = false;
            cardR.GetComponent<Button>().enabled = true;
            cardTransform = cardR.GetComponent<RectTransform>();
        }
        else if (InitManager.instance.tutorialCntr == 5)
        {
            cardA.GetComponent<Button>().enabled = false;
            cardR.GetComponent<Button>().enabled = false;
            cardTransform = null;
            infoPanelText.text = "You've got it!\nNow submit <b><size=110%>'FAR'.</size></b>";
        }

    }

    void SetHandPosition(float xPos, float yPos)
    {
        handRectTransform.anchoredPosition = new Vector2(xPos, yPos);
        initialPosHand = handRectTransform.anchoredPosition;
        StartCoroutine(HandAnimation());
    }
    IEnumerator HandAnimation()
    {
        yield return new WaitForSeconds(0f);

        Debug.Log("initialPosHand: " + initialPosHand);
        handRectTransform.DOAnchorPosX(initialPosHand.x + moveDistance, duration1)
            .SetEase(Ease.InOutSine)  // Smooth motion
            .SetLoops(-1, LoopType.Yoyo);
    }
    IEnumerator HandAnimation2()
    {
        yield return new WaitForSeconds(0f);
        handRectTransform.DOScale(new Vector3(0.9f, 0.9f, 1f), 0.5f)
                  .SetLoops(-1, LoopType.Yoyo) // Infinite loop
                  .SetEase(Ease.InOutSine);
    }
    public void TapGotIt()
    {
        DOTween.KillAll();

        SpriteRenderer sr = alphaPatch.GetComponent<SpriteRenderer>();
        Color newColor = sr.color;
        newColor.a = 0f;
        sr.color = newColor;

        infoPanel2.SetActive(false);
        infoPanel.SetActive(true);
        infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 550);
        infoPanelText.text = "Three cards left to go!\nNow make <b><size=110%>'FAR'.</size></b>";
        handRectTransform.gameObject.SetActive(true);
        handRectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        handRectTransform.anchoredPosition = new Vector2(145f, 29f);
        StartCoroutine(HandAnimation2());
        cardF.GetComponent<Button>().enabled = true;
        cardTransform = cardF.GetComponent<RectTransform>();
        isAnimating = false;
    }
}

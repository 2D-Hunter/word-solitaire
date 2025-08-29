using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Dictionary : MonoBehaviour
{
    public static Dictionary instance;
    public TextMeshProUGUI title = null;
    public TextMeshProUGUI titleShadow = null;
    public TextMeshProUGUI title2 = null;
    public TextMeshProUGUI title2Shadow = null;
    public GameObject loading = null;
    public GameObject makeWords = null;
    public GameObject wordnikIcon = null;
    public TextMeshProUGUI definition = null;

    public CanvasGroup bg = null;
    public CanvasGroup popup = null;
    public RectTransform popupObj = null;
    public TextMeshProUGUI indexText;

    private List<string> displayWords => GameManager.instance.ReversedFoundWords;
    public GameObject prevButton;
    public GameObject nextButton;
    private List<string> currentDisplayWords = new List<string>();

    private Vector2 touchStartPos;
    private bool isSwiping = false;

    private bool isClosing = false;

    private void Awake()
    {
        instance = this;
        bg.alpha = 0;
        popup.alpha = 0;
        popupObj.anchoredPosition = new Vector2(0, -350f);
        makeWords.SetActive(false);
        wordnikIcon.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void Start()
    {
        popupObj.anchoredPosition = new Vector2(0, -350f);

        Debug.Log("Start::::: " + SlotManager.instance.GetSlotString());
        Debug.Log("Start::::: " + GameManager.instance.isValidWord);
        ShowPopup();
    }

    public void ShowPopup()
    {
        isClosing = false; // reset flag when reopening

        if (InitManager.instance.CurrentScene == "Levelup")
        {
            bg.DOFade(0.8f, 0.6f).SetEase(Ease.OutBack);
        }
        else
            bg.DOFade(0.4f, 0.6f).SetEase(Ease.OutBack);

        popup.DOFade(1f, 0.4f).SetEase(Ease.OutBack);
        popupObj.DOAnchorPosY(-70, 0.4f).SetEase(Ease.OutBack);

        List<string> words = new List<string>();
        string slotWord = SlotManager.instance.GetSlotString().ToUpper().Trim();

        bool alreadySubmitted = GameManager.instance.foundWords
            .Exists(w => w.Equals(slotWord, System.StringComparison.OrdinalIgnoreCase));

        if (GameManager.instance.isValidWord && !alreadySubmitted)
        {
            words.Add(slotWord);
        }

        List<string> reversedFound = new List<string>(GameManager.instance.foundWords);
        reversedFound.Reverse();
        words.AddRange(reversedFound);

        currentDisplayWords = words;
        GameManager.instance.currentIndex = 0;

        if (words.Count > 0)
        {
            makeWords.SetActive(false);
            wordnikIcon.SetActive(true);
            UpdatePopup();
        }
        else
        {
            makeWords.SetActive(true);
            wordnikIcon.SetActive(false);
            loading.SetActive(false);
            definition.text = "";
            nextButton.SetActive(false);
            prevButton.SetActive(false);
            indexText.gameObject.SetActive(false);
        }
    }

    public void ClosePopup()
    {
        isClosing = true; // mark for cancellation

        if (InitManager.instance.CurrentScene == "Levelup")
            Invoke("BringStars", 0.3f);
        bg.DOFade(0f, 0.6f).SetEase(Ease.InBack).OnComplete(RemoveThis);
        popup.DOFade(0, 0.4f).SetEase(Ease.InBack);
        popupObj.DOAnchorPosY(-283, 0.4f).SetEase(Ease.InBack);
    }

    void BringStars()
    {
        GameManager.instance.levelupStars.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }

    void RemoveThis()
    {
        if (InitManager.instance.CurrentScene == "Levelup")
        {
            PopupManager.instance.ShowDictionary(PopupManager.instance.dictionaryPopup);
        }
        else
            PopupManager.instance.TogglePopup(PopupManager.instance.dictionaryPopup);
    }

    private void UpdatePopup()
    {
        if (isClosing) return; 

        var words = currentDisplayWords;
        if (words == null || words.Count == 0) return;

        string word = words[GameManager.instance.currentIndex];
        Debug.Log("UpdatePopup Dictionary: " + word);
        AnalyticsManager.Instance.TrackDictionaryOpened(word, FBPlayerData.instance.CURRENT_LEVEL);
        title.text = titleShadow.text = title2.text = title2Shadow.text = word;

        if (GameManager.instance.wordDefinitions.TryGetValue(word, out string def))
        {
            definition.text = def;
            if (loading != null)
                loading.SetActive(false);
        }
        else if (GameManager.instance.definitionsBeingFetched.Contains(word))
        {
            definition.text = "";
            if (loading != null)
                loading.SetActive(true);
        }
        else
        {
            definition.text = "";
            if (loading != null)
                loading.SetActive(true);

            GameManager.instance.definitionsBeingFetched.Add(word);
            string fetchingWord = word; // capture for closure

            WordnikDefinition.instance.FetchDefinition(fetchingWord.ToLower(), (definitionResult) =>
            {
                // if popup is closing, bail
                if (isClosing) return;

                // Always store the definition (normalize casing if needed)
                GameManager.instance.wordDefinitions[fetchingWord] = definitionResult;
                GameManager.instance.definitionsBeingFetched.Remove(fetchingWord);

                // If user has moved to a different word, avoid forcing unexpected UI change
                string currentWord = currentDisplayWords.Count > 0
                    ? currentDisplayWords[GameManager.instance.currentIndex]
                    : "";
                if (!string.Equals(currentWord, fetchingWord, System.StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // Refresh UI for the same word
                UpdatePopup();
            });
        }

        indexText.text = $"{GameManager.instance.currentIndex + 1} / {words.Count}";

        nextButton.SetActive(GameManager.instance.currentIndex < currentDisplayWords.Count - 1);
        prevButton.SetActive(GameManager.instance.currentIndex > 0);
        if (words.Count <= 1)
            indexText.gameObject.SetActive(false);
        else
            indexText.gameObject.SetActive(true);
    }

    public void OnNextPressed()
    {
        if (GameManager.instance.currentIndex < currentDisplayWords.Count - 1)
        {
            GameManager.instance.currentIndex++;
            UpdatePopup();
        }
    }

    public void OnPrevPressed()
    {
        if (GameManager.instance.currentIndex > 0)
        {
            GameManager.instance.currentIndex--;
            UpdatePopup();
        }
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    if (!isSwiping) return;

                    float deltaX = touch.position.x - touchStartPos.x;

                    if (Mathf.Abs(deltaX) > 100f)
                    {
                        if (deltaX < 0)
                            OnNextPressed();
                        else
                            OnPrevPressed();
                    }

                    isSwiping = false;
                    break;
            }
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            float deltaX = Input.mousePosition.x - touchStartPos.x;

            if (Mathf.Abs(deltaX) > 100f)
            {
                if (deltaX < 0)
                    OnNextPressed();
                else
                    OnPrevPressed();
            }

            isSwiping = false;
        }
#endif
    }
}
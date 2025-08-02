using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterTutorial : MonoBehaviour
{
    public Button settingBtn;
    public Button dictionaryBtn;
    public Button hintBtn;
    public RectTransform handRectTransform;
    public Card card;
    ButtonEffect settingBtnEffect;
    ButtonEffect dictionaryBtnEffect;
    ButtonEffect hintBtnEffect;
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Booster Tutorial");
        if(FBPlayerData.instance.CURRENT_LEVEL == 11)
        {
            settingBtnEffect = settingBtn.GetComponent<ButtonEffect>();
            dictionaryBtnEffect = dictionaryBtn.GetComponent<ButtonEffect>();
            hintBtnEffect = hintBtn.GetComponent<ButtonEffect>();


            settingBtn.enabled = false;
            dictionaryBtn.enabled = false;
            hintBtn.enabled = false;
            settingBtnEffect.DisableEffect();
            dictionaryBtnEffect.DisableEffect();
            hintBtnEffect.DisableEffect();
        }
        

        card = GameObject.Find("Card").GetComponent<Card>();
        
    }
    void Start()
    {
        StartCoroutine(HandAnimation());
        if (FBPlayerData.instance.CURRENT_LEVEL == 11)
        {
            CardManager.instance.DisableAllCards();
        }
    }

    IEnumerator HandAnimation()
    {
        yield return new WaitForSeconds(0f);
        handRectTransform.DOScale(new Vector3(0.9f, 0.9f, 1f), 0.5f)
                  .SetLoops(-1, LoopType.Yoyo) // Infinite loop
                  .SetEase(Ease.InOutSine);
    }
    public void StopHandAnim()
    {
        if (FBPlayerData.instance.CURRENT_LEVEL == 11)
        {
            CardManager.instance.EnableAllCards();
            settingBtnEffect.EnableEffect();
            dictionaryBtnEffect.EnableEffect();
            hintBtnEffect.EnableEffect();

            settingBtn.enabled = true;
            dictionaryBtn.enabled = true;
            hintBtn.enabled = true;
            handRectTransform?.DOKill();
            handRectTransform.gameObject.SetActive(false);
        }
    }
}

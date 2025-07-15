using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryButton : MonoBehaviour
{
    public static DictionaryButton instance;
    public Button myButton;
    public string firstImage = "DictionaryIcon-1";
    public string secondImage = "DictionaryIcon-2";
    private void Start()
    {
        Debug.Log("________DictionaryButton: "+ MultiplayerEventHandler.Instance.isMultiplayer);
        //if (MultiplayerEventHandler.Instance.isMultiplayer)
            gameObject.SetActive(false);
        instance = this;
    }
    public void SwapImage()
    {
        if (MultiplayerEventHandler.Instance.isMultiplayer)
        {
            return;
        }
            string imageToLoad = GameManager.instance.isValidWord ? secondImage : firstImage;
        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
    }
}

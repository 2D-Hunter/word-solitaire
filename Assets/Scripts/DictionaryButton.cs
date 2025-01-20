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
        instance = this;
    }
    public void SwapImage()
    {
        string imageToLoad = GameManager.instance.isValidWord ? secondImage : firstImage;
        Sprite loadedSprite = Resources.Load<Sprite>(imageToLoad);
        if (loadedSprite != null)
            myButton.image.sprite = loadedSprite;
    }
}

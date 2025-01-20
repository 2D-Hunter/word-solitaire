using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject dictionary = null;
    public bool isValidWord = false;
    public int totalPoint = 0;
    public GameObject backButton;
    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        instance = this;
    }

    public void ShowDictionary()
    {
        dictionary.SetActive(true);
        Dictionary.instance.ShowPopup();
    }
    public void ShowBackButton()
    {
        if (!backButton.activeSelf)
        {
            backButton.SetActive(true);
            BackButton.instance.ShowThis();
        }

    }
}

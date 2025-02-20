using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;

    public GameObject goalPopup;

    private GameObject currentPopup;
    public RectTransform uiContainer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        AssignUIContainer();
    }

    void AssignUIContainer()
    {
        uiContainer = GameObject.Find("UI-Panel").GetComponent<RectTransform>();
    }


    public void TogglePopup(GameObject prefab)
    {
        if(currentPopup == null)
        {
            currentPopup = Instantiate(prefab, uiContainer);
        }
        else
        {
            Destroy(prefab);
            prefab = null;
        }
    }
}

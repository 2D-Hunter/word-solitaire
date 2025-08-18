using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialAlphaPatch : MonoBehaviour
{
    public Tutorial tutorial;
    public GameObject cardUIObject; // Assign your UI card here
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Start()
    {
        if (tutorial == null)
            tutorial = FindObjectOfType<Tutorial>();

        if (eventSystem == null)
            eventSystem = EventSystem.current;

        //if (raycaster == null)
        //    raycaster = FindObjectOfType<GraphicRaycaster>();
    }

    //void OnMouseDown() // Detects click on SpriteRenderer with Collider
    //{
    //    Debug.Log(gameObject.name + " was clicked!");

    //    if (gameObject.CompareTag("BlackPatch"))
    //    {
    //        Debug.Log("Black Patch Clicked! Start Animation.");
    //        // Call your animation function here
    //    }
    //    if (tutorial != null)
    //    {
    //        tutorial.AnimateCard(); // Play the animation
    //    }
    //}
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GetMouseButtonDown InitManager.instance.tutorialCntr: " + InitManager.instance.tutorialCntr);

            if (tutorial == null)
                return;

            int level = FBPlayerData.instance.CURRENT_LEVEL;
            int step = InitManager.instance.tutorialCntr;

            Dictionary<(int level, int step), GameObject> tutorialSteps = new Dictionary<(int, int), GameObject>()
            {
                { (1, 0), tutorial.cardG },
                { (1, 1), tutorial.cardO },
                { (1, 2), tutorial.cardF },
                { (1, 3), tutorial.cardA },
                { (1, 4), tutorial.cardR },
                { (2, 0), tutorial.cardJ },
                { (2, 1), tutorial.cardU },
                { (2, 2), tutorial.cardG1 }
            };

            if (tutorialSteps.TryGetValue((level, step), out GameObject currentCard) && !ClickedOnUIObject(currentCard))
            {
                tutorial.AnimateCard();
            }
            else
            {
                Debug.Log("Card was clicked. Do nothing.");
            }
        }
    }

    bool ClickedOnUIObject(GameObject target)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == target)
                return true;
        }

        return false;
    }

}

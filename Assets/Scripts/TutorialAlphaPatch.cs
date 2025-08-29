using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialAlphaPatch : MonoBehaviour
{
    public Tutorial tutorial;

    void Start()
    {
        if (tutorial == null) tutorial = FindObjectOfType<Tutorial>();
    }

    void Update()
    {
        if (!PointerDown()) return;
        if (tutorial == null || EventSystem.current == null) return;

        int level = FBPlayerData.instance.CURRENT_LEVEL;
        int step = InitManager.instance.tutorialCntr;

        // Map (level, step) -> target card GameObject
        var tutorialSteps = new Dictionary<(int, int), GameObject>
        {
            { (1, 0), tutorial.cardG },
            { (1, 1), tutorial.cardO },
            { (1, 2), tutorial.cardF },
            { (1, 3), tutorial.cardA },
            { (1, 4), tutorial.cardR },
            { (2, 0), tutorial.cardJ },
            { (2, 1), tutorial.cardU },
            { (2, 2), tutorial.cardG1 },
        };

        if (!tutorialSteps.TryGetValue((level, step), out var currentCard) || currentCard == null)
            return;

        // Topmost UI under the pointer
        var top = GetTopmostUIUnderPointer();
        bool tappedTarget = IsTargetOrChild(top, currentCard);

        if (!tappedTarget)
        {
            // Wrong tap → shake
            tutorial.AnimateCard();
        }
        else
        {
            // Correct tap → do nothing here; let the card’s own click logic run
            // (Button/SlotManager/etc.)
        }
    }

    static bool PointerDown()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetMouseButtonDown(0);
#else
        if (Input.touchCount == 0) return false;
        return Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    static Vector2 PointerPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#else
        return (Input.touchCount > 0) ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;
#endif
    }

    static GameObject GetTopmostUIUnderPointer()
    {
        var es = EventSystem.current;
        var ped = new PointerEventData(es) { position = PointerPosition() };
        var results = new List<RaycastResult>();
        es.RaycastAll(ped, results);
        return results.Count > 0 ? results[0].gameObject : null; // topmost
    }

    static bool IsTargetOrChild(GameObject hit, GameObject target)
    {
        if (hit == null || target == null) return false;
        if (hit == target) return true;
        return hit.transform.IsChildOf(target.transform);
    }
}
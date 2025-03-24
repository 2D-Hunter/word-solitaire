using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAlphaPatch : MonoBehaviour
{
    public Tutorial tutorial; // Reference to the card script

    void Start()
    {
        if (tutorial == null)
            tutorial = FindObjectOfType<Tutorial>(); // Auto-assign
    }

    void OnMouseDown() // Detects click on SpriteRenderer with Collider
    {
        Debug.Log(gameObject.name + " was clicked!");

        if (gameObject.CompareTag("BlackPatch"))
        {
            Debug.Log("Black Patch Clicked! Start Animation.");
            // Call your animation function here
        }
        if (tutorial != null)
        {
            tutorial.AnimateCard(); // Play the animation
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectingToServer : MonoBehaviour
{
    public TextMeshProUGUI connectingText; // Replace with TMPro.TMP_Text if using TextMeshPro
    public float dotSpeed = 0.5f; // Time between dot changes

    private void Start()
    {
        StartCoroutine(AnimateDots());
    }

    private IEnumerator AnimateDots()
    {
        string baseText = "Connecting to server";
        int dotCount = 0;

        while (true)
        {
            connectingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Cycle between 0 and 3 dots
            yield return new WaitForSeconds(dotSpeed);
        }
    }
}

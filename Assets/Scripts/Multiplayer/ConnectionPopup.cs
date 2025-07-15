using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionPopup : MonoBehaviour
{
    public TextMeshProUGUI txt = null;

    public void SetText(string str)
    {
        txt.text = str;
    }
}

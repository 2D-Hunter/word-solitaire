using UnityEngine;
using UnityEngine.UI; // For UI Text (remove if using TextMesh)
using TMPro;

public class CardData : MonoBehaviour
{
    public TextMeshProUGUI letterTxt;
    public char letter; // Letter assigned to the card

    void Start()
    {
        // Assign a random letter from A to Z
        letter = (char)Random.Range('A', 'Z' + 1);

        //// Find and update the text component on the card
        //Text textComponent = GetComponentInChildren<Text>(); // For UI Text
        //if (textComponent != null)
        //{
        //    textComponent.text = letter.ToString();
        //}

        //// For 3D TextMesh (if used instead of UI Text)
        //TextMesh textMesh = GetComponentInChildren<TextMesh>();
        //if (textMesh != null)
        //{
        //    textMesh.text = letter.ToString();
        //}
        letterTxt.text = letter.ToString();

        Debug.Log($"{gameObject.name} assigned letter: {letter}");
    }
}
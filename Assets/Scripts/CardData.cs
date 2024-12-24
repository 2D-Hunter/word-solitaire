using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // For UI Text (remove if using TextMesh)
using TMPro;
[ExecuteAlways] // Ensures the script runs in edit mode
public class CardData : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI valueText;
    public char letter; // Letter assigned to the card
    public int value; // Value assigned to the card

    private static readonly Dictionary<char, int> letterValues = new Dictionary<char, int>
    {
        { 'A', 1 }, { 'B', 3 }, { 'C', 3 }, { 'D', 2 },
        { 'E', 1 }, { 'F', 4 }, { 'G', 2 }, { 'H', 4 },
        { 'I', 1 }, { 'J', 8 }, { 'K', 5 }, { 'L', 1 },
        { 'M', 3 }, { 'N', 1 }, { 'O', 1 }, { 'P', 3 },
        { 'Q', 10 }, { 'R', 1 }, { 'S', 1 }, { 'T', 1 },
        { 'U', 1 }, { 'V', 4 }, { 'W', 4 }, { 'X', 8 },
        { 'Y', 4 }, { 'Z', 10 }
    };
    public int GetCardValue(char letter)
    {
        char uppercaseLetter = char.ToUpper(letter);
        if (letterValues.TryGetValue(uppercaseLetter, out int value))
        {
            return value;
        }
        return 0; // Default value for invalid characters
    }

    //private static readonly int[] letterValues =
    //{
    //    1, 3, 3, 2, 1, 4, 2, 4, 1, 8,
    //    5, 1, 3, 1, 1, 3, 10, 1, 1, 1,
    //    1, 4, 4, 8, 4, 10
    //};
    //int GetCardValue(char letter)
    //{
    //    char uppercaseLetter = char.ToUpper(letter);
    //    int index = uppercaseLetter - 'A'; // Convert 'A'-'Z' to 0-25
    //    if (index >= 0 && index < letterValues.Length)
    //    {
    //        return letterValues[index];
    //    }
    //    return 0; // Default value for invalid characters
    //}

    private void OnValidate()
    {
        letterText.text = letter.ToString();
        valueText.text = GetCardValue(letter).ToString();
    }
    void Start()
    {
        letterText.text = letter.ToString();
        valueText.text = GetCardValue(letter).ToString();

        Debug.Log($"{gameObject.name} assigned letter: {letter}");
    }
}
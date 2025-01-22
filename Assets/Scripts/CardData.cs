using UnityEngine;
using System.Collections.Generic;
using TMPro;
//[ExecuteAlways]
public class CardData : MonoBehaviour
{
    //public static CardData instance;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI valueText;
    public char letter;
    public int cardValue = 0;
    public int value;

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
    private void Start()
    {
        if (gameObject.tag == "ExtraCard")
        {
            char randomLetter = (char)('A' + Random.Range(0, 26));
            letterText.text = randomLetter.ToString();
            valueText.text = GetCardValue(randomLetter).ToString();
            cardValue = GetCardValue(randomLetter);
        }
        else
        {
            cardValue = GetCardValue(letter);
        }
        var card = GetComponent<Card>();
        card.cardData = this;

    }
    public int GetCardValue(char letter)
    {
        char uppercaseLetter = char.ToUpper(letter);
        if (letterValues.TryGetValue(uppercaseLetter, out int value))
        {
            return value;
        }
        return 0;
    }

    private void OnValidate()
    {
        //if (Card.instance.gameObject.tag != "ExtraCard")
        //{
            letterText.text = letter.ToString();
            valueText.text = GetCardValue(letter).ToString();
        //}
    }
    //void Start()
    //{
    //    Debug.Log("Card");
    //    instance = this;
    //    letterText.text = letter.ToString();
    //    valueText.text = GetCardValue(letter).ToString();
    //    cardValue = GetCardValue(letter);
    //    Debug.Log($"{gameObject.name} assigned letter: {letter} assigned value: {cardValue}");
    //}
    //public int GetCardValue(char letter)
    //{
    //    char uppercaseLetter = char.ToUpper(letter);
    //    if (letterValues.TryGetValue(uppercaseLetter, out int value))
    //    {
    //        return value;
    //    }
    //    return 0;
    //}

    //void Start()
    //{
    //    Debug.Log("Card");
    //    instance = this;
    //    char randomLetter = (char)('A' + Random.Range(0, 26));
    //    letterText.text = randomLetter.ToString();
    //    valueText.text = GetCardValue(randomLetter).ToString();
    //    cardValue = GetCardValue(randomLetter);
    //    Debug.Log($"{gameObject.name} assigned letter: {randomLetter} assigned value: {cardValue}");
    //}
}
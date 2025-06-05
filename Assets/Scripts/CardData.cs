using UnityEngine;
using System.Collections.Generic;
using TMPro;

//[ExecuteAlways]
public class CardData : MonoBehaviour
{
    //public static CardData instance;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI valueText;
    private List<char> letters = new List<char> { 'A', 'C', 'P', 'T', 'O', 'S', 'A', 'S', 'R', 'J' };
    public char letter
    {
        get { return letterText.text[0]; }
        set
        {
            letterText.text = value.ToString();
        }
    }
    public int cardValue = 0;
    public int value;

    private static readonly Dictionary<char, int> letterValues = new Dictionary<char, int>
    {
        { 'A', 1 }, { 'B', 4 }, { 'C', 4 }, { 'D', 3 },
        { 'E', 1 }, { 'F', 4 }, { 'G', 4 }, { 'H', 3 },
        { 'I', 1 }, { 'J', 8 }, { 'K', 5 }, { 'L', 2 },
        { 'M', 3 }, { 'N', 2 }, { 'O', 1 }, { 'P', 3 },
        { 'Q', 10 }, { 'R', 1 }, { 'S', 2 }, { 'T', 1 },
        { 'U', 3 }, { 'V', 4 }, { 'W', 5 }, { 'X', 8 },
        { 'Y', 4 }, { 'Z', 10 }
    };
    private void Start()
    {
        Debug.Log("____Card Data111");
        if (gameObject.tag == "ExtraCard")
        {
            Debug.Log("____Card Data222");
            if (FBPlayerData.instance.CURRENT_LEVEL == 2)
            {
                Debug.Log("gameobject.name: "+gameObject.name);
                //Debug.Log("Before removal: " + string.Join(", ", InitManager.instance.letters));
                if(gameObject.name == "Card")
                    letterText.text = letters[0].ToString();
                else if (gameObject.name == "Card (1)")
                    letterText.text = letters[1].ToString();
                else if (gameObject.name == "Card (2)")
                    letterText.text = letters[2].ToString();
                else if (gameObject.name == "Card (3)")
                    letterText.text = letters[3].ToString();
                else if (gameObject.name == "Card (4)")
                    letterText.text = letters[4].ToString();
                else if (gameObject.name == "Card (5)")
                    letterText.text = letters[5].ToString();
                else if (gameObject.name == "Card (6)")
                    letterText.text = letters[6].ToString();
                else if (gameObject.name == "Card (7)")
                    letterText.text = letters[7].ToString();
                else if (gameObject.name == "Card (8)")
                    letterText.text = letters[8].ToString();
                else if (gameObject.name == "Card (9)")
                    letterText.text = letters[9].ToString();
            }
            else
            {
                char randomLetter = (char)('A' + Random.Range(0, 26));
                letterText.text = randomLetter.ToString();
                valueText.text = GetCardValue(randomLetter).ToString();
                cardValue = GetCardValue(randomLetter);
            }
            
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
            Debug.Log("________GetCardValue: "+value);
            return value;
        }
        return 0;
    }

    //private void OnValidate()
    //{
    //    //if (Card.instance.gameObject.tag != "ExtraCard")
    //    //{
    //    Debug.Log("________OnValidate");
    //    letterText.text = letter.ToString();
    //    valueText.text = GetCardValue(letter).ToString();
    //}

    
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
    int GetCardIndexFromName(string name)
    {
        if (name == "Card") return 0;

        // Example: "Card (1)" -> 1
        if (name.StartsWith("Card ("))
        {
            string num = name.Substring(6, name.Length - 7); // Extract the number inside parentheses
            if (int.TryParse(num, out int index))
                return index;
        }

        return -1; // Invalid name format
    }
}
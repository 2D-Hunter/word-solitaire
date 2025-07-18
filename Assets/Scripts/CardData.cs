using UnityEngine;
using System.Collections.Generic;
using TMPro;

//[ExecuteAlways]
public class CardData : MonoBehaviour
{
    //public static CardData instance;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI valueText;
    public static List<char> sharedRandomLetters;
    public static List<List<char>> letterBatches = new List<List<char>>();
    [HideInInspector]
    public int cardIndex = -1;

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
        { 'U', 3 }, { 'V', 5 }, { 'W', 5 }, { 'X', 8 },
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
                int index = cardIndex >= 0 ? cardIndex : GetCardIndexFromName(gameObject.name);
                if (index < 0)
                {
                    Debug.LogWarning("Invalid card index from name: " + gameObject.name);
                    return;
                }

                int batchSize = 10; // or 5 if you're spawning in 5s
                int batchIndex = index / batchSize;
                int localIndex = index % batchSize;

                // Ensure enough batches exist
                while (letterBatches.Count <= batchIndex)
                {
                    List<char> newBatch = GenerateAtLeastFiveVowels(batchSize);
                    letterBatches.Add(newBatch);
                    Debug.Log($"Generated batch {letterBatches.Count - 1}: " + string.Join(", ", newBatch));
                }

                List<char> targetBatch = letterBatches[batchIndex];

                if (localIndex >= 0 && localIndex < targetBatch.Count)
                {
                    char letter = targetBatch[localIndex];
                    letterText.text = letter.ToString();
                    valueText.text = GetCardValue(letter).ToString();
                    cardValue = GetCardValue(letter);
                }
                else
                {
                    Debug.LogWarning($"Invalid local index {localIndex} for batch {batchIndex}");
                }
            }

        }
        else
        {
            cardValue = GetCardValue(letter);
        }
        var card = GetComponent<Card>();
        card.cardData = this;

    }
    //List<char> GenerateAtLeastFiveVowels(int totalCards)
    //{
    //    string vowels = "AEIOU";
    //    string consonants = "BCDFGHJKLMNPQRSTVWXYZ";
    //    List<char> result = new List<char>();

    //    // Ensure 5 vowels
    //    for (int i = 0; i < 5; i++)
    //        result.Add(vowels[Random.Range(0, vowels.Length)]);

    //    // Fill remaining with random letters (vowels + consonants)
    //    string allLetters = vowels + consonants;
    //    for (int i = 5; i < totalCards; i++)
    //        result.Add(allLetters[Random.Range(0, allLetters.Length)]);

    //    // Shuffle
    //    for (int i = result.Count - 1; i > 0; i--)
    //    {
    //        int j = Random.Range(0, i + 1);
    //        (result[i], result[j]) = (result[j], result[i]);
    //    }

    //    return result;
    //}
    List<char> GenerateAtLeastFiveVowels(int totalCards, float difficulty = 0f)
    {
        string vowels = "AEIOU";
        string consonants = "BCDFGHJKLMNPQRSTVWXYZ";
        string consonants2 = "BCDAFGHJKELMNPQRISTVWXOYZU";
        string consonants3 = "BCADFGHJEKLMANPOQRSTUVWXYZI";

        List<char> result = new List<char>();

        difficulty = Mathf.Clamp01(difficulty);

        // For 10 cards, map difficulty to 5–2 vowels (easy → hard)
        int vowelCount = Mathf.RoundToInt(Mathf.Lerp(5, 2, difficulty));
        if(vowelCount < 3)
        {
            consonants = consonants3;
        }
        if(vowelCount < 4)
        {
            consonants = consonants2;
        }
        int consonantCount = totalCards - vowelCount;

        // Add vowels
        for (int i = 0; i < vowelCount; i++)
            result.Add(vowels[Random.Range(0, vowels.Length)]);

        // Add consonants
        for (int i = 0; i < consonantCount; i++)
            result.Add(consonants[Random.Range(0, consonants.Length)]);

        // Shuffle the result
        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result;
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

    //___________

    //public static class SmartLetterGenerator
    //{
    //    private static readonly string vowels = "AEIOU";
    //    private static readonly string weightedPool = "EEEAAARRRNNNTTTLLSSSIIIOOUDGBCMPFHVWYJKXZQ";

    //    public static List<char> GenerateHelpfulLetters(int count = 5)
    //    {
    //        List<char> letters = new List<char>();

    //        // Always inject at least 1 vowel
    //        letters.Add(vowels[Random.Range(0, vowels.Length)]);

    //        // Fill rest from weighted pool
    //        for (int i = 1; i < count; i++)
    //        {
    //            letters.Add(weightedPool[Random.Range(0, weightedPool.Length)]);
    //        }

    //        // Shuffle so vowel isn't always first
    //        Shuffle(letters);

    //        return letters;
    //    }

    //    private static void Shuffle(List<char> list)
    //    {
    //        for (int i = list.Count - 1; i > 0; i--)
    //        {
    //            int j = Random.Range(0, i + 1);
    //            (list[i], list[j]) = (list[j], list[i]);
    //        }
    //    }
    //}
    //List<char> moreLetters = SmartLetterGenerator.GenerateHelpfulLetters(5);
    //int nextLetterIndex = 0;

    //public void RevealNextLetter()
    //{
    //    if (nextLetterIndex < moreLetters.Count)
    //    {
    //        char next = moreLetters[nextLetterIndex];
    //        nextLetterIndex++;

    //        // Add letter to player hand
    //        AddLetterToHand(next);
    //    }
    //}

    //___________

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

        if (name.StartsWith("Card ("))
        {
            string num = name.Substring(6, name.Length - 7);
            if (int.TryParse(num, out int index))
                return index;
        }

        Debug.LogWarning("Card name format invalid: " + name);
        return -1;
    }
}
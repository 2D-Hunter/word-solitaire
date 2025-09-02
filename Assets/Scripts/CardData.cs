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
        if (FBPlayerData.instance.CURRENT_LEVEL <= 2)
        {
            valueText.text = GetCardValue(letter).ToString();
            cardValue = GetCardValue(letter);
        }
        if (gameObject.tag == "ExtraCard")
        {
            Debug.Log("____Card Data222");
            if (FBPlayerData.instance.CURRENT_LEVEL == 2)
            {
                Debug.Log("gameobject.name: " + gameObject.name);
                //Debug.Log("Before removal: " + string.Join(", ", InitManager.instance.letters));
                if (gameObject.name == "Card")
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
                Debug.Log("BoardManager.instance.levelDifficulty::: " + BoardManager.instance.levelDifficulty);

                string letter = WordLetterGenerationSystem.Instance.GenerateLetter(
                    BoardManager.instance.levelDifficulty,
                    BagType.DrawPileLetterBag
                );
                Debug.Log("Letter on the card: " + letter);
                // ✅ Prevent wildcards before level 15
                if (letter == "*" && FBPlayerData.instance.CURRENT_LEVEL < 15)
                {
                    // Re-roll until it's not "*"
                    do
                    {
                        letter = WordLetterGenerationSystem.Instance.GenerateLetter(
                            BoardManager.instance.levelDifficulty,
                            BagType.DrawPileLetterBag
                        );
                    } while (letter == "*");
                }

                var card = GetComponent<Card>();

                if (letter == "*")
                {
                    valueText.text = "4";
                    cardValue = 4;
                    card.isWildCard = true;
                    card.isFaceUp = false;
                    card.transform.GetChild(2).gameObject.SetActive(true);
                    card.transform.GetChild(3).gameObject.SetActive(false);

                    if (CardManager.instance.rightSideCards.Count > 0)
                    {
                        CardManager.instance.rightSideCards[0].isFaceUp = true;
                        card.transform.GetChild(2).gameObject.SetActive(false);
                        card.transform.GetChild(3).gameObject.SetActive(true);
                    }
                }
                else
                {
                    int tmpCardValue = GetCardValue(letter[0]);
                    cardValue = tmpCardValue;
                    valueText.text = cardValue.ToString(); // 🔥 fix: use cardValue not default 0
                    letterText.text = letter;
                }
            }

        }
        else
        {
            //cardValue = GetCardValue(letter);
        }
        //var card = GetComponent<Card>();
        //card.cardData = this;

    }

    public int GetCardValue(char letter)
    {
        char uppercaseLetter = char.ToUpper(letter);
        if (letterValues.TryGetValue(uppercaseLetter, out int value))
        {
            Debug.Log("________GetCardValue: uppercaseLetter" + uppercaseLetter);
            Debug.Log("________GetCardValue: " + value);
            return value;
        }
        return 0;
    }

    public List<char> GenerateHelpfulLetters(int count = 10)
    {
        Debug.Log("GenerateHelpfulLetters");
        string vowels = "AEIOU";
        string weightedPool = "EEEAAARRRNNNTTTLLSSSIIIOOUDGBCMPFHVWYJKXZQ";

        List<char> letters = new List<char>();

        // Ensure at least one vowel
        letters.Add(vowels[Random.Range(0, vowels.Length)]);

        // Fill the rest with letters from the weighted pool
        for (int i = 1; i < count; i++)
        {
            letters.Add(weightedPool[Random.Range(0, weightedPool.Length)]);
        }

        Shuffle(letters);
        return letters;
    }

    /// <summary>
    /// Shuffles a list of characters in-place using the Fisher-Yates algorithm.
    /// </summary>
    private void Shuffle(List<char> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
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

    //   •	Level 1 → difficulty = 0.01 (easy)
    //•	Level 50 → difficulty = 0.5 (medium)
    //•	Level 100+ → difficulty = 1.0 (hard)

    //public List<char> GenerateHelpfulLetters(int count, float difficulty = 0f)
    //{
    //    string vowels = "AEIOU";
    //    string consonantsEasy = "BCDFGHJKLMNPQRSTVWXYZ";
    //    string consonantsMed = "BCDAFGHJKELMNPQRISTVWXOYZU";
    //    string consonantsHard = "BCADFGHJEKLMANPOQRSTUVWXYZI";

    //    difficulty = Mathf.Clamp01(difficulty); // Ensure range [0, 1]

    //    // Map difficulty to vowel count (e.g., 5 → 2)
    //    int vowelCount = Mathf.RoundToInt(Mathf.Lerp(count >= 10 ? 5 : 3, 2, difficulty));
    //    int consonantCount = count - vowelCount;

    //    // Choose harder consonants based on difficulty
    //    string consonantPool = consonantsEasy;
    //    if (difficulty > 0.5f) consonantPool = consonantsMed;
    //    if (difficulty > 0.75f) consonantPool = consonantsHard;

    //    List<char> result = new List<char>();

    //    // Add vowels
    //    for (int i = 0; i < vowelCount; i++)
    //        result.Add(vowels[Random.Range(0, vowels.Length)]);

    //    // Add consonants
    //    for (int i = 0; i < consonantCount; i++)
    //        result.Add(consonantPool[Random.Range(0, consonantPool.Length)]);

    //    // Shuffle
    //    Shuffle(result);
    //    return result;
    //}
    //float difficulty = Mathf.Clamp01(FBPlayerData.instance.CURRENT_LEVEL / 100f);
    //List<char> newBatch = GenerateHelpfulLetters(batchSize, difficulty);
}
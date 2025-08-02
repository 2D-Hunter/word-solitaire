using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;


public class ObjectPoolCard<T>
{
    private readonly Func<T> m_CreateFunc;
    private readonly Action<T> m_ActionOnGet;
    private readonly Action<T> m_ActionOnRelease;
    private readonly Stack<T> m_Stack = new Stack<T>();

    public ObjectPoolCard(Func<T> createFunc, Action<T> actionOnGet, Action<T> actionOnRelease)
    {
        m_CreateFunc = createFunc;
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element = m_Stack.Count == 0 ? m_CreateFunc() : m_Stack.Pop();
        m_ActionOnGet?.Invoke(element);
        return element;
    }

    public void Release(T element)
    {
        m_ActionOnRelease?.Invoke(element);
        m_Stack.Push(element);
    }
}

public class BoardManager : MonoBehaviour
{
    [Header("Level Data Settings")]
    public string levelRampFileName = "Random_RampTest1_Level6"; // Name of the JSON file (without .json extension)
    [Header("Visual Settings")]
    public float layoutScaleX = 100f;
    [Header("Visual Settings")]
    public float layoutScaleY = 100f;

    // Make sure this JSON file is in Assets/Resources/Levels/

    [Header("References")]
    public GameObject tilePrefab; // Assign your Tile_Prefab here
    public Transform boardParent; // An empty GameObject to hold all tiles
    [Tooltip("Additional offset to position the entire board on the canvas.")]
    public Vector2 boardOriginOffset = Vector2.zero; // Tweak this for overall centering

    
    [SerializeField]
    private List<Card> activeCards = new List<Card>();
    public bool collectionChecks = true;
    public int maxPoolSize = 10;

    ObjectPoolCard<GameObject> m_Pool;
    private int topLevel = -1;
    private LevelRamp loadedLevelRampData = null;
    [SerializeField]
    private int currentLevel = 0;
    LetterBucket letterBucket = null;
    public ObjectPoolCard<GameObject> Pool
    {
        get
        {
            if (m_Pool == null)
            {

                m_Pool = new ObjectPoolCard<GameObject>(
                createFunc: () =>
                {
                    var gameObjectTile = Instantiate(tilePrefab);
                    // gameObjectTile.AddComponent<TileData>();
                    return gameObjectTile;


                },
                actionOnGet: (go) =>
                {
                    go.SetActive(true);
                },
                actionOnRelease: (go) =>
                {
                    go.SetActive(false);
                    var card = go.GetComponent<Card>();
                    card.isFaceUp = false;
                    card.cardFace.SetActive(true);
                    if (card != null) {
                        card.belowCards.Clear();


                    }
                }
            );

            }
            return m_Pool;
        }
    }

    public static BoardManager instance;
    public static BoardManager Instance { get { return instance; }  }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
      
        if (loadedLevelRampData == null)
        {
            LoadLevelDataFromJson(levelRampFileName);
        }

     


    }

    void LoadLevelDataFromJson(string fileRampName)
    {
        // Adjust path if your JSONs are in a different Resources subfolder (e.g., "Levels/")

        string fullPathRamp = "Config/" + fileRampName;

        TextAsset jsonTextAsset = Resources.Load<TextAsset>(fullPathRamp);

        if (jsonTextAsset == null)
        {
            Debug.LogError($"JSON level file not found at: Resources/{fullPathRamp}");
            return;
        }

        Debug.Log($"Loading level from JSON: {fullPathRamp}");
        loadedLevelRampData = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelRamp>(jsonTextAsset.text);
       
        Debug.Log($"Loading level from JSON: {loadedLevelRampData.Id}");
        Debug.Log($"Loading level from JSON: {loadedLevelRampData.Levels.Count}");
        if (loadedLevelRampData == null)
        {
            Debug.LogError("Failed to deserialize JSON level data. >>>>>>>");
            return;
        }

        string letterBucketPath = "Config/letterbucket";


        TextAsset jsonLetterBucketTextAsset = Resources.Load<TextAsset>(letterBucketPath);

        if (jsonLetterBucketTextAsset == null)
        {
            Debug.LogError($"JSON letter Bucket file not found at: Resources/{letterBucketPath}");
            return;
        }
        letterBucket = Newtonsoft.Json.JsonConvert.DeserializeObject<LetterBucket>(jsonLetterBucketTextAsset.text);
        if (letterBucket == null)
        {
            Debug.LogError("Failed to deserialize JSON letterBucket data. >>>>>>>");
            return;
        }

        GenerateLevelByNumber(currentLevel);

    }

    public void NextLevel()
    {
        currentLevel++;
        GenerateLevelByNumber(currentLevel);
    }

    public void PrevousLevel()
    {
        currentLevel--;
        GenerateLevelByNumber(currentLevel);
    }

    public void GenerateLevelByNumber(int LevelNumber)
    {
      
        if (loadedLevelRampData == null)
        {
            Debug.LogError("Failed to deserialize JSON level data.");
            return;
        }
        string LevelfullPath = "Levels/" + loadedLevelRampData.Levels[LevelNumber];

        TextAsset jsonTextAssetLevel = Resources.Load<TextAsset>(LevelfullPath);

        if (jsonTextAssetLevel == null)
        {
            Debug.LogError($"JSON level file not found at: Resources/{LevelfullPath}");
            return;
        }

        Debug.Log($"Loading level from JSON: {LevelfullPath}");
        GameLevelData loadedLevelData = JsonUtility.FromJson<GameLevelData>(jsonTextAssetLevel.text);

        if (loadedLevelData == null)
        {
            Debug.LogError("Failed to deserialize JSON level data.");
            return;
        }
        GenerateBoardFromLevelData(loadedLevelData);
    }
    private void GenerateBoardFromLevelData(GameLevelData levelData)
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            var activeCard = activeCards[i];

            Pool.Release(activeCard.gameObject);
        }
        activeCards.Clear();


        var cardcount = levelData.Layout.FindAll(tile => tile.Tile == "?").Count;
        int difficulty = loadedLevelRampData.Difficulties[currentLevel];
        string letters = letterBucket.DefficultiMapLetterBucket[difficulty];
        var generateLetter = GenerateAtLeastFiveVowels(levelData.Layout.Count, letters, difficulty);

        for (var rowPairIndex = 0; rowPairIndex < levelData.Layout.Count; rowPairIndex++)
        {
            var rowPair = levelData.Layout[rowPairIndex];

            float scaledJsonX = rowPair.X * layoutScaleX;
            float scaledJsonY = rowPair.Y * layoutScaleY;
            GameObject tileGO = Pool.Get();//Instantiate(tilePrefab, boardParent, false);
            rowPair.visual = tileGO;
            tileGO.transform.SetParent(boardParent, false);
            var canvas = tileGO.GetComponent<Canvas>();
            canvas.sortingOrder = (rowPair.Level+1);

            RectTransform rectTransform = tileGO.GetComponent<RectTransform>();
            Vector2 adjustedPosition;
            if (rectTransform != null)
            {
                RectTransform parentRect = boardParent.GetComponent<RectTransform>();
                Vector2 parentHalfSize = parentRect.rect.size / 2f;

                adjustedPosition = new Vector2(
                    scaledJsonX - parentHalfSize.x + layoutScaleX / 2,
                    scaledJsonY - parentHalfSize.y + layoutScaleY / 2
                );

                rectTransform.anchoredPosition = adjustedPosition;
                rectTransform.localScale = Vector3.one;
                var card = tileGO.GetComponent<Card>();
                tileGO.name = "card" + "_" + rowPair.Level + "_" + rowPairIndex;
                if (rowPair.Tile == "?" || rowPair.Tile == "*")
                {
                    Debug.Log(generateLetter[rowPairIndex].ToString());
                    card.cardData.letterText.text = generateLetter[rowPairIndex].ToString();
                }
                else
                {
                    card.cardData.letterText.text = rowPair.Tile.ToString();
                }
                if (card != null)
                {
                    card.Level = rowPair.Level;
                    activeCards.Add(card);
                }

            }
            else
            {
                Debug.LogWarning("Tile Prefab does not have a RectTransform. Ensure it's a UI element.");
            }

        }


        for (int i = 0; i < activeCards.Count; i++)
        {
            if (TryGetIntersectingCards(activeCards[i], out var tileLayoutDatas))
            {
                //Debug.Log(activeCards[i].name +"_"+ tileLayoutDatas.Count);
                activeCards[i].belowCards.AddRange(tileLayoutDatas);
            }

            if (TrySetFaceUpCard(activeCards[i]))
            {
                //Debug.Log(activeCards[i].name +"_"+ tileLayoutDatas.Count);

                activeCards[i].isFaceUp = true;
                activeCards[i].cardFace.SetActive(false);


            }
        }

    }


    public bool TryGetIntersectingCards(Card targetCard, out List<Card> intersectingCards, bool isFaceup = false)
    {
        intersectingCards = new List<Card>();

        RectTransform targetRect = targetCard.GetComponent<RectTransform>();
        if (targetRect == null)
            return false;

        Rect targetWorldRect = GetWorldRect(targetRect);
        Debug.Log("target card Rect " + targetCard.name);
        foreach (var card in activeCards)
        {
            if (card == targetCard) continue;

            RectTransform otherRect = card.GetComponent<RectTransform>();
            if (otherRect == null) continue;

            Rect otherWorldRect = GetWorldRect(otherRect);
            if (isFaceup==false)
            {
                if (targetWorldRect.Overlaps(otherWorldRect) && (targetCard.Level - card.Level) == 1)
                {
                   

                    intersectingCards.Add(card);
                }
            }
           
            
        }

        return intersectingCards.Count > 0;
    }


    public bool TrySetFaceUpCard(Card targetCard)
    {
      

        RectTransform targetRect = targetCard.GetComponent<RectTransform>();
        if (targetRect == null)
            return false;

        Rect targetWorldRect = GetWorldRect(targetRect);
        Debug.Log("target card Rect " + targetCard.name);
        var alluplevelCard = activeCards.FindAll(card => card.Level > targetCard.Level);
        foreach (var card in alluplevelCard)
        {
            if (card == targetCard) continue;

            RectTransform otherRect = card.GetComponent<RectTransform>();
            if (otherRect == null) continue;

            Rect otherWorldRect = GetWorldRect(otherRect);
            if (targetWorldRect.Overlaps(otherWorldRect))
            {
                return false;
            }


        }

        return true;
    }


    public bool ShouldFlip(List<Card> belowlst, Card targetCard)
    {
        foreach (var card in belowlst)
        {
            var alluplevelCard = activeCards.FindAll(cardObj => cardObj.Level > card.Level);
         
            RectTransform targetCardRect = card.GetComponent<RectTransform>();
           
            if (targetCardRect == null) continue;

            Rect targetWorldRect = GetWorldRect(targetCardRect);
            foreach (var cardtemp in alluplevelCard)
            {
                RectTransform otherRect = cardtemp.GetComponent<RectTransform>();
                if (otherRect == null) continue;
                if (targetCard == cardtemp) continue;
                Rect otherWorldRect = GetWorldRect(otherRect);
                if (targetWorldRect.Overlaps(otherWorldRect))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float width = Vector3.Distance(corners[0], corners[3]);
        float height = Vector3.Distance(corners[0], corners[1]);
        return new Rect(corners[0], new Vector2(width, height));
    }


    List<char> GenerateAtLeastFiveVowels(int totalCards, string consonants = "BCDFGHJKLMNPQRSTVWXYZ",float difficulty = 0f,string vowels = "AEIOU")
    {
        List<char> result = new List<char>();

        // Clamp difficulty between 0 and 2, then normalize
        float normalizedDifficulty = Mathf.Clamp(difficulty, 0f, 2f) / 2f;

        // Map difficulty to 5–2 vowels (easy → hard)
        int vowelCount = Mathf.RoundToInt(Mathf.Lerp(5, 2, normalizedDifficulty));
        int consonantCount = totalCards - vowelCount;

        // Expand consonant set based on difficulty
        string extendedConsonants = consonants;
        if (vowelCount < 3) extendedConsonants += "AEIOU";  // increase confusion at hard difficulty
        else if (vowelCount < 4) extendedConsonants += "UOY"; // mild confusion at medium-hard

        // Add vowels
        for (int i = 0; i < vowelCount; i++)
            result.Add(vowels[UnityEngine.Random.Range(0, vowels.Length)]);

        // Add consonants
        for (int i = 0; i < consonantCount; i++)
            result.Add(extendedConsonants[UnityEngine.Random.Range(0, extendedConsonants.Length)]);

        // Shuffle result
        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }

        return result;
    }


}

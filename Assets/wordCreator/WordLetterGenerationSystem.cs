using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public enum BagType
{
    StandardLetterBag,
    DrawPileLetterBag,
    DrawPilePurchasedLetterBag
}
[Serializable]
public class DuplicatePreventionConfig
{
    public bool Enabled { get; set; }
    public Dictionary<string, List<double>> DuplicatePreventionTileCountToOddsToExclude { get; set; }
}

public class WordLetterGenerationSystem
{
    private static WordLetterGenerationSystem wordLetterGenerationSystem = null;
    private static readonly System.Random random = new();
    private readonly string awsUrl = "https://2dhunter.s3.us-west-2.amazonaws.com/word-solitaire-go/fb/config/letterbucket.json";
  
    private Dictionary<string, int> currentBoardLetterCounts = new Dictionary<string, int>();


    public static readonly DuplicatePreventionConfig duplicateConfig = new DuplicatePreventionConfig
    {
        Enabled = true,
        DuplicatePreventionTileCountToOddsToExclude = new Dictionary<string, List<double>>
        {
            ["E"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["A"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["I"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["O"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["U"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["B"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["C"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["D"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["F"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["G"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["H"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["J"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["K"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["L"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["M"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["N"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["P"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["Qu"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["Q"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["R"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["S"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["T"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["V"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["W"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["X"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["Y"] = new List<double> { 0, 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 },
            ["Z"] = new List<double> { 0, 0, 0.5, 0.75, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99 }
        }
    };

    public static WordLetterGenerationSystem Instance
    {

        get {
            if (wordLetterGenerationSystem == null)
            {
                wordLetterGenerationSystem= new WordLetterGenerationSystem();   
            }

            return wordLetterGenerationSystem;
        
        }
    }
    private Dictionary<string, Dictionary<string, Dictionary<string, int>>> LetterBags;

    public IEnumerator LoadWordLetterGenerationSystem()
    {
        using UnityWebRequest request = UnityWebRequest.Get(awsUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed to load letter bucket from AWS: "+request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            LetterBags = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(json);

            if (LetterBags == null)
                Debug.Log("Failed to deserialize AWS letterBucket data.");
            else
                Debug.Log("letterBucket loaded from AWS successfully!");
        }
    }
   
    private string DrawLetter(string difficulty, string bagType)
    {
        var bag = LetterBags[difficulty][bagType];
        int total = bag.Values.Sum();
        int rand = random.Next(total);

        int cumulative = 0;
        foreach (var kvp in bag)
        {
            cumulative += kvp.Value;
            if (rand < cumulative)
                return kvp.Key;
        }
        // Dynamic fallback: most frequent letter in current bag
        return bag.OrderByDescending(kvp => kvp.Value).First().Key;
    }

    private string DrawLetterV2(string difficulty, string bagType)
    {
        var bag = LetterBags[difficulty][bagType];
        int total = bag.Values.Sum();
        int rand = random.Next(total);

        int cumulative = 0;
        foreach (var kvp in bag)
        {
            cumulative += kvp.Value;
            if (rand < cumulative)
            {
                string letter = kvp.Key;

                // ✅ Duplicate prevention check
                if (duplicateConfig != null && duplicateConfig.Enabled)
                {
                    if (duplicateConfig.DuplicatePreventionTileCountToOddsToExclude.TryGetValue(letter, out var oddsList))
                    {
                        int countOnBoard = currentBoardLetterCounts.ContainsKey(letter) ? currentBoardLetterCounts[letter] : 0;
                        double odds = countOnBoard < oddsList.Count ? oddsList[countOnBoard] : oddsList.Last();

                        if (UnityEngine.Random.value < odds)
                        {
                            Debug.Log($"❌ Excluded letter {letter} due to duplicate-prevention (count {countOnBoard}, odds {odds})");
                            return DrawLetter(difficulty, bagType); // retry
                        }
                    }
                }

                // ✅ Track letter usage
                if (!currentBoardLetterCounts.ContainsKey(letter))
                    currentBoardLetterCounts[letter] = 0;
                currentBoardLetterCounts[letter]++;

                Debug.Log($"✅ Drew letter: {letter}");
                return letter;
            }
        }

        // 🔹 Dynamic fallback: most frequent letter in bag
        string fallback = bag.OrderByDescending(kvp => kvp.Value).First().Key;

        if (!currentBoardLetterCounts.ContainsKey(fallback))
            currentBoardLetterCounts[fallback] = 0;
        currentBoardLetterCounts[fallback]++;

        Debug.Log($"⚠️ Used fallback letter: {fallback}");
        return fallback;
    }

    public string GenerateLetter(int difficulty, BagType bagType)
    {
        var set = difficulty switch
        {
            0 => "NormalDifficultyDrawBags",
            1 => "HardDifficultyDrawBags",
            2 => "VeryHardDifficultyDrawBags",
            _ => "NormalDifficultyDrawBags"
        };


        var setBag = bagType switch
        {
            BagType.DrawPileLetterBag => "DrawPileLetterBag",
            BagType.StandardLetterBag => "StandardLetterBag",
            BagType.DrawPilePurchasedLetterBag => "DrawPilePurchasedLetterBag",
            _ => "StandardLetterBag"
        };
        return DrawLetter(set, setBag);
    }


    public void PrintBoardStats()
    {
        Debug.Log("📊 Current Board Letter Counts:");
        foreach (var kvp in currentBoardLetterCounts.OrderBy(k => k.Key))
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }
    public string GenerateLetterV2(int difficulty, BagType bagType)
    {
        var set = difficulty switch
        {
            0 => "NormalDifficultyDrawBags",
            1 => "HardDifficultyDrawBags",
            2 => "VeryHardDifficultyDrawBags",
            _ => "NormalDifficultyDrawBags"
        };

        var setBag = bagType switch
        {
            BagType.DrawPileLetterBag => "DrawPileLetterBag",
            BagType.StandardLetterBag => "StandardLetterBag",
            BagType.DrawPilePurchasedLetterBag => "DrawPilePurchasedLetterBag",
            _ => "StandardLetterBag"
        };

        return DrawLetterV2(set, setBag);
    }
}

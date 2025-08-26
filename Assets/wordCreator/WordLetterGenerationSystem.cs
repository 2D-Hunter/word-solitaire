using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public enum BagType
{
    StandardLetterBag,
    DrawPileLetterBag,
    DrawPilePurchasedLetterBag
}
public class WordLetterGenerationSystem
{
    private static WordLetterGenerationSystem wordLetterGenerationSystem = null;
    private static readonly System.Random random = new();
    
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
    public void  LoadWordLetterGenerationSystem()
    {
          string letterBucketPath = "Config/letterbucket";


        TextAsset jsonLetterBucketTextAsset = Resources.Load<TextAsset>(letterBucketPath);

        if (jsonLetterBucketTextAsset == null)
        {
            Debug.LogError($"JSON letter Bucket file not found at: Resources/{letterBucketPath}");
            //return;
        }
        /* letterBucket = Newtonsoft.Json.JsonConvert.DeserializeObject<LetterBucket>(jsonLetterBucketTextAsset.text);
         if (letterBucket == null)
         {
             Debug.LogError("Failed to deserialize JSON letterBucket data. >>>>>>>");
             // return;
         }*/
        string json = jsonLetterBucketTextAsset.text; //File.ReadAllText("letterbags.json");
        LetterBags = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(json);
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
}

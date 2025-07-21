// LevelDataModels.cs (You can put all these in one file or separate them)
using System;
using System.Collections.Generic;
using UnityEngine; // For Vector2; if you want to use the X,Y directly.

// --- Root Level Data ---
[System.Serializable]
public class GameLevelData
{
    public string Guid;
    public string Id;
    public LevelInfo LevelInfo;
    public List<TileLayoutData> Layout; // List of individual tile definitions
}

// --- LevelInfo Class ---
[System.Serializable]
public class LevelInfo
{
    public int PrimaryGoal;
    public List<string> Goals; // Assuming goals are strings, though empty in example
    public bool ShuffleCards;
    public bool UseSeededRandom;
    public List<int> PointsForEachStar;
    public int Difficulty;
    public List<string> Hand; // Assuming hand letters are strings
    public bool CustomStarScore;
}

// --- TileLayoutData Class (for each tile in Layout array) ---
[System.Serializable]
public class TileLayoutData
{
    public float X;
    public float Y;
    public int Level; // This appears to be the layer/Z-depth for blocking
    public string Tile; // The letter itself (e.g., "?", "A", "O")
    public int TileType;
    public int KeyLockType;
    public string Arg;
    public string Arg1;
    public string Arg2;
    public float Angle; // Potentially for tile rotation
    public GameObject visual;
}

// --- Add our existing SerializableVector2 for consistency if needed, though X,Y are floats directly ---
// (If you don't need this elsewhere, you can remove it.)
[System.Serializable]
public class SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(float rX, float rY)
    {
        x = rX;
        y = rY;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public static SerializableVector2 FromVector2(Vector2 v)
    {
        return new SerializableVector2(v.x, v.y);
    }
}


public class Metadata
{
    public int FirstKeyLockLevelNum { get; set; }
    public int FirstGoldCardLevel { get; set; }
    public int FirstEnchantedCardsLevel { get; set; }
    public int FirstMakeThreeLetterWordLevel { get; set; }
    public int FirstRandoCardLevel { get; set; }
    public int FirstTimerGoalLevel { get; set; }
    public int First2xLevel { get; set; }
    public int FirstBonusGoalLevel { get; set; }
}

public class LevelRamp
{
    public string Id { get; set; }
    public string Guid { get; set; }
    public Metadata Metadata { get; set; }
    public string HomeTutorialFilename { get; set; }
    public string PuzzleTutorialFilename { get; set; }
    public Unlocks Unlocks { get; set; }
    public List<string> Levels { get; set; }
    public List<int> Difficulties { get; set; }
}

public class Unlocks
{
    public int WildBoosterUnlockLevel { get; set; }
    public int RemoveCardBoosterUnlockLevel { get; set; }
    public int ShowBestWordUnlockLevel { get; set; }
    public int RandomizerBoosterUnlockLevel { get; set; }
}


public class LetterBucket
{
    // Runtime storage for all active tiles, mapped by their exact position for blocking logic
    public Dictionary<int, string> DefficultiMapLetterBucket = new Dictionary<int, string>();
}
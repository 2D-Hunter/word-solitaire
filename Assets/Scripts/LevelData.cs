using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelInfo
    {
        public int levelNumber;
        public int levelTarget;
        public string description;
    }

    public LevelInfo[] levels;
}
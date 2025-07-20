using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelInfo
    {
        public int levelNumber;
        public int levelTarget;
        public BonusGoalType bonusGoalType;
        public int targetPointsForBonus;
        public int reward;
        public int numberOfLetters;
        public int numberOfWords;
        public bool isLevelHard;
        public float estimatedMultiplier = 3f;
        public float[] starThresholds = new float[] { 0.4f, 0.3f, 0.3f };
    }

    public LevelInfo[] levels;
}
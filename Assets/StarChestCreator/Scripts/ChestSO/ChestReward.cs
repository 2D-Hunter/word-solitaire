using UnityEngine;

namespace StarChestCreator
{
    public enum RewardType
    {
        SuperBonus,
        ScorePoints
    }

    [System.Serializable]
    public class ChestReward
    {
        public Reward reward;
        public int amount;
    }
}
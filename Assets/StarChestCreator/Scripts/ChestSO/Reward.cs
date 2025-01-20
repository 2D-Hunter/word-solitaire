using UnityEngine;

namespace StarChestCreator
{
    public enum SuperBonusType
    {
        None = 0,
        BonusTime,
        Fireball,
        SpeedUp
    }

    [CreateAssetMenu(fileName = "New Reward", menuName = "Reward")]
    public class Reward : ScriptableObject
    {
        public new string name;
        public RewardType rewardType;
        public GameObject rewardPrefab;
        public SuperBonusType superBonusType;
    }
}
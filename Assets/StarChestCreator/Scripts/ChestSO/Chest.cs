using UnityEngine;
using System.Collections.Generic;

namespace StarChestCreator
{
    [CreateAssetMenu(fileName = "New Chest", menuName = "Chest")]
    public class Chest : ScriptableObject
    {
        public string chestName;
        public Sprite[] chestSprites;   // Array to hold the sliced sprites
        public List<ChestReward> chestRewards = new List<ChestReward>();
    }
}
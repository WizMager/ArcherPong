﻿using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BotData", menuName = "Data/BotData")]
    public class BotData : ScriptableObject
    {
        public Transform spawnPoint;
        public float botSpeed;
        public float distanceToCalculateDirection;
    }
}
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float playerMoveSpeed;
        public Transform spawnPositionFirst;
        public Transform spawnPositionSecond;
        public float[] clampValueFirstPlayer;
        public float[] clamValueEqualizerFirstPlayer;
        public float[] clampValueSecondPlayer;
        public float[] clamValueEqualizerSecondPlayer;
        public float startTimeBeforeShoot;
        public float minimalTimeBeforeShoot;
        public int timeBeforeShootMultiply;
    }
}
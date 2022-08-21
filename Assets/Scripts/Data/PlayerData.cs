using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public GameObject playerPrefab;
        public GameObject botPrefab;
        public float playerMoveSpeed;
        public Transform spawnPositionFirst;
        public Transform spawnPositionSecond;
    }
}
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PrefabData", menuName = "Data/PrefabData")]
    public class PrefabData : ScriptableObject
    {
        public GameObject uiPrefab;
        public GameObject botPrefab;
        public GameObject soloArrowPrefab;
        public GameObject networkArrowPrefab;
    }
}
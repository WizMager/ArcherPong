using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PrefabData", menuName = "Data/PrefabData")]
    public class PrefabData : ScriptableObject
    {
        [Header("Common")]
        public GameObject ui;
        public GameObject mainCamera;
        
        [Header("Single")]
        public GameObject bot;
        public GameObject singleArrow;
        public GameObject singleEnvironment;
        public GameObject singlePlayer;
        
        [Header("Multiplayer")]
        public GameObject networkArrow;
    }
}
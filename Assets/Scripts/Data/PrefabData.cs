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
        
        [Header("Multi")]
        public GameObject firstPlayerEnvironment;
        public GameObject secondPlayerEnvironment;
        
        [Header("Photon Prefabs Path")]
        public string networkArrow;
        public string firstPlayer;
        public string secondPlayer;
    }
}
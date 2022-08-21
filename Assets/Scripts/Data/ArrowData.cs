using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ArrowData", menuName = "Data/ArrowData")]
    public class ArrowData :  ScriptableObject
    {
        public GameObject arrowPrefab;
        public float arrowSpeed;
    }
}
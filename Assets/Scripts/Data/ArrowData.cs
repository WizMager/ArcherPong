using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ArrowData", menuName = "Data/ArrowData")]
    public class ArrowData :  ScriptableObject
    {
        public GameObject soloArrowPrefab;
        public GameObject networkArrowPrefab;
        public float startArrowSpeed;
        public float maxArrowSpeed;
        public int arrowMoveSpeedMultiplyPercent;
    }
}
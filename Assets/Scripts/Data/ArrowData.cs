using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ArrowData", menuName = "Data/ArrowData")]
    public class ArrowData :  ScriptableObject
    {
        public float startArrowSpeed;
        public float maxArrowSpeed;
        public int arrowMoveSpeedMultiplyPercent;
    }
}
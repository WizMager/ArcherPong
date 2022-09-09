using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ScoreData", menuName = "Data/ScoreData")]
    public class ScoreData : ScriptableObject
    {
        public int winScoreLimit;
        public float watchScoreTime;
    }
}
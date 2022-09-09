using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Data", order = 0)]
    public class Data : ScriptableObject
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private ArrowData arrowData;
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private BotData botData;

        public PlayerData GetPlayerData
        {
            get
            {
                if (playerData != null)
                {
                    return playerData;
                }
                throw new ArgumentNullException("PlayerData is null.");
            }
        }
        
        public ArrowData GetArrowData
        {
            get
            {
                if (arrowData != null)
                {
                    return arrowData;
                }
                throw new ArgumentNullException("ArrowData is null.");
            }
        }
        
        public ScoreData GetScoreData
        {
            get
            {
                if (scoreData != null)
                {
                    return scoreData;
                }
                throw new ArgumentNullException("ScoreData is null.");
            }
        }
        
        public BotData GetBotData
        {
            get
            {
                if (botData != null)
                {
                    return botData;
                }
                throw new ArgumentNullException("BotData is null.");
            }
        }
    }
}
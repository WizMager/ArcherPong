using UnityEngine;
using Views;

public class SingleGameInitialization
{
        public SingleGameInitialization(Controllers.Controllers controllers, Data.Data data)
        {
            var botData = data.GetBotData;
            var arrowData = data.GetArrowData;
            var prefabData = data.GetPrefabData;
            var scoreData = data.GetScoreData;
            
            var bot = Object.Instantiate(prefabData.botPrefab, botData.spawnPoint.position, botData.spawnPoint.rotation);
            var arrow = Object.Instantiate(prefabData.soloArrowPrefab);
            var ui = Object.Instantiate(prefabData.uiPrefab);
            
            var botController = new BotController(bot.transform, arrow.transform, botData);
            var scoreController = new SingleScoreController(ui.GetComponent<UIView>(), scoreData);
        }
}
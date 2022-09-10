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
            var playerData = data.GetPlayerData;

            var bot = Object.Instantiate(prefabData.bot, botData.spawnPoint.position, botData.spawnPoint.rotation);
            var arrow = Object.Instantiate(prefabData.singleArrow);
            var arrowView = arrow.GetComponent<SingleArrowView>();
            var ui = Object.Instantiate(prefabData.ui);
            var mainCamera = Object.Instantiate(prefabData.mainCamera);
            var environmentView = Object.Instantiate(prefabData.singleEnvironment).GetComponent<EnvironmentView>();
            var playerView = Object.Instantiate(prefabData.singlePlayer, playerData.spawnPositionFirst.position,
                playerData.spawnPositionFirst.rotation).GetComponent<PlayerView>();
            
            var botController = new BotController(bot.transform, arrow.transform, botData, arrowView);
            var scoreController = new SingleScoreController(ui.GetComponent<UIView>(), scoreData);
            var shootController = new SinglePlayerShootController(mainCamera.GetComponent<Camera>(), environmentView.GetShootlessAreaView, environmentView.GetJoystickPosition, playerView, arrowView, playerData); 
            
            botController.Init(scoreController);
            //scoreController.Init();
            shootController.Init(scoreController);
            

            controllers.Add(botController);
            controllers.Add(scoreController);
            controllers.Add(shootController);
        }
}
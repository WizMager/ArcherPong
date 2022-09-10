using UnityEngine;
using Views;

namespace Controllers.SinglePlayer
{
    public class SingleGameInitialization
    {
        public SingleGameInitialization(Controllers controllers, Data.Data data)
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
            var scoreController = new SingleScoreController(ui.GetComponent<UIView>(), scoreData, arrowView);
            var shootController = new SinglePlayerShootController(mainCamera.GetComponent<Camera>(), environmentView.GetShootlessAreaView, environmentView.GetJoystickPosition, playerView, arrowView, playerData);
            var moveController = new SinglePlayerMoveController(playerView, playerData.playerMoveSpeed,
                environmentView.GetShootlessAreaView, arrowView);
            var arrowController = new SingleArrowController(arrowView, arrowData, playerView.GetShootPosition);
            
            botController.Init(scoreController);
            shootController.Init(scoreController);
            moveController.Init(shootController, scoreController);
            arrowController.Init(shootController, scoreController);
            

            controllers.Add(botController);
            controllers.Add(scoreController);
            controllers.Add(shootController);
            controllers.Add(moveController);
            controllers.Add(arrowController);
        }
    }
}
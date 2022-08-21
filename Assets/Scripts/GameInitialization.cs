using System.Collections.Generic;
using Controllers;
using Model;
using UnityEngine;
using Views;

public class GameInitialization
{
        public GameInitialization(Controllers.Controllers controllers, Data.Data data)
        {
                var arrow = Object.Instantiate(data.GetArrowData.arrowPrefab);
                var firstPlayer = SpawnPlayer(data.GetPlayerData.playerPrefab, data.GetPlayerData.spawnPositionFirst);
                firstPlayer.GetComponent<PlayerView>().IsFirstPlayer = true;
                var secondPlayer = SpawnPlayer(data.GetPlayerData.botPrefab, data.GetPlayerData.spawnPositionSecond);
                secondPlayer.GetComponent<PlayerView>().IsFirstPlayer = false;
                var scoreCanvas = Object.Instantiate(data.GetScoreData.scoreCanvasPrefab);
                var firstPlayerScoreView = scoreCanvas.GetComponentInChildren<FirstPlayerScoreView>();
                var secondPlayerScoreView = scoreCanvas.GetComponentInChildren<SecondPlayerScoreView>();
                
                var playerShootControllers = new List<PlayerShootController>();
                var firstPlayerShootController = new PlayerShootController(firstPlayer);
                playerShootControllers.Add(firstPlayerShootController);

                var playerMoveControllers = new List<PlayerMoveController>();
                var firstPlayerMoveController = new PlayerMoveController(firstPlayer, data.GetPlayerData.playerMoveSpeed);
                playerMoveControllers.Add(firstPlayerMoveController);
                var secondPlayerMoveController = new PlayerMoveController(secondPlayer, data.GetPlayerData.playerMoveSpeed);
                playerMoveControllers.Add(secondPlayerMoveController);
                
                var arrowController = new ArrowController(playerShootControllers, arrow, data.GetArrowData.arrowSpeed);
                controllers.Add(arrowController);
                var scoreModel = new ScoreModel(firstPlayerScoreView, secondPlayerScoreView);
                var scoreController = new ScoreController(scoreModel, arrowController);
                controllers.Add(scoreController);
                
                foreach (var shootController in playerShootControllers)
                {
                        shootController.GetArrowController(arrowController);
                        controllers.Add(shootController);
                }

                foreach (var playerMoveController in playerMoveControllers)
                {
                        playerMoveController.GetArrowController(arrowController);
                        controllers.Add(playerMoveController);
                }

                
        }

        private GameObject SpawnPlayer(GameObject playerPrefab, Transform spawnPosition)
        {
                return Object.Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);
        }
}
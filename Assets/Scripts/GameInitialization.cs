using System.Collections.Generic;
using Controllers;
using Model;
using Photon.Pun;
using UnityEngine;
using Views;

public class GameInitialization
{
        public GameInitialization(Controllers.Controllers controllers, Data.Data data, PlayerView[] players)
        {
                var arrow = Object.Instantiate(data.GetArrowData.arrowPrefab);

                var scoreCanvas = Object.Instantiate(data.GetScoreData.scoreCanvasPrefab);
                var firstPlayerScoreView = scoreCanvas.GetComponentInChildren<FirstPlayerScoreView>();
                var secondPlayerScoreView = scoreCanvas.GetComponentInChildren<SecondPlayerScoreView>();
                
                var playerShootControllers = new List<PlayerShootController>();
                var firstPlayerShootController = new PlayerShootController(players[0].gameObject);
                playerShootControllers.Add(firstPlayerShootController);
                
                var playerMoveController = new PlayerMoveController(players[0].gameObject, data.GetPlayerData.playerMoveSpeed, players[0].GetComponent<PhotonView>());

                var arrowController = new ArrowControllerOld(playerShootControllers, arrow, data.GetArrowData.arrowSpeed);
                playerMoveController.GetArrowController(arrowController);
                controllers.Add(arrowController);
                controllers.Add(playerMoveController);
                
                var scoreModel = new ScoreModel(firstPlayerScoreView, secondPlayerScoreView);
                var scoreController = new ScoreControllerMVC(scoreModel, arrowController);
                controllers.Add(scoreController);
                
                foreach (var shootController in playerShootControllers)
                {
                        shootController.GetArrowController(arrowController);
                        controllers.Add(shootController);
                }
        }
}
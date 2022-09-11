using Data;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;
using Views;

namespace Controllers.MultiPlayer
{
    public class GameInitialization
    {
        public GameInitialization(Controllers controllers, Data.Data data, GameObject arrow)
        {
            var arrowData = data.GetArrowData;
            var prefabData = data.GetPrefabData;
            var scoreData = data.GetScoreData;
            var playerData = data.GetPlayerData;

            var arrowView = arrow.GetComponent<ArrowView>();
            GameObject player;
            PhotonView playerPhotonView;
            PlayerView playerView;
            GameObject environment;
            EnvironmentView environmentView;
            Camera mainCamera;
            var ui = Object.Instantiate(prefabData.ui);
            if (PhotonNetwork.IsMasterClient)
            {
                player = PhotonNetwork.Instantiate(prefabData.firstPlayer, playerData.spawnPositionFirst.position,
                    playerData.spawnPositionFirst.rotation);
                playerPhotonView = player.GetComponent<PhotonView>();
                playerView = player.GetComponent<PlayerView>();
                environment = Object.Instantiate(prefabData.firstPlayerEnvironment);
                environmentView = environment.GetComponent<EnvironmentView>();
                mainCamera = Object.Instantiate(prefabData.mainCamera).GetComponent<Camera>();
            }
            else
            {
                player = PhotonNetwork.Instantiate(prefabData.secondPlayer, playerData.spawnPositionSecond.position,
                    playerData.spawnPositionSecond.rotation);
                playerPhotonView = player.GetComponent<PhotonView>();
                playerView = player.GetComponent<PlayerView>();
                environment = Object.Instantiate(prefabData.secondPlayerEnvironment);
                environmentView = environment.GetComponent<EnvironmentView>();
                mainCamera = Object.Instantiate(prefabData.mainCamera).GetComponent<Camera>();
                mainCamera.transform.Rotate(0, 0, 180f);
            }

            var moveController = new PlayerMoveController(playerPhotonView, playerView, playerData.playerMoveSpeed,
                environmentView.GetShootlessAreaView, arrowView);
            var shootController = new PlayerShootController(playerPhotonView, mainCamera,
                environmentView.GetShootlessAreaView, environmentView.GetJoystickPosition, playerView, arrowView,
                playerData);
            var scoreController = new ScoreController(ui.GetComponent<UIView>(), scoreData, arrowView);
            var arrowController = new ArrowController(arrowView, arrowData);
            
            moveController.Init(shootController, scoreController);
            shootController.Init(scoreController);
            arrowController.Init(shootController, scoreController);

            controllers.Add(moveController);
            controllers.Add(shootController);
            controllers.Add(scoreController);
            controllers.Add(arrowController);
        }
    }
}
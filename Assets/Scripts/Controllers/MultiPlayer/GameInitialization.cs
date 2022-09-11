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
        public GameInitialization(Controllers controllers, Data.Data data)
        {
            var arrowData = data.GetArrowData;
            var prefabData = data.GetPrefabData;
            var scoreData = data.GetScoreData;
            var playerData = data.GetPlayerData;
            
            GameObject player;
            GameObject environment;
            EnvironmentView environmentView;
            Camera mainCamera;
            var ui = Object.Instantiate(prefabData.ui);

            if (PhotonNetwork.IsMasterClient)
            {
                var arrowView = PhotonNetwork.Instantiate(prefabData.networkArrow, Vector3.zero, Quaternion.identity)
                    .GetComponent<ArrowView>();
                player = PhotonNetwork.Instantiate(prefabData.firstPlayer, playerData.spawnPositionFirst.position,
                    playerData.spawnPositionFirst.rotation);
                environment = Object.Instantiate(prefabData.firstPlayerEnvironment);
                environmentView = environment.GetComponent<EnvironmentView>();
                mainCamera = Object.Instantiate(prefabData.mainCamera).GetComponent<Camera>();
            }
            else
            {
                player = PhotonNetwork.Instantiate(prefabData.secondPlayer, playerData.spawnPositionSecond.position,
                    playerData.spawnPositionSecond.rotation);
                environment = Object.Instantiate(prefabData.secondPlayerEnvironment);
                environmentView = environment.GetComponent<EnvironmentView>();
                mainCamera = Object.Instantiate(prefabData.mainCamera).GetComponent<Camera>();
                mainCamera.transform.Rotate(0, 0, 180f);
            }
            
        }
    }
}
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
     [SerializeField] private Button leave;
     [SerializeField] private Transform firstPlayerSpawn;
     [SerializeField] private Transform secondPlayerSpawn;

     private void Start()
     {
          if (PhotonNetwork.IsMasterClient)
          {
               PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FirstPlayer"), firstPlayerSpawn.position, firstPlayerSpawn.rotation);
               Instantiate(Resources.Load<GameObject>("FirstPlayerEnvironment"));
          }
          else
          {
               PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SecondPlayer"), secondPlayerSpawn.position, secondPlayerSpawn.rotation);
               Instantiate(Resources.Load<GameObject>("SecondPlayerEnvironment"));
          }
          leave.onClick.AddListener(LeaveRoom);
     }

     private void LeaveRoom()
     {
          PhotonNetwork.LeaveRoom();
     }

     public override void OnLeftRoom()
     {
          SceneManager.LoadScene(0);
     }

     public override void OnPlayerLeftRoom(Player otherPlayer)
     {
          LeaveRoom();
     }
}
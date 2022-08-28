using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
     [SerializeField] private Button leave;

     private void Start()
     {
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

     public override void OnPlayerEnteredRoom(Player newPlayer)
     {
          Debug.Log($"Player{newPlayer} join to room");
     }

     public override void OnPlayerLeftRoom(Player otherPlayer)
     {
          Debug.Log($"Player{otherPlayer} left room");
     }
}
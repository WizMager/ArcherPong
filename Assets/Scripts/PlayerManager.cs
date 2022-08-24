using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
     private PhotonView _photonView;

     private void Awake()
     {
          _photonView = GetComponent<PhotonView>();
     }

     private void Start()
     {
          if (_photonView.IsMine)
          {
               CreatePlayer();
          }
     }

     private void CreatePlayer()
     {
          var isFirstPlayer = FindObjectOfType<PlayerController>() == null;
          Debug.Log(FindObjectsOfType<PlayerController>().Length);
          var player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
          player.GetComponent<PlayerController>().TakeArrow(isFirstPlayer);
          //Debug.Log(FindObjectOfType<PlayerController>());
          //player.GetComponent<PlayerController>().TakeArrow(!FindObjectOfType<PlayerController>());
          // if (isFirstPlayer)
          // {
          //      player.GetComponent<PlayerController>().TakeArrow(true);
          //      Debug.Log("true");
          // }
          // else
          // {
          //      player.GetComponent<PlayerController>().TakeArrow(false);
          //      Debug.Log("false");
          // }
     }
}
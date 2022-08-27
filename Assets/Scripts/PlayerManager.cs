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
          PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
     }
}
using System.IO;
using Photon.Pun;
using UnityEngine;

public class ArrowManager : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && _photonView.IsMine)
        {
           CreateArrowController(); 
        }
    }

    private void CreateArrowController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ArrowController"), Vector3.zero, Quaternion.identity);
    }
}
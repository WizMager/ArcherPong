using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button join;
    [SerializeField] private Button create;
    
    private void Start()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectingToMasterServer)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(); 
        }

        join.onClick.AddListener(JoinRoom);
        create.onClick.AddListener(CreateRoom);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
    }

    private void CreateRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 2});
    }

    private void JoinRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }

    private void OnDestroy()
    {
        join.onClick.RemoveListener(JoinRoom);
        create.onClick.RemoveListener(CreateRoom);
    }
}
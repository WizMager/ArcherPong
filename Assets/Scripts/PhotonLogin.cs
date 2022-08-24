using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLogin : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button findRoom;
    [SerializeField] private Button createRoom;

    private void Start()
    {
        findRoom.onClick.AddListener(FindRoom);
        createRoom.onClick.AddListener(CreateRoom);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CreateRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            MaxPlayers = 2
        });
    }

    private void FindRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.JoinRandomRoom();
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnMaster");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join failed");
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
}
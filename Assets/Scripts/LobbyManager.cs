using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button join;
    [SerializeField] private Button create;
    [SerializeField] private GameObject connectionLabel;
    [SerializeField] private Button singlePlayerGame;
    
    private void Start()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            connectionLabel.SetActive(false);
            join.onClick.AddListener(JoinRoom);
            create.onClick.AddListener(CreateRoom);
            singlePlayerGame.onClick.AddListener(BotGame);
        }
        else
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            join.onClick.AddListener(JoinRoom);
            create.onClick.AddListener(CreateRoom);
            singlePlayerGame.onClick.AddListener(BotGame);
            join.gameObject.SetActive(false);
            create.gameObject.SetActive(false);
        }
    }

    private void JoinRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.JoinRandomRoom();
    }
    
    private void CreateRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer) return;
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 2});
    }

    private void BotGame()
    {
        SceneManager.LoadScene(2);
    }
    
    public override void OnConnectedToMaster()
    {
        connectionLabel.SetActive(false);
        join.gameObject.SetActive(true);
        create.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    private void OnDestroy()
    {
        join.onClick.RemoveListener(JoinRoom);
        create.onClick.RemoveListener(CreateRoom);
        singlePlayerGame.onClick.RemoveListener(BotGame);
    }
}
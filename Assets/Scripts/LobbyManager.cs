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
    [SerializeField] private Button bot;
    
    private void Start()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectingToMasterServer)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(); 
        }

        join.onClick.AddListener(JoinRoom);
        create.onClick.AddListener(CreateRoom);
        bot.onClick.AddListener(BotGame);
        join.gameObject.SetActive(false);
        create.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        join.gameObject.SetActive(true);
        create.gameObject.SetActive(true);
        connectionLabel.SetActive(false);
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
    
    private void BotGame()
    {
        SceneManager.LoadScene(2);
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
        bot.onClick.RemoveListener(BotGame);
    }
}
using System;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button join;
    [SerializeField] private Button create;
    [SerializeField] private GameObject connectionLabel;
    [SerializeField] private Button bot;
    private const string PlayFabAuthorizedKey = "AuthorizedKey";
    private const string LastScoreKey = "LastScore";
    private bool _photonIsLogin = true;
    private bool _playFabIsLogin = true;
    private bool _isCheckedLogin;
    private string _playFabId;

    private void Start()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            _playFabIsLogin = false;
            var needCreation = PlayerPrefs.HasKey(PlayFabAuthorizedKey);
            if (!needCreation)
            {
                PlayerPrefs.SetString(PlayFabAuthorizedKey, Guid.NewGuid().ToString());
            }
            var id = PlayerPrefs.GetString(PlayFabAuthorizedKey);
            var playFabRequest = new LoginWithCustomIDRequest
            {
                CustomId = id,
                CreateAccount = needCreation
            };
            PlayFabClientAPI.LoginWithCustomID(playFabRequest, OnLoginSuccess, OnPlayFabError);
        }
        else
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                OnGetUserInfo, OnPlayFabError);
        }


        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            _photonIsLogin = false;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(); 
        }

        join.onClick.AddListener(JoinRoom);
        create.onClick.AddListener(CreateRoom);
        bot.onClick.AddListener(BotGame);
        join.gameObject.SetActive(false);
        create.gameObject.SetActive(false);
        bot.gameObject.SetActive(false);
    }

    private void OnGetUserInfo(GetAccountInfoResult result)
    {
        _playFabId = result.AccountInfo.PlayFabId;
        SetPreviousScore();
    }

    private void OnPlayFabError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        _playFabId = result.PlayFabId;
        _playFabIsLogin = true;
        SetPreviousScore();
        Debug.Log("Login is success!");
    }

    private void SetPreviousScore()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = _playFabId
        }, resultCallback =>
        {
            var lastScore = resultCallback.Data.ContainsKey(LastScoreKey)
                ? int.Parse(resultCallback.Data[LastScoreKey].Value)
                : -1;
            SrtLastScoreText(lastScore);
        }, OnPlayFabError);
    }

    private void SrtLastScoreText(int score)
    {
        if (score < 0)
        {
            connectionLabel.GetComponent<TMP_Text>().text = "Your have no last score...";  
        }
        else
        {
            connectionLabel.GetComponent<TMP_Text>().text = "Your last score is: " + score; 
        }
    }
    
    public override void OnConnectedToMaster()
    {
        _photonIsLogin = true;
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

    private void Update()
    {
        if (_isCheckedLogin) return;
        if (_photonIsLogin && _playFabIsLogin)
        {
            join.gameObject.SetActive(true);
            create.gameObject.SetActive(true);
            bot.gameObject.SetActive(true);
            _isCheckedLogin = true;
        }
    }

    private void OnDestroy()
    {
        join.onClick.RemoveListener(JoinRoom);
        create.onClick.RemoveListener(CreateRoom);
        bot.onClick.RemoveListener(BotGame);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using Utils;

public class ScoreController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public Action OnStartNextRound;
    [SerializeField] private TMP_Text firstPlayerScore;
    [SerializeField] private TMP_Text secondPlayerScore;
    [SerializeField] private TMP_Text winLabel;
    [SerializeField] private int winScoreLimit;
    [SerializeField] private float watchScoreTime;
    [SerializeField] private ArrowController arrowController;
    private int _firstPlayer;
    private int _secondPlayer;
    private int _sumFirstPlayerScore;
    private int _sumSecondPlayerScore;
    private readonly List<PlayerController> _playerControllers = new(2);
    private const string LastScoreKey = "LastScore";
    private const string WinRoundKey = "WinRound";

    private void Start()
    {
        winLabel.enabled = false;
        if (!PhotonNetwork.IsMasterClient) return;
        arrowController.OnPlayerMiss += ArrowMissed;
    }

    public void AddPlayerController(PlayerController playerController)
    {
        _playerControllers.Add(playerController);
    }
    
    private void ArrowMissed(bool isFirstPlayer)
    {
        if (isFirstPlayer)
        {
            _secondPlayer++;
            _sumSecondPlayerScore++;
            secondPlayerScore.text = _secondPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.SecondPlayerScore, _secondPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        else
        {
            _firstPlayer++;
            _sumFirstPlayerScore++;
            firstPlayerScore.text = _firstPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.FirstPlayerScore, _firstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        
        if (_firstPlayer < winScoreLimit && _secondPlayer < winScoreLimit) return;
        if (_firstPlayer > _secondPlayer)
        {
            PlayFabGetId();
        }
        else
        {
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.SecondPlayerWinRound, null, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        var winText = _firstPlayer > _secondPlayer ? "First Player\n Win!" : "Second Player\n Win!";
        PhotonNetwork.RaiseEvent((int)PhotonEventCode.WatchWinScore, winText, RaiseEventOptions.Default,
            SendOptions.SendReliable);
        WatchWinScore(winText);
        StartCoroutine(WatchScoreTimer());
    }

    private void PlayFabGetId()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            PlayFabGetWinScore(result.AccountInfo.PlayFabId); 
        }, error => { Debug.Log(error.GenerateErrorReport());});
    }
    
    private void PlayFabGetWinScore(string id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest{ PlayFabId = id}, result =>
        {
            var currentScore = result.Data.ContainsKey(WinRoundKey) ? int.Parse(result.Data[WinRoundKey].Value) : 0;
            AddWinScore(currentScore);
        }, error => { Debug.Log(error.GenerateErrorReport());});
    }

    private void AddWinScore(int currentScore)
    {
        var winData = new Dictionary<string, string>
            {{WinRoundKey, (currentScore + 1).ToString()}};
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = winData
        }, _ => { }, error => { Debug.Log(error.GenerateErrorReport()); });
    }
    
    private void WatchWinScore(string winText)
    {
        foreach (var controller in _playerControllers)
        {
            controller.StopMove = true;
        }
        winLabel.enabled = true;
        winLabel.text = winText;
    }

    private void StopWatchWinScore()
    {
        _firstPlayer = 0;
        _secondPlayer = 0;
        firstPlayerScore.text = _firstPlayer.ToString();
        secondPlayerScore.text = _secondPlayer.ToString();
        winLabel.enabled = false;
        foreach (var controller in _playerControllers)
        {
            controller.StopMove = false;
        }
    }
    
    private IEnumerator WatchScoreTimer()
    {
        for (float i = 0; i < watchScoreTime; i += Time.deltaTime)
        {
            yield return null;
        }
        StopWatchWinScore();
        PhotonNetwork.RaiseEvent((int)PhotonEventCode.StopWatchWinScore, null, RaiseEventOptions.Default,
            SendOptions.SendReliable);
        OnStartNextRound?.Invoke();
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (int)PhotonEventCode.FirstPlayerScore:
                _sumSecondPlayerScore++;
                secondPlayerScore.text = photonEvent.CustomData.ToString();
                break;
            case (int)PhotonEventCode.SecondPlayerScore:
                _sumFirstPlayerScore++;
                firstPlayerScore.text = photonEvent.CustomData.ToString();
                break;
            case (int)PhotonEventCode.WatchWinScore:
                WatchWinScore(photonEvent.CustomData.ToString());
                break;
            case (int)PhotonEventCode.StopWatchWinScore:
                StopWatchWinScore();
                break;
            case (int)PhotonEventCode.SecondPlayerWinRound:
                PlayFabGetId();
                break;
        }
    }

    private void OnDestroy()
    {
        var scoreData = new Dictionary<string, string> {{LastScoreKey, (PhotonNetwork.IsMasterClient ? _sumFirstPlayerScore : _sumSecondPlayerScore).ToString()}};
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = scoreData
        }, _=> {}, error => { Debug.Log(error.GenerateErrorReport());});
        if (!PhotonNetwork.IsMasterClient) return;
        arrowController.OnPlayerMiss -= ArrowMissed;
    }
}
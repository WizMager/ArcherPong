using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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
    private readonly List<PlayerController> _playerControllers = new(2);

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
            secondPlayerScore.text = _secondPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.SecondPlayerScore, _secondPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        else
        {
            _firstPlayer++;
            firstPlayerScore.text = _firstPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.FirstPlayerScore, _firstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        
        if (_firstPlayer < winScoreLimit && _secondPlayer < winScoreLimit) return;
        var winText = _firstPlayer > _secondPlayer ? "First Player\n Win!" : "Second Player\n Win!";
        PhotonNetwork.RaiseEvent((int)PhotonEventCode.WatchWinScore, winText, RaiseEventOptions.Default,
            SendOptions.SendReliable);
        WatchWinScore(winText);
        StartCoroutine(WatchScoreTimer());
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
                firstPlayerScore.text = photonEvent.CustomData.ToString();
                break;
            case (int)PhotonEventCode.SecondPlayerScore:
                secondPlayerScore.text = photonEvent.CustomData.ToString();
                break;
            case (int)PhotonEventCode.WatchWinScore:
                WatchWinScore(photonEvent.CustomData.ToString());
                break;
            case (int)PhotonEventCode.StopWatchWinScore:
                StopWatchWinScore();
                break;
        }
    }

    private void OnDestroy()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        arrowController.OnPlayerMiss -= ArrowMissed;
    }
}
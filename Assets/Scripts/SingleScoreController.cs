using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SingleScoreController : MonoBehaviour
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
    private SinglePlayerController _playerControllers;
    private BotController _botController;

    private void Start()
    {
        _playerControllers = FindObjectOfType<SinglePlayerController>();
        _botController = FindObjectOfType<BotController>();
        winLabel.enabled = false;
        arrowController.OnPlayerMiss += ArrowMissed;
    }
    
    
    private void ArrowMissed(bool isFirstPlayer)
    {
        if (isFirstPlayer)
        {
            _secondPlayer++;
            secondPlayerScore.text = _secondPlayer.ToString();
        }
        else
        {
            _firstPlayer++;
            firstPlayerScore.text = _firstPlayer.ToString();
        }
        
        if (_firstPlayer < winScoreLimit && _secondPlayer < winScoreLimit) return;
        var winText = _firstPlayer > _secondPlayer ? "First Player\n Win!" : "Second Player\n Win!";
        WatchWinScore(winText);
        StartCoroutine(WatchScoreTimer());
    }

    private void WatchWinScore(string winText)
    {
        _playerControllers.StopMove = true;
        _botController.StopMove = true;
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
        _playerControllers.StopMove = false;
        _botController.StopMove = false;
    }
    
    private IEnumerator WatchScoreTimer()
    {
        for (float i = 0; i < watchScoreTime; i += Time.deltaTime)
        {
            yield return null;
        }
        StopWatchWinScore();
        OnStartNextRound?.Invoke();
    }

    private void OnDestroy()
    {
        arrowController.OnPlayerMiss -= ArrowMissed;
    }      
}
using System;
using System.Collections;
using Data;
using UnityEngine;
using Views;

namespace Model
{
    public class ScoreModel
    {
        public Action<bool> OnStopGame;
        private readonly UIView _uiView;
        private int _firstPlayerScore;
        private int _secondPlayerScore;
        private readonly int _winScoreLimit;
        private readonly float _watchScoreTime;

        public ScoreModel(UIView uiView, ScoreData scoreData)
        {
            _uiView = uiView;
            _winScoreLimit = scoreData.winScoreLimit;
            _watchScoreTime = scoreData.watchScoreTime;
            _uiView.WinTextActivation(false);
        }

        public void ChangeScore(bool isFirstPlayer)
        {
            if (isFirstPlayer)
            {
                _secondPlayerScore++;
                _uiView.SetSecondPlayerScore(_secondPlayerScore.ToString());
            }
            else
            {
                _firstPlayerScore++;
                _uiView.SetFirstPlayerScore(_firstPlayerScore.ToString());
            }

            if (_firstPlayerScore < _winScoreLimit && _secondPlayerScore < _winScoreLimit) return;
            OnStopGame?.Invoke(true);
            _uiView.WinTextActivation(true);
            _uiView.SetWinText(_firstPlayerScore > _secondPlayerScore ? "First Player\n Win!" : "Bot\n Win!");
            _uiView.StartCoroutine(WatchScoreTimer());
            _firstPlayerScore = 0;
            _secondPlayerScore = 0;
            _uiView.SetFirstPlayerScore(_firstPlayerScore.ToString());
            _uiView.SetSecondPlayerScore(_secondPlayerScore.ToString());
        }
        
        private IEnumerator WatchScoreTimer()
        {
            for (float i = 0; i < _watchScoreTime; i += Time.deltaTime)
            {
                yield return null;
            }
            _uiView.WinTextActivation(false);
            OnStopGame?.Invoke(false);
            _uiView.StopCoroutine(WatchScoreTimer());
        }
    }
}
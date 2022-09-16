using System;
using Controllers.Interfaces;
using Data;
using Model;
using UnityEngine;
using Views;

namespace Controllers.SinglePlayer
{
    public class SingleScoreController : ICleanup
    {
        public Action<bool> OnGamePause;
        private readonly SingleScoreModel _scoreModel;
        private readonly SingleArrowView _arrowView;

        public SingleScoreController(UIView uiView, ScoreData scoreData, SingleArrowView arrowView)
        {
            _scoreModel = new SingleScoreModel(uiView, scoreData);
            _arrowView = arrowView;
            
            _scoreModel.OnStopGame += OnStopGameHandler;
            _arrowView.OnMiss += OnMissHandler;
        }

        private void OnStopGameHandler(bool isStop)
        {
            OnGamePause?.Invoke(isStop);
        }
    
        private void OnMissHandler(bool isFirstPlayer)
        {
            _scoreModel.ChangeScore(isFirstPlayer);
        }

        public void Cleanup()
        {
            _scoreModel.OnStopGame -= OnStopGameHandler;
            _arrowView.OnMiss -= OnMissHandler;
        }
    }
}
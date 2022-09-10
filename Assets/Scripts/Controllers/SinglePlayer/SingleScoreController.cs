using System;
using Controllers.Interfaces;
using Data;
using Model;
using Views;

namespace Controllers.SinglePlayer
{
    public class SingleScoreController : ICleanup
    {
        public Action<bool> OnGamePause;
        private readonly ScoreModel _scoreModel;
        private readonly SingleArrowView _arrowView;

        public SingleScoreController(UIView uiView, ScoreData scoreData, SingleArrowView arrowView)
        {
            _scoreModel = new ScoreModel(uiView, scoreData);
            _arrowView = arrowView;
            
            _scoreModel.OnStopGame += GameStopped;
            _arrowView.OnMiss += ArrowMissed;
        }

        private void GameStopped(bool isStop)
        {
            OnGamePause?.Invoke(isStop);
        }
    
        private void ArrowMissed(bool isFirstPlayer)
        {
            _scoreModel.ChangeScore(isFirstPlayer);
        }

        public void Cleanup()
        {
            _scoreModel.OnStopGame -= GameStopped;
            _arrowView.OnMiss -= ArrowMissed;
        }
    }
}
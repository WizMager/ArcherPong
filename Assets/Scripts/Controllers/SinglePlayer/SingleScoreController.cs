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
        private SingleArrowController _arrowController;
        private readonly ScoreModel _scoreModel;

        public SingleScoreController(UIView uiView, ScoreData scoreData)
        {
            _scoreModel = new ScoreModel(uiView, scoreData);
            _scoreModel.OnStopGame += GameStopped;
        }

        public void Init(SingleArrowController arrowController)
        {
            _arrowController = arrowController;
            _arrowController.OnPlayerMiss += ArrowMissed;
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
            _arrowController.OnPlayerMiss -= ArrowMissed;
        }
    }
}
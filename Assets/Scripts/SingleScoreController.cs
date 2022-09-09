using System;
using Controllers.Interfaces;
using Data;
using Model;
using Views;

public class SingleScoreController : ICleanup
{
    public Action<bool> OnStopCharacters;
    private SingleArrowController _arrowController;
    private readonly ScoreModel _scoreModel;

    public SingleScoreController(UIView uiView, ScoreData scoreData)
    {
        _scoreModel = new ScoreModel(uiView, scoreData);
        _scoreModel.OnStopGame += GameStopped;
    }

    public void TakeArrowController(SingleArrowController arrowController)
    {
        _arrowController = arrowController;
        _arrowController.OnPlayerMiss += ArrowMissed;
    }
    
    private void GameStopped(bool isStop)
    {
        OnStopCharacters?.Invoke(isStop);
    }
    
    private void ArrowMissed(bool isFirstPlayer)
    {
        _scoreModel.ChangeScore(isFirstPlayer);
    }

    public void Cleanup()
    {
        _arrowController.OnPlayerMiss -= ArrowMissed;
    }
}
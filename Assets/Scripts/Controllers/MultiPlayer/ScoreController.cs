using System;
using Controllers.Interfaces;
using Data;
using ExitGames.Client.Photon;
using Model;
using Photon.Pun;
using Photon.Realtime;
using Utils;
using Views;

namespace Controllers.MultiPlayer
{
    public class ScoreController : ICleanup, IOnEventCallback
    {
        public Action<bool> OnGamePause;
        private readonly ScoreModel _scoreModel;
        private readonly ArrowView _arrowView;

        public ScoreController(UIView uiView, ScoreData scoreData, ArrowView arrowView)
        {
            _scoreModel = new ScoreModel(uiView, scoreData);
            _arrowView = arrowView;
        
            if (!PhotonNetwork.IsMasterClient) return;
            _scoreModel.OnStopGame += OnStopGameHandler;
            _arrowView.OnMiss += OnMissHandler;
        }

        private void OnStopGameHandler(bool isStop)
        {
            PhotonNetwork.RaiseEvent((int) PhotonEventCode.OnStopGame, isStop, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            OnGamePause?.Invoke(isStop);
        }
    
        private void OnMissHandler(bool isFirstPlayer)
        {
            PhotonNetwork.RaiseEvent((int) PhotonEventCode.ArrowMiss, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            _scoreModel.ChangeScore(isFirstPlayer);
        }

        public void Cleanup()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _scoreModel.OnStopGame -= OnStopGameHandler;
            _arrowView.OnMiss -= OnMissHandler;
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)PhotonEventCode.OnStopGame:
                    OnGamePause?.Invoke((bool)photonEvent.CustomData);
                    break;
                case (int)PhotonEventCode.ArrowMiss:
                    _scoreModel.ChangeScore((bool)photonEvent.CustomData);
                    break;
            }
        }
    }
}
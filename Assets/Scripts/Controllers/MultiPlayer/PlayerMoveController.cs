using Controllers.Interfaces;
using Controllers.SinglePlayer;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;
using Views;

namespace Controllers.MultiPlayer
{
    public class PlayerMoveController : IAwake, IEnable, IExecute, IDisable, ICleanup, IOnEventCallback
    {
        private readonly PhotonView _photonView;
        private readonly PlayerView _playerView;
        private readonly Transform _playerTransform;
        private readonly float _playerMoveSpeed;
        private readonly ShootlessAreaView _shootlessArea;
        private readonly ArrowView _arrowView;
        private SinglePlayerShootController _shootController;
        private SingleScoreController _scoreController;
    
        private PlayerInput _playerInput;
        private readonly Vector2 _startPosition;
        private readonly Quaternion _startRotation;
        private bool _stopMove = true;
        private bool _hasArrow = true;
        private bool _canShoot = true;
        private readonly bool _isFirstPlayer;
        
        public PlayerMoveController(PhotonView photonView, PlayerView playerView, float playerMoveSpeed, ShootlessAreaView shootlessAreaView, ArrowView arrowView)
        {
            _photonView = photonView;
            _playerView = playerView;
            _playerTransform = playerView.GetComponent<Transform>();
            _playerMoveSpeed = playerMoveSpeed;
            _shootlessArea = shootlessAreaView;
            _arrowView = arrowView;
            _startPosition = _playerTransform.position;
            _startRotation = _playerTransform.rotation;
            _isFirstPlayer = _playerView.IsFirstPlayer;
        
            _playerView.OnWallEnter += OnWallEnterHandler;
            _shootlessArea.OnShootActivator += OnShootActivatorHandler;
            if (!PhotonNetwork.IsMasterClient) return;
            _arrowView.OnMiss += OnMissHandler;
            _arrowView.OnCatch += OnCatchHandler;
        }
        
        public void Init(SinglePlayerShootController shootController, SingleScoreController scoreController)
        {
            _shootController = shootController;
            _scoreController = scoreController;
            _shootController.OnShoot += OnShootHandler;
            _scoreController.OnGamePause += OnGamePauseHandler;
        }

        private void OnWallEnterHandler(Vector3 normal)
        {
            _playerTransform.Translate(normal * _playerMoveSpeed * Time.deltaTime);
        }
    
        private void OnShootActivatorHandler(bool shootActivate)
        {
            _canShoot = shootActivate;
        }

        private void OnMissHandler(bool isFirstPlayer)
        {
            PhotonNetwork.RaiseEvent((int) PhotonEventCode.ArrowMiss, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            OnMissHelper(isFirstPlayer);
        }

        private void OnMissHelper(bool isFirstPlayer)
        {
            ReturnToStartPosition();
            if (_isFirstPlayer != isFirstPlayer) return;
            _hasArrow = true;
            _stopMove = true;
        }
        
        private void OnCatchHandler(bool isFirstPlayer)
        {
            PhotonNetwork.RaiseEvent((int) PhotonEventCode.ArrowCatch, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            OnCatchHelper(isFirstPlayer);
        }

        private void OnCatchHelper(bool isFirstPlayer)
        {
            if (_isFirstPlayer != isFirstPlayer) return;
            _hasArrow = true;
            if (!_canShoot) return;
            _stopMove = true;
        }
        
        private void OnShootHandler()
        {
            _hasArrow = false;
            _stopMove = false;
        }
    
        private void OnGamePauseHandler(bool isPause)
        {
            if (!isPause) return;
            ReturnToStartPosition();
            _hasArrow = true;
            _stopMove = true;
        }
        
        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)PhotonEventCode.ArrowMiss:
                    OnMissHelper((bool)photonEvent.CustomData);
                    break;
                case (int)PhotonEventCode.ArrowCatch:
                    OnCatchHelper((bool)photonEvent.CustomData);
                    break;
            }
        }
        
        private void ReturnToStartPosition()
        {
            _playerTransform.position = _startPosition;
            _playerTransform.rotation = _startRotation;
        }
        
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }

        public void OnEnable()
        {
            if (!_photonView.IsMine) return;
            _playerInput.Player.Move.Enable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void Execute(float deltaTime)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;
            if (!_photonView.IsMine) return;
            if (_stopMove) return;
            if (_hasArrow && _canShoot) return;
            _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * (_playerMoveSpeed * deltaTime)); 
        }

        public void OnDisable()
        {
            if (!_photonView.IsMine) return;
            _playerInput.Player.Move.Disable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void Cleanup()
        {
            _playerView.OnWallEnter -= OnWallEnterHandler;
            _shootlessArea.OnShootActivator -= OnShootActivatorHandler;
            _shootController.OnShoot -= OnShootHandler;
            _scoreController.OnGamePause -= OnGamePauseHandler;
            if (!PhotonNetwork.IsMasterClient) return;
            _arrowView.OnMiss -= OnMissHandler;
            _arrowView.OnCatch -= OnCatchHandler;
        }
    }
}
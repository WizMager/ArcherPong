using Controllers.Interfaces;
using UnityEngine;
using Views;

namespace Controllers.SinglePlayer
{
    public class SinglePlayerMoveController : IAwake, IEnable, IExecute, IDisable, ICleanup
    {
        private readonly PlayerView _playerView;
        private readonly Transform _playerTransform;
        private readonly float _playerMoveSpeed;
        private readonly ShootlessAreaView _shootlessArea;
        private readonly SingleArrowView _arrowView;
        private SinglePlayerShootController _shootController;
        private SingleScoreController _scoreController;
    
        private PlayerInput _playerInput;
        private readonly Vector2 _startPosition;
        private readonly Quaternion _startRotation;
        private bool _stopMove;

        public SinglePlayerMoveController(PlayerView playerView, float playerMoveSpeed, ShootlessAreaView shootlessAreaView, SingleArrowView arrowView)
        {
            _playerView = playerView;
            _playerTransform = playerView.GetComponent<Transform>();
            _playerMoveSpeed = playerMoveSpeed;
            _shootlessArea = shootlessAreaView;
            _arrowView = arrowView;
            _startPosition = _playerTransform.position;
            _startRotation = _playerTransform.rotation;
        
            _playerView.OnWallEnter += WallEntered;
            _shootlessArea.OnShootActivator += MoveStopper;
            _arrowView.OnMiss += ReturnToStartPosition;
            _arrowView.OnCatch += OnCaught;
        }

        public void Init(SinglePlayerShootController shootController, SingleScoreController scoreController)
        {
            _shootController = shootController;
            _scoreController = scoreController;
        
            _shootController.OnShoot += OnShooted;
            _scoreController.OnGamePause += MoveStopper;
        }

        private void WallEntered(Vector3 normal)
        {
            _playerTransform.Translate(normal * _playerMoveSpeed * Time.deltaTime);
        }
    
        private void MoveStopper(bool isStop)
        {
            _stopMove = isStop;
        }
    
        private void ReturnToStartPosition(bool isFirstPlayer)
        {
            _playerTransform.position = _startPosition;
            _playerTransform.rotation = _startRotation;
        }
    
        private void OnCaught()
        {
            _stopMove = true;
        }

        private void OnShooted()
        {
            _stopMove = false;
        }
    
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }

        public void OnEnable()
        {
            _playerInput.Player.Move.Enable();
        }
    
        public void Execute(float deltaTime)
        {
            if (_stopMove) return;
            _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * (_playerMoveSpeed * deltaTime)); 
        }

        public void OnDisable()
        {
            _playerInput.Player.Move.Disable();
        }

        public void Cleanup()
        {
            _playerView.OnWallEnter -= WallEntered;
            _shootlessArea.OnShootActivator -= MoveStopper;
            _arrowView.OnMiss -= ReturnToStartPosition;
            _arrowView.OnCatch -= OnCaught;
            _shootController.OnShoot -= OnShooted;
            _scoreController.OnGamePause -= MoveStopper;
        }
    }
}
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
        private bool _stopMove = true;
        private bool _hasArrow = true;
        private bool _canShoot = true;

        public SinglePlayerMoveController(PlayerView playerView, float playerMoveSpeed, ShootlessAreaView shootlessAreaView, SingleArrowView arrowView)
        {
            _playerView = playerView;
            _playerTransform = playerView.GetComponent<Transform>();
            _playerMoveSpeed = playerMoveSpeed;
            _shootlessArea = shootlessAreaView;
            _arrowView = arrowView;
            _startPosition = _playerTransform.position;
            _startRotation = _playerTransform.rotation;
        
            _playerView.OnWallEnter += OnWallEnterHandler;
            _shootlessArea.OnShootActivator += OnShootActivatorHandler;
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
            ReturnToStartPosition();
            _hasArrow = true;
            _stopMove = true;
        }
        
        private void OnCatchHandler()
        {
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
            if (isPause)
            {
                ReturnToStartPosition();
                _hasArrow = true;
                _stopMove = true;  
            }
            else
            {
                _stopMove = false;
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
            _playerInput.Player.Move.Enable();
        }
    
        public void Execute(float deltaTime)
        {
            if (_stopMove) return;
            if (_hasArrow && _canShoot) return;
            _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * (_playerMoveSpeed * deltaTime)); 
        }

        public void OnDisable()
        {
            _playerInput.Player.Move.Disable();
        }

        public void Cleanup()
        {
            _playerView.OnWallEnter -= OnWallEnterHandler;
            _shootlessArea.OnShootActivator -= OnShootActivatorHandler;
            _arrowView.OnMiss -= OnMissHandler;
            _arrowView.OnCatch -= OnCatchHandler;
            _shootController.OnShoot -= OnShootHandler;
            _scoreController.OnGamePause -= OnGamePauseHandler;
        }
    }
}
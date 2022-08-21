using Controllers.Interfaces;
using UnityEngine;
using Views;

namespace Controllers
{
    public class PlayerMoveController : IStart,IEnable, IDisable, ICleanup, IExecute, IAwake
    {
        private ArrowController _arrowController;
        private Transform _playerTransform;
        private float _playerMoveSpeed;
        private bool _isFirstPlayer;
        private PlayerView _playerView;
        private PlayerInput _playerInput;
        private Vector2 _startPosition;
        private Quaternion _startRotation;

        public PlayerMoveController(GameObject player, float playerMoveSpeed)
        {
            _playerTransform = player.GetComponent<Transform>();
            _playerView = player.GetComponent<PlayerView>();
            _isFirstPlayer = _playerView.IsFirstPlayer;
            _playerMoveSpeed = playerMoveSpeed;
            _startPosition = _playerTransform.position;
            _startRotation = _playerTransform.rotation;
        }

        public void GetArrowController(ArrowController arrowController)
        {
            _arrowController = arrowController;
        }
        
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }
        
        public void Start()
        {
            _arrowController.OnArrowMiss += ArrowMissed;
            _playerView.OnWallEnter += WallEntered;
        }

        private void WallEntered(Vector3 normal)
        {
            _playerTransform.Translate(normal * _playerMoveSpeed * Time.deltaTime);
        }

        private void ArrowMissed(bool isFirstPlayer)
        {
            _playerTransform.SetPositionAndRotation(_startPosition, _startRotation);
        }

        public void Cleanup()
        {
            _arrowController.OnArrowMiss -= ArrowMissed;
        }

        public void OnEnable()
        {
            if (_isFirstPlayer)
            {
                _playerInput.Player.Move.Enable(); 
            }
            else
            {
                _playerInput.PlayerTwo.Move.Enable();
            }
        }

        public void OnDisable()
        {
            if (_isFirstPlayer)
            {
                _playerInput.Player.Move.Disable(); 
            }
            else
            {
                _playerInput.PlayerTwo.Move.Disable();
            }
        }

        public void Execute(float deltaTime)
        {
            if (_isFirstPlayer)
            {
                _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * _playerMoveSpeed * Time.deltaTime);
            }
            else
            {
                _playerTransform.Translate(_playerInput.PlayerTwo.Move.ReadValue<Vector2>() * _playerMoveSpeed * Time.deltaTime);
            }
        }
    }
}
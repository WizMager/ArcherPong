using Controllers.Interfaces;
using Photon.Pun;
using UnityEngine;
using Views;

namespace Controllers
{
    public class PlayerMoveController : IStart,IEnable, IDisable, ICleanup, IExecute, IAwake
    {
        private ArrowControllerOld _arrowControllerOld;
        private readonly Transform _playerTransform;
        private readonly float _playerMoveSpeed;
        private readonly PhotonView _photonView;
        private readonly PlayerView _playerView;
        private PlayerInput _playerInput;
        private readonly Vector2 _startPosition;
        private readonly Quaternion _startRotation;

        public PlayerMoveController(GameObject player, float playerMoveSpeed, PhotonView photonView)
        {
            _playerTransform = player.GetComponent<Transform>();
            _playerView = player.GetComponent<PlayerView>();
            _playerMoveSpeed = playerMoveSpeed;
            _photonView = photonView;
            _startPosition = _playerTransform.position;
            _startRotation = _playerTransform.rotation;
        }

        public void GetArrowController(ArrowControllerOld arrowControllerOld)
        {
            _arrowControllerOld = arrowControllerOld;
        }
        
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }
        
        public void Start()
        {
            _arrowControllerOld.OnArrowMiss += ArrowMissed;
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
            _arrowControllerOld.OnArrowMiss -= ArrowMissed;
        }

        public void OnEnable()
        {
            //if (!_photonView.IsMine) return;
            _playerInput.Player.Move.Enable();
        }

        public void OnDisable()
        {
            //if (!_photonView.IsMine) return;
            _playerInput.Player.Move.Disable();
        }

        public void Execute(float deltaTime)
        {
            if (!_photonView.IsMine) return;
            _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * _playerMoveSpeed * Time.deltaTime);
        }
    }
}
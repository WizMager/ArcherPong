using Controllers.Interfaces;
using Data;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Views;

namespace Controllers.MultiPlayer
{
    public class ArrowController : IStart, ILateExecute, ICleanup, IOnEventCallback
    {
        private readonly ArrowView _arrowView;
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _transform;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly float _startArrowMoveSpeed;
        private readonly float _maxArrowMoveSpeed;
        private readonly int _arrowMoveSpeedMultiply;
        private PlayerShootController _shootController;
        private ScoreController _scoreController;

        private Vector2 _currentVelocity;
        private float _arrowMoveSpeed;

        public ArrowController(ArrowView arrowView, ArrowData arrowData)
        {
            _arrowView = arrowView;
            _rigidbody = _arrowView.GetRigidbody;
            _transform = _arrowView.GetTransform;
            _spriteRenderer = _arrowView.GetSpriteRenderer;
            _startArrowMoveSpeed = arrowData.startArrowSpeed;
            _maxArrowMoveSpeed = arrowData.maxArrowSpeed;
            _arrowMoveSpeedMultiply = arrowData.arrowMoveSpeedMultiplyPercent;
            SetStartArrowMoveSpeed();
            
            _arrowView.OnCatch += CatchHandler;
            _arrowView.OnMiss += OnMissHandler;
            if (!PhotonNetwork.IsMasterClient) return;
            _arrowView.OnReflect += OnReflectHandler;
        }

        public void Init(PlayerShootController shootController, ScoreController scoreController)
        {
            _shootController = shootController;
            _scoreController = scoreController;
            
            _shootController.OnShoot += OnShootHandler;
            if (!PhotonNetwork.IsMasterClient) return;
            _scoreController.OnGamePause += OnGamePauseHandler;
        }

        private void OnReflectHandler(Vector2 normal)
        {
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _rigidbody.velocity = direction * _arrowMoveSpeed;
            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) * _transform.rotation;
        }
        
        private void CatchHandler(bool isFirstPlayer)
        {
            ArrowDisable();
            if (!PhotonNetwork.IsMasterClient) return;
            AddArrowSpeed();
        }
        
        private void OnMissHandler(bool isFirstPlayer)
        {
            ArrowDisable();
            if (!PhotonNetwork.IsMasterClient) return;
            SetStartArrowMoveSpeed();
        }
        
        private void OnShootHandler(bool isFirstPlayer)
        {
            ArrowEnable();
        }
        
        private void OnGamePauseHandler(bool isPause)
        {
            if (!isPause) return;
            SetStartArrowMoveSpeed();
        }
        
        public void OnEvent(EventData photonEvent)
        {
            
        }
        
        public void Start()
        {
            ArrowDisable();
        }

        private void SetStartArrowMoveSpeed()
        {
            _arrowMoveSpeed = _startArrowMoveSpeed;
        }

        private void AddArrowSpeed()
        {
            if (_arrowMoveSpeed > _maxArrowMoveSpeed)return;
            _arrowMoveSpeed += _arrowMoveSpeed / 100 * _arrowMoveSpeedMultiply;
        }

        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            if (!PhotonNetwork.IsMasterClient) return;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }

        private void ArrowEnable()
        {
            _spriteRenderer.enabled = true;
            if (!PhotonNetwork.IsMasterClient) return;
            _rigidbody.AddForce(_transform.up * (_arrowMoveSpeed * _rigidbody.mass), ForceMode2D.Impulse);
        }

        public void LateExecute(float deltaTime)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _currentVelocity = _rigidbody.velocity; 
        }

        public void Cleanup()
        {
            _arrowView.OnReflect -= OnReflectHandler;
            _arrowView.OnCatch -= CatchHandler;
            _arrowView.OnMiss -= OnMissHandler;
            _shootController.OnShoot -= OnShootHandler;
            _scoreController.OnGamePause -= OnGamePauseHandler;
        }
    }
}
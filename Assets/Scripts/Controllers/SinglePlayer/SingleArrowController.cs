using Controllers.Interfaces;
using Data;
using UnityEngine;
using Views;

namespace Controllers.SinglePlayer
{
    public class SingleArrowController : IStart, ILateExecute, ICleanup
    {
        private readonly SingleArrowView _arrowView;
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _transform;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly float _startArrowMoveSpeed;
        private readonly float _maxArrowMoveSpeed;
        private readonly int _arrowMoveSpeedMultiply;
        private readonly Transform _shootPosition;
        private SinglePlayerShootController _shootController;
        private SingleScoreController _scoreController;

        private Vector2 _currentVelocity;
        private float _arrowMoveSpeed;

        public SingleArrowController(SingleArrowView arrowView, ArrowData arrowData, Transform playerShootPosition)
        {
            _arrowView = arrowView;
            _rigidbody = _arrowView.GetRigidbody;
            _transform = _arrowView.GetTransform;
            _spriteRenderer = _arrowView.GetSpriteRenderer;
            _startArrowMoveSpeed = arrowData.startArrowSpeed;
            _maxArrowMoveSpeed = arrowData.maxArrowSpeed;
            _arrowMoveSpeedMultiply = arrowData.arrowMoveSpeedMultiplyPercent;
            _shootPosition = playerShootPosition;
            SetStartArrowMoveSpeed();
            
            _arrowView.OnReflect += OnReflectHandler;
            _arrowView.OnCatch += CatchHandler;
            _arrowView.OnMiss += OnMissHandler;
        }

        public void Init(SinglePlayerShootController shootController, SingleScoreController scoreController)
        {
            _shootController = shootController;
            _scoreController = scoreController;
            
            _shootController.OnShoot += OnShootHandler;
            _scoreController.OnGamePause += OnGamePauseHandler;
        }

        private void OnReflectHandler(Vector2 normal, bool isBot)
        {
            if (isBot)
            {
                AddArrowSpeed();
            }
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _rigidbody.velocity = direction * _arrowMoveSpeed;
            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) * _transform.rotation;
        }
        
        private void CatchHandler()
        {
            AddArrowSpeed();
            ArrowDisable();
        }
        
        private void OnMissHandler(bool isFirstPlayer)
        {
            SetStartArrowMoveSpeed();
            ArrowDisable();
        }
        
        private void OnShootHandler()
        {
            _transform.SetPositionAndRotation(_shootPosition.position, _shootPosition.rotation);
            ArrowEnable();
        }
        
        private void OnGamePauseHandler(bool isPause)
        {
            if (!isPause) return;
            SetStartArrowMoveSpeed();
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
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }

        private void ArrowEnable()
        {
            _spriteRenderer.enabled = true;
            _rigidbody.AddForce(_transform.up * (_arrowMoveSpeed * _rigidbody.mass), ForceMode2D.Impulse);
        }

        public void LateExecute(float deltaTime)
        {
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
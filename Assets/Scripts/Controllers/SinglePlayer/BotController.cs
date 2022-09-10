using Controllers.Interfaces;
using Data;
using UnityEngine;
using Views;

namespace Controllers.SinglePlayer
{
    public class BotController : IExecute, ILateExecute, ICleanup
    {
        private readonly Transform _botTransform;
        private readonly Transform _arrow;
        private readonly float _botSpeed;
        private readonly float _distanceToCalculateDirection;
        private readonly SingleArrowView _arrowView;
        private readonly Vector2 _startPosition;
        private readonly Quaternion _startRotation;
        private SingleScoreController _scoreController;
        private float _calculatedHorizontalDirection;
        private bool _isStopMove;

        public BotController(Transform bot, Transform arrow, BotData botData, SingleArrowView arrowView)
        {
            _botTransform = bot;
            _arrow = arrow;
            _botSpeed = botData.botSpeed;
            _distanceToCalculateDirection = botData.distanceToCalculateDirection;
            _arrowView = arrowView;
            _startPosition = _botTransform.position;
            _startRotation = _botTransform.rotation;

            _arrowView.OnMiss += OnMissHandler;
        }

        public void Init(SingleScoreController scoreController)
        {
            _scoreController = scoreController;
            _scoreController.OnGamePause += OnGamePauseHandler;
        }

        private void OnMissHandler(bool isFirstPlayer)
        {
            ReturnToStartPosition();
            _calculatedHorizontalDirection = 0;
        }
    
        private void OnGamePauseHandler(bool isStopMove)
        {
            _isStopMove = isStopMove;
        }

        private void ReturnToStartPosition()
        {
            _botTransform.position = _startPosition;
            _botTransform.rotation = _startRotation;
        }

        private float CalculateMoveDirection()
        {
            var distanceToArrow = Vector2.Distance(_botTransform.position, _arrow.position);
            if (distanceToArrow > _distanceToCalculateDirection)
            {
                return 0;
            }
            var directionToArrow = (_arrow.position - _botTransform.position).normalized;
            var angleBetweenDown = Mathf.Atan2(directionToArrow.y, directionToArrow.x) * Mathf.Rad2Deg + 90f;
            if (angleBetweenDown is > 90f or < -90f)
            {
                return 0;
            }
            return Mathf.Lerp(-1f, 1f, angleBetweenDown);
        }

        public void Execute(float deltaTime)
        {
            if (_isStopMove) return;
            var axisXChange = _calculatedHorizontalDirection * _botSpeed * deltaTime;
            _botTransform.Translate(axisXChange, 0f, 0f);
        }

        public void LateExecute(float deltaTime)
        {
            _calculatedHorizontalDirection = CalculateMoveDirection();
        }

        public void Cleanup()
        {
            _arrowView.OnMiss -= OnMissHandler;
            _scoreController.OnGamePause -= OnGamePauseHandler;
        }
    }
}
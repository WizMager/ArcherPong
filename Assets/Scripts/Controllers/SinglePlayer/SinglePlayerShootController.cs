﻿using System;
using System.Collections;
using Controllers.Interfaces;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

namespace Controllers.SinglePlayer
{
    public class SinglePlayerShootController : IAwake, IEnable, IExecute, IDisable, ICleanup
    {
        public Action OnShoot;
        private readonly Camera _mainCamera;
        private readonly ShootlessAreaView _shootlessArea;
        private readonly Transform _stickPosition;
        private readonly SingleArrowView _arrowView;
        private readonly Transform _bowBody;
        private readonly SpriteRenderer _bowArrow;
        private readonly float[] _clampValue;
        private readonly float[] _clampEqualizer;
        private readonly float _startTimeBeforeShoot;
        private readonly float _minimalTimeBeforeShoot;
        private readonly int _timeBeforeShootMultiply;
        private SingleScoreController _scoreController;

        private bool _hasArrow = true;
        private PlayerInput _playerInput;
        private bool _canShoot = true;
        private bool _timerEnable;
        private float _timeBeforeShoot;

        public SinglePlayerShootController(Camera mainCamera, ShootlessAreaView shootlessAreaView, Transform stickPosition, PlayerView playerView, SingleArrowView arrowView, PlayerData playerData)
        {
            _mainCamera = mainCamera;
            _shootlessArea = shootlessAreaView;
            _stickPosition = stickPosition;
            _arrowView = arrowView;
            _bowBody = playerView.GetBowBody;
            _bowArrow = playerView.GetBowArrow;
            _clampValue = playerData.clampValueFirstPlayer;
            _clampEqualizer = playerData.clamValueEqualizerFirstPlayer;
            _startTimeBeforeShoot = playerData.startTimeBeforeShoot;
            _minimalTimeBeforeShoot = playerData.minimalTimeBeforeShoot;
            _timeBeforeShootMultiply = playerData.timeBeforeShootMultiply;

            _shootlessArea.OnShootActivator += ShootStateChanged;
            _arrowView.OnCatch += OnCaught;
            _arrowView.OnMiss += OnMissed;
        }

        public void Init(SingleScoreController scoreController)
        {
            _scoreController = scoreController;
            _scoreController.OnGamePause += OnGamePaused;
        }

        private void ShootStateChanged(bool shootActivate)
        {
            _canShoot = shootActivate;
            if (_timerEnable)
            {
                _arrowView.StartCoroutine(TimeBeforeShoot(_timeBeforeShoot)); 
            }
        }
    
        private void OnMissed(bool isFirstPlayer)
        {
            SetStartTimeBeforeShoot();
            TakeArrow(_timerEnable, _timeBeforeShoot);
        }

        private void OnCaught()
        {
            RemoveTimeBeforeShoot();
            _timerEnable = true;
            TakeArrow(_timerEnable, _timeBeforeShoot);
        }
    
        private void OnGamePaused(bool isStop)
        {
            _canShoot = !isStop;
        }
    
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }

        public void OnEnable()
        {
            _playerInput.Player.Aiming.Enable();
            _playerInput.Player.Touch.Enable();
            _playerInput.Player.Touch.canceled += Shoot;
        }

        public void Execute(float deltaTime)
        {
            if (!_hasArrow) return;
            if (_playerInput.Player.Touch.phase != InputActionPhase.Performed) return;
            Aiming();
        }

        private void TakeArrow(bool timerEnable, float timeBeforeShoot)
        {
            _hasArrow = true;
            _bowArrow.enabled = _hasArrow;
            if (!timerEnable)return;
            if (!_canShoot)
            {
                _timeBeforeShoot = timeBeforeShoot;
                _timerEnable = true;
                return;
            }
            _arrowView.StartCoroutine(TimeBeforeShoot(timeBeforeShoot));
        }
    
        private void Aiming()
        {
            var touchPosition = _playerInput.Player.Aiming.ReadValue<Vector2>();
            var worldTouchPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
            var direction = worldTouchPosition - _stickPosition.position;
            direction.Normalize();
            var angleAxisZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            var angleAxisZClamped = Math.Clamp(angleAxisZ, _clampValue[0], _clampValue[1]); 
            if (angleAxisZ > _clampEqualizer[0] && angleAxisZ < _clampEqualizer[1])
            {
                angleAxisZClamped = _clampValue[1];
            }
            _bowBody.rotation = Quaternion.Euler(0f, 0f, angleAxisZClamped);
        }
    
        private void Shoot(InputAction.CallbackContext obj)
        {
            if (!_hasArrow) return;
            if (!_canShoot) return;
            OnShoot?.Invoke();
            _hasArrow = false;
        }
    
        private IEnumerator TimeBeforeShoot(float timeBeforeShoot)
        {
            for (float i = 0; i < timeBeforeShoot; i += Time.deltaTime)
            {
                yield return null;
            }
            Shoot(new InputAction.CallbackContext());
            _timerEnable = false;
            _arrowView.StopCoroutine(TimeBeforeShoot(0));
        }
    
        private void SetStartTimeBeforeShoot()
        {
            _timeBeforeShoot = _startTimeBeforeShoot;
        }
    
        private void RemoveTimeBeforeShoot()
        {
            if (_timeBeforeShoot > _minimalTimeBeforeShoot)
            {
                _timeBeforeShoot -= _timeBeforeShoot / 100 * _timeBeforeShootMultiply;
            }
        }
    
        public void OnDisable()
        {
            _playerInput.Player.Shoot.performed -= Shoot;
            _playerInput.Player.Aiming.Disable();
            _playerInput.Player.Touch.Disable();
        }
    
        public void Cleanup()
        {
            _shootlessArea.OnShootActivator -= ShootStateChanged;
            _arrowView.OnCatch -= OnCaught;
            _arrowView.OnMiss -= OnMissed;
        }
    }
}
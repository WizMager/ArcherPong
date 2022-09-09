using System;
using System.Collections;
using Controllers.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class SingleShootController : IAwake, IStart, IEnable, IExecute, IDisable, ICleanup
{
    public Action OnShoot;
    private readonly Camera _mainCamera;
    private ShootlessAreaView _shootlessArea;
    private Transform _bow;
    private SpriteRenderer _bowArrow;
    private Transform _shootPosition;
    private float[] _clampValue;
    private float[] _clampEqualizer;
    private Transform _stickTransform;
    private PlayerView _playerView;
    private bool _hasArrow;
    private PlayerInput _playerInput;
    private bool _canShoot = true;
    private bool _timeEnable;
    private float _timeBeforeShoot;

    public Transform GetShootPosition => _shootPosition;
    
    public SingleShootController(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }
    
    public void Awake()
    {
        _playerInput = new PlayerInput();
    }

    public void Start()
    {
        TakeArrow(true, (false, 0f));
        _shootlessArea.OnShootActivator += ShootStateChanged;
    }
    
    public void OnEnable()
    {
        _playerInput.Player.Aiming.Enable();
        _playerInput.Player.Touch.Enable();
        _playerInput.Player.Touch.canceled += Shoot;
    }

    public void OnDisable()
    {
        _playerInput.Player.Shoot.performed -= Shoot;
        _playerInput.Player.Aiming.Disable();
        _playerInput.Player.Touch.Disable();
    }
    
    public void Execute(float deltaTime)
    {
        
    }
    
    private void ShootStateChanged(bool shootActivate)
    {
        _canShoot = shootActivate;
        if (_timeEnable)
        {
            _playerView.StartCoroutine(TimeBeforeShoot(_timeBeforeShoot)); 
        }
    }
    
    public void TakeArrow(bool hasArrow, (bool timerEnable, float timeBeforeShoot) timer)
    {
        _hasArrow = hasArrow;
        _bowArrow.enabled = _hasArrow;
        if (!timer.timerEnable)return;
        if (!_canShoot)
        {
            _timeBeforeShoot = timer.timeBeforeShoot;
            _timeEnable = true;
            return;
        }
        _playerView.StartCoroutine(TimeBeforeShoot(timer.timeBeforeShoot));
    }
    
    private void Aiming()
    {
        if (!_hasArrow) return;
        if (_playerInput.Player.Touch.phase != InputActionPhase.Performed) return;
        var touchPosition = _playerInput.Player.Aiming.ReadValue<Vector2>();
        var worldTouchPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
        var direction = worldTouchPosition - _stickTransform.position;
        direction.Normalize();
        var angleAxisZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        var angleAxisZClamped = Math.Clamp(angleAxisZ, _clampValue[0], _clampValue[1]); 
        if (angleAxisZ > _clampEqualizer[0] && angleAxisZ < _clampEqualizer[1])
        {
            angleAxisZClamped = _playerView.IsFirstPlayer ? _clampValue[1] : _clampValue[0];
        }
        _bow.rotation = Quaternion.Euler(0f, 0f, angleAxisZClamped);
    }
    
    private void Shoot(InputAction.CallbackContext obj)
    {
        if (!_hasArrow) return;
        if (!_canShoot) return;
        OnShoot?.Invoke();
    }
    
    private IEnumerator TimeBeforeShoot(float timeBeforeShoot)
    {
        for (float i = 0; i < timeBeforeShoot; i += Time.deltaTime)
        {
            yield return null;
        }
        Shoot(new InputAction.CallbackContext());
        _timeEnable = false;
        _playerView.StopCoroutine(TimeBeforeShoot(0));
    }
    
    public void Cleanup()
    {
        _shootlessArea.OnShootActivator -= ShootStateChanged;
    }
}
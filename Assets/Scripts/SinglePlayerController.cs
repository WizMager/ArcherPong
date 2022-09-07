using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class SinglePlayerController : MonoBehaviour
{
    public Action OnShoot;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private Transform bow;
    [SerializeField] private SpriteRenderer bowArrow;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float[] clampValue;
    [SerializeField] private float[] clampEqualizer;
    [SerializeField] private ShootlessPositionView shootless;
    [SerializeField] private Transform stickTransform;
    private Transform _playerTransform;
    private GameObject _wallColliders;
    private Camera _mainCamera;
    private PlayerInput _playerInput;
    private PlayerView _playerView;
    private bool _hasArrow;
    private Vector2 _startPosition;
    private Quaternion _startRotation;
    private bool _stopMove;
    private bool _canShoot = true;
    private bool _timeEnable;
    private float _timeBeforeShoot;

    public Transform GetShootPosition => shootPosition;

    public bool StopMove
    {
        set => _stopMove = value;
    }
    
    private void Awake()
    {
        _playerView = GetComponent<PlayerView>();
        _playerTransform = GetComponent<Transform>();
        _playerInput = new PlayerInput();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        TakeArrow(true, (false, 0f));
        _startPosition = _playerTransform.position;
        _startRotation = _playerTransform.rotation;
        _playerView.OnWallEnter += WallEntered;
        shootless.OnShootActivator += ShootStateChanged;
    }

    private void ShootStateChanged(bool shootActivate)
    {
        _canShoot = shootActivate;
        if (_timeEnable)
        {
            StartCoroutine(TimeBeforeShoot(_timeBeforeShoot)); 
        }
    }

    public void SetStartPosition()
    {
        _playerTransform.position = _startPosition;
        _playerTransform.rotation = _startRotation;
    }

    public void TakeArrow(bool hasArrow, (bool timerEnable, float timeBeforeShoot) timer)
    {
        _hasArrow = hasArrow;
        bowArrow.enabled = _hasArrow;
        if (!timer.timerEnable)return;
        if (!_canShoot)
        {
            _timeBeforeShoot = timer.timeBeforeShoot;
            _timeEnable = true;
            return;
        }
        StartCoroutine(TimeBeforeShoot(timer.timeBeforeShoot));
    }

    private IEnumerator TimeBeforeShoot(float timeBeforeShoot)
    {
        for (float i = 0; i < timeBeforeShoot; i += Time.deltaTime)
        {
            yield return null;
        }
        Shoot(new InputAction.CallbackContext());
        _timeEnable = false;
        StopAllCoroutines();
    }
    
    private void WallEntered(Vector3 normal)
    {
        _playerTransform.Translate(normal * playerMoveSpeed * Time.deltaTime);
    }
    
    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Aiming.Enable();
        _playerInput.Player.Shoot.Enable();
        _playerInput.Player.Touch.Enable();
        _playerInput.Player.Touch.canceled += Shoot;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (_stopMove) return;
        if (!_hasArrow) return;
        if (!_canShoot) return;
        OnShoot?.Invoke();
    }

    private void Aiming()
    {
        if (!_hasArrow) return;
        if (_playerInput.Player.Touch.phase != InputActionPhase.Performed) return;
        var touchPosition = _playerInput.Player.Aiming.ReadValue<Vector2>();
        var worldTouchPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
        var direction = worldTouchPosition - stickTransform.position;
        direction.Normalize();
        var angleAxisZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        var angleAxisZClamped = Math.Clamp(angleAxisZ, clampValue[0], clampValue[1]); 
        if (angleAxisZ > clampEqualizer[0] && angleAxisZ < clampEqualizer[1])
        {
            angleAxisZClamped = _playerView.IsFirstPlayer ? clampValue[1] : clampValue[0];
        }
        bow.rotation = Quaternion.Euler(0f, 0f, angleAxisZClamped);
    }

    private void Move()
    {
        if (_canShoot && _hasArrow) return;
        _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * playerMoveSpeed * Time.deltaTime);
    }
    
    private void Update()
    {
        if (_stopMove) return;
        Aiming();
        Move();
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Aiming.Disable();
        _playerInput.Player.Shoot.Disable();
        _playerInput.Player.Touch.Enable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnDestroy()
    {
        _playerView.OnWallEnter -= WallEntered;
        _playerInput.Player.Shoot.performed -= Shoot;
    }
}
using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Views;

public class PlayerController : MonoBehaviour, IOnEventCallback
{
    public Action<bool> OnShoot;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private Transform bow;
    [SerializeField] private SpriteRenderer bowArrow;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float[] clampValue;
    [SerializeField] private float[] clampEqualizer;
    [SerializeField] private Sprite playerFront;
    [SerializeField] private GameObject playerSprite;
    private Transform _playerTransform;
    private GameObject _wallColliders;
    private Camera _mainCamera;
    private PlayerInput _playerInput;
    private PhotonView _photonView;
    private PlayerView _playerView;
    private bool _hasArrow;
    private Vector2 _startPosition;
    private Quaternion _startRotation;
    private bool _stopMove;
    private SpriteRenderer _spriteRenderer;
    private Transform _stickTransform;
    private ShootlessPositionView _shootless;
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
        _photonView = GetComponent<PhotonView>();
        _playerView = GetComponent<PlayerView>();
        _spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        _playerTransform = GetComponent<Transform>();
        if (!_photonView.IsMine) return;
        _playerInput = new PlayerInput();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        FindObjectOfType<ArrowController>().AddPlayerController(this);
        FindObjectOfType<ScoreController>().AddPlayerController(this);
        _stickTransform = FindObjectOfType<JoystickPositionView>().gameObject.transform;
        _shootless = FindObjectOfType<ShootlessPositionView>();
        TakeArrow(_playerView.IsFirstPlayer, (false, 0f));
        _startPosition = _playerTransform.position;
        _startRotation = _playerTransform.rotation;
        if (_photonView.IsMine)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                _wallColliders = FindObjectOfType<WallCollidersView>().gameObject;
                _mainCamera.transform.Rotate(0, 0, 180f);
                _wallColliders.transform.Rotate(0, 0, 180f);
            }
            _playerView.OnWallEnter += WallEntered;
            _shootless.OnShootActivator += ShootStateChanged;
        }
        else
        {
            _spriteRenderer.sprite = playerFront;
            _spriteRenderer.flipY = true;
            _spriteRenderer.sortingOrder = 5;
            playerSprite.transform.localPosition = new Vector3(0, -0.1f, 0);
        }
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
        if (!_photonView.IsMine) return;
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Aiming.Enable();
        _playerInput.Player.Shoot.Enable();
        _playerInput.Player.Touch.Enable();
        _playerInput.Player.Touch.canceled += Shoot;
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (int)PhotonEventCode.PlayerShoot:
                if (!_photonView.IsMine) return;
                OnShoot?.Invoke((bool)photonEvent.CustomData);
                break;
        }
    }
    
    private void Shoot(InputAction.CallbackContext obj)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2 || _stopMove) return;
        if (!_photonView.IsMine) return;
        if (!_hasArrow) return;
        if (!_canShoot) return;
        PhotonNetwork.RaiseEvent((int)PhotonEventCode.PlayerShoot, _playerView.IsFirstPlayer, new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient},
            SendOptions.SendReliable);
    }

    private void Aiming()
    {
        if (!_photonView.IsMine) return;
        if (!_hasArrow) return;
        if (_playerInput.Player.Touch.phase != InputActionPhase.Performed) return;
        var touchPosition = _playerInput.Player.Aiming.ReadValue<Vector2>();
        var worldTouchPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
        var direction = worldTouchPosition - _stickTransform.position;
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
        
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2 || _stopMove) return;
        if (!_photonView.IsMine) return;
        Aiming();
        Move();
    }

    private void OnDisable()
    {
        if (!_photonView.IsMine) return;
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Aiming.Disable();
        _playerInput.Player.Shoot.Disable();
        _playerInput.Player.Touch.Enable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnDestroy()
    {
        if (!_photonView.IsMine) return;
        _playerView.OnWallEnter -= WallEntered;
        _playerInput.Player.Shoot.performed -= Shoot;
    }
}
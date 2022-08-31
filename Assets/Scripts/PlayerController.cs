using System;
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
    [SerializeField]private float[] clampValue;
    [SerializeField]private float[] clampEqualizer;
    private Camera _mainCamera;
    private PlayerInput _playerInput;
    private PhotonView _photonView;
    private PlayerView _playerView;
    private bool _hasArrow;
    private Vector2 _startPosition;
    private Quaternion _startRotation;

    public Transform GetShootPosition => shootPosition;
    
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _photonView = GetComponent<PhotonView>();
        _playerView = GetComponent<PlayerView>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        FindObjectOfType<ArrowController>().AddPlayerController(this);
        TakeArrow(_playerView.IsFirstPlayer);
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        if (!_photonView.IsMine) return;
        _playerView.OnWallEnter += WallEntered;
    }

    public void SetStartPosition()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }
    
    public void TakeArrow(bool hasArrow)
    {
        _hasArrow = hasArrow;
        bowArrow.enabled = _hasArrow;
    }
    
    private void WallEntered(Vector3 normal)
    {
        transform.Translate(normal * playerMoveSpeed * Time.deltaTime);
    }
    
    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Aiming.Enable();
        _playerInput.Player.Shoot.Enable();
        _playerInput.Player.Aiming.performed += Aiming;
        _playerInput.Player.Shoot.performed += Shoot;
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
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;
        if (!_photonView.IsMine) return;
        if (!_hasArrow) return;
        PhotonNetwork.RaiseEvent((int)PhotonEventCode.PlayerShoot, _playerView.IsFirstPlayer, new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient},
            SendOptions.SendReliable);
    }

    private void Aiming(InputAction.CallbackContext aiming)
    {
        if (!_photonView.IsMine) return;
        var mousePosition = aiming.ReadValue<Vector2>();
        var worldMousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        var mouseDirection = worldMousePosition - transform.position;
        mouseDirection.Normalize();
        var angleAxisZ = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
        var angleAxisZClamped = Math.Clamp(angleAxisZ, clampValue[0], clampValue[1]);
        if (angleAxisZ > clampEqualizer[0] && angleAxisZ < clampEqualizer[1])
        {
            angleAxisZClamped = _playerView.IsFirstPlayer ? clampValue[1] : clampValue[0];
        }
        bow.rotation = Quaternion.Euler(0f, 0f, angleAxisZClamped);
    }
    
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;
        if (!_photonView.IsMine) return;
        transform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * playerMoveSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Aiming.Disable();
        _playerInput.Player.Shoot.Disable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnDestroy()
    {
        _playerView.OnWallEnter -= WallEntered;
        _playerInput.Player.Aiming.performed -= Aiming;
        _playerInput.Player.Shoot.performed -= Shoot;
    }
}
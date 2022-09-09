using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class SinglePlayerController : MonoBehaviour
{
    private float _playerMoveSpeed;
    private Transform _playerTransform;
    private GameObject _wallColliders;
    private PlayerInput _playerInput;
    private PlayerView _playerView;
    private Vector2 _startPosition;
    private Quaternion _startRotation;
    private bool _stopMove;

    public bool StopMove
    {
        set => _stopMove = value;
    }
    
    private void Awake()
    {
        //_playerView = GetComponent<PlayerView>();
        //_playerTransform = GetComponent<Transform>();
        _playerInput = new PlayerInput();
    }

    private void Start()
    {
        
        _startPosition = _playerTransform.position;
        _startRotation = _playerTransform.rotation;
        _playerView.OnWallEnter += WallEntered;
    }

    public void SetStartPosition()
    {
        _playerTransform.position = _startPosition;
        _playerTransform.rotation = _startRotation;
    }

    private void WallEntered(Vector3 normal)
    {
        _playerTransform.Translate(normal * _playerMoveSpeed * Time.deltaTime);
    }
    
    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
    }

    private void Update()
    {
        if (_stopMove) return;
        _playerTransform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * _playerMoveSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
    }

    private void OnDestroy()
    {
        _playerView.OnWallEnter -= WallEntered;
    }
}
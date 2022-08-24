using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private Transform bow;
    [SerializeField] private SpriteRenderer bowArrow;
    private Camera _mainCamera;
    private PlayerInput _playerInput;
    private PhotonView _photonView;
    private PlayerView _playerView;
    private bool _hasArrow;
        
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _photonView = GetComponent<PhotonView>();
        _playerView = GetComponent<PlayerView>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _playerView.OnWallEnter += WallEntered;
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
        _playerInput.Player.Aiming.performed += Aiming;
    }

    private void Aiming(InputAction.CallbackContext aiming)
    {
        if (!_photonView.IsMine) return;
        var mousePosition = aiming.ReadValue<Vector2>();
        var worldMousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        var mouseDirection = worldMousePosition - transform.position;
        mouseDirection.Normalize();
        var angleAxisZ = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
        bow.rotation = Quaternion.Euler(0f, 0f, angleAxisZ);
    }
    
    private void Update()
    {
        if (!_photonView.IsMine) return;
        transform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * playerMoveSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Aiming.Disable();
    }

    private void OnDestroy()
    {
        _playerView.OnWallEnter -= WallEntered;
        _playerInput.Player.Aiming.performed -= Aiming;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject bowArrow;
    [SerializeField] private Arrow arrow;
    private bool _isFirstPlayer;
    private PlayerInput _playerInput;
    private bool _hasArrow = true;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void Start()
    {
        _isFirstPlayer = GetComponent<PlayerView>().IsFirstPlayer;
        
        arrow.OnCatchArrow += ArrowCaught;
        
        if (_isFirstPlayer)
        {
            arrow.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _playerInput.Player.Shoot.Enable();

        _playerInput.Player.Shoot.performed += Shoot;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (!_hasArrow) return;
        arrow.transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
        arrow.gameObject.SetActive(true);
        _hasArrow = false;
        bowArrow.SetActive(false);
    }

    private void ArrowCaught(bool isFirstPlayer)
    {
        if (isFirstPlayer == _isFirstPlayer)
        {
            arrow.gameObject.SetActive(false);
            _hasArrow = true;
            bowArrow.SetActive(true);
        }
        else
        {
            arrow.gameObject.SetActive(false);
            _hasArrow = true;
            bowArrow.SetActive(true);
        }
    }

    private void OnDisable()
    {
        _playerInput.Player.Shoot.Disable();
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject bowArrow;
    private PlayerInput _playerInput;
    private bool _hasArrow = true;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Player.Shoot.Enable();

        _playerInput.Player.Shoot.performed += Shoot;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        if (!_hasArrow) return;
        Instantiate(arrow, shootPosition.position, shootPosition.rotation);
        _hasArrow = false;
        bowArrow.SetActive(false);
    }

    public void CatchArrow()
    {
        _hasArrow = true;
        bowArrow.SetActive(true);
    }

    private void OnDisable()
    {
        _playerInput.Player.Shoot.Disable();
    }
}
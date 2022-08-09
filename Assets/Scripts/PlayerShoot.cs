using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform shootPosition;
    private PlayerInput _playerInput;

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
        Instantiate(arrow, shootPosition.position, shootPosition.rotation);
    }

    private void OnDisable()
    {
        _playerInput.Player.Shoot.Disable();
    }
}
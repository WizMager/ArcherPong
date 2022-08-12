using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isPlayer1;
    private PlayerInput _playerInput;
    
    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        if (isPlayer1)
        {
            _playerInput.Player.Move.Enable(); 
        }
        else
        {
            _playerInput.PlayerTwo.Move.Enable();
        }
    }

    private void OnDisable()
    {
        if (isPlayer1)
        {
            _playerInput.Player.Move.Disable(); 
        }
        else
        {
            _playerInput.PlayerTwo.Move.Disable();
        }
    }

    private void Update()
    {
        if (isPlayer1)
        {
            transform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(_playerInput.PlayerTwo.Move.ReadValue<Vector2>() * moveSpeed * Time.deltaTime);
        }
    }
}
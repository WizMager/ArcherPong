using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isFirstPlayer;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        if (isFirstPlayer)
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
        if (isFirstPlayer)
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
        if (isFirstPlayer)
        {
            transform.Translate(_playerInput.Player.Move.ReadValue<Vector2>() * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(_playerInput.PlayerTwo.Move.ReadValue<Vector2>() * moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var colliderTag = col.gameObject.tag;
        
        if (colliderTag == "Wall")
        {
            transform.Translate(col.contacts[0].normal * moveSpeed * Time.deltaTime);
        }

        if (colliderTag == "PlayerWallLimiter")
        {
            transform.Translate(col.contacts[0].normal * moveSpeed * Time.deltaTime);
        }
    }
}
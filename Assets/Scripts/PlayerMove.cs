using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private bool _isFirstPlayer;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _isFirstPlayer = GetComponent<PlayerView>().IsFirstPlayer;
    }

    private void OnEnable()
    {
        if (_isFirstPlayer)
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
        if (_isFirstPlayer)
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
        if (_isFirstPlayer)
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
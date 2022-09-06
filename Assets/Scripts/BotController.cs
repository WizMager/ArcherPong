using UnityEngine;
using Views;

public class BotController : MonoBehaviour
{
    private PlayerView _playerView;
    private Transform _botTransform;
    private bool _stopMove;
    private Vector2 _startPosition;
    private Quaternion _startRotation;
    
    public bool StopMove
    {
        set => _stopMove = value;
    }

    private void Start()
    {
        _playerView = GetComponent<PlayerView>();
        _botTransform = GetComponent<Transform>();
        _startPosition = _botTransform.position;
        _startRotation = _botTransform.rotation;
    }

    public void SetStartPosition()
    {
        _botTransform.position = _startPosition;
        _botTransform.rotation = _startRotation;
    }
}
using Controllers.Interfaces;
using Data;
using UnityEngine;

public class BotController : IExecute
{
    private readonly Transform _arrow;
    private readonly float _botSpeed;
    private readonly float _distanceToCalculateDirection;
    private readonly Transform _botTransform;
    private bool _stopMove;
    private readonly Vector2 _startPosition;
    private readonly Quaternion _startRotation;
    
    public bool StopMove
    {
        set => _stopMove = value;
    }

    public BotController(Transform bot, Transform arrow, BotData botData)
    {
        _botTransform = bot;
        _arrow = arrow;
        _botSpeed = botData.botSpeed;
        _distanceToCalculateDirection = botData.distanceToCalculateDirection;
        _startPosition = _botTransform.position;
        _startRotation = _botTransform.rotation;
    }

    public void SetStartPosition()
    {
        _botTransform.position = _startPosition;
        _botTransform.rotation = _startRotation;
    }

    private float CalculateMoveDirection()
    {
        var distanceToArrow = Vector2.Distance(_botTransform.position, _arrow.position);
        if (distanceToArrow > _distanceToCalculateDirection)
        {
            return 0;
        }
        var directionToArrow = (_arrow.position - _botTransform.position).normalized;
        var angleBetweenDown = Mathf.Atan2(directionToArrow.y, directionToArrow.x) * Mathf.Rad2Deg + 90f;
        if (angleBetweenDown is > 90f or < -90f)
        {
            angleBetweenDown = 0;
        }
        return Mathf.Lerp(-1f, 1f, angleBetweenDown);;
    }

    public void Execute(float deltaTime)
    {
        if (_stopMove) return;
        var axisXChange = CalculateMoveDirection() * _botSpeed * deltaTime;
        _botTransform.Translate(axisXChange, 0f, 0f);
    }
}
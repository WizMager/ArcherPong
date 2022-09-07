using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    [SerializeField] private float botSpeed;
    [SerializeField] private float minimalDistanceToCalculate;
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
        _botTransform = GetComponent<Transform>();
        _startPosition = _botTransform.position;
        _startRotation = _botTransform.rotation;
    }

    public void SetStartPosition()
    {
        _botTransform.position = _startPosition;
        _botTransform.rotation = _startRotation;
    }

    private void Update()
    {
        if (_stopMove) return;
        var axisXChange = CalculateMoveDirection() * botSpeed * Time.deltaTime;
        _botTransform.Translate(axisXChange, 0f, 0f);
    }

    private float CalculateMoveDirection()
    {
        var distanceToArrow = Vector2.Distance(_botTransform.position, arrow.position);
        if (distanceToArrow > minimalDistanceToCalculate)
        {
            return 0;
        }
        var directionToArrow = (arrow.position - _botTransform.position).normalized;
        var angleBetweenDown = Mathf.Atan2(directionToArrow.y, directionToArrow.x) * Mathf.Rad2Deg + 90f;
        if (angleBetweenDown is > 90f or < -90f)
        {
            angleBetweenDown = 0;
        }
        return Mathf.Lerp(-1f, 1f, angleBetweenDown);;
    }
}
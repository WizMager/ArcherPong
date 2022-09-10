using System;
using Controllers.SinglePlayer;
using UnityEngine;
using Views;

public class SingleArrowController : MonoBehaviour
{
        private float _startArrowMoveSpeed;
        private float _maxArrowMoveSpeed;
        private int _arrowMoveSpeedMultiply;
        [SerializeField] private SingleScoreController scoreController;
        [SerializeField] private SingleArrowView arrowView;
        [SerializeField] private SinglePlayerMoveController playerMoveControllers;
        [SerializeField] private BotController botController;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentVelocity;
        private float _arrowMoveSpeed;

        public SingleArrowController(SingleArrowController arrowController)
        {
            
        }
        
        private void Start()
        {
            SetStartArrowMoveSpeed();
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            arrowView.OnReflect += ArrowReflected;
            arrowView.OnCatch += ArrowCaught;
            arrowView.OnMiss += PlayerMissedArrow;
            //scoreController.OnStartNextRound += SetStartArrowMoveSpeed;
            //playerControllers.OnShoot += Shooted;
            ArrowDisable();
        }

        private void SetStartArrowMoveSpeed()
        {
            _arrowMoveSpeed = _startArrowMoveSpeed;
        }
    
        
        private void ArrowCaught()
        {
            AddArrowSpeed();
        }

        private void AddArrowSpeed()
        {
            if (_arrowMoveSpeed > _maxArrowMoveSpeed)return;
            _arrowMoveSpeed += _arrowMoveSpeed / 100 * _arrowMoveSpeedMultiply;
        }

        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }
        
        private void ArrowReflected(Vector2 normal, bool isBot)
        {
            if (isBot)
            {
                AddArrowSpeed();
            }
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _rigidbody.velocity = direction * _arrowMoveSpeed;
            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) * _transform.rotation;
        }

        private void PlayerMissedArrow(bool isFirstPlayer)
        {
            SetStartArrowMoveSpeed();
            ArrowDisable();
        }
        

        private void Shooted()
        {
            PrepareShootArrow();
            ArrowEnable();
        }

        private void PrepareShootArrow()
        {
            //var shootPosition = playerControllers.GetShootPosition;
            //_transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
            //playerControllers.TakeArrow(false, (false, 0f));
        }
        
        private void ArrowEnable()
        {
            _spriteRenderer.enabled = true;
            _rigidbody.AddForce(_transform.up * _arrowMoveSpeed * _rigidbody.mass, ForceMode2D.Impulse);
        }

        private void FixedUpdate()
        {
            _currentVelocity = _rigidbody.velocity;
        }

        private void OnDestroy()
        {
            //playerControllers.OnShoot -= Shooted;
            Destroy(gameObject);
        }    
}
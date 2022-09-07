using System;
using Photon.Pun;
using UnityEngine;
using Views;

public class SingleArrowController : MonoBehaviour
{
        public Action<bool> OnPlayerMiss;
        [SerializeField] private float startArrowMoveSpeed;
        [SerializeField] private float maxArrowMoveSpeed;
        [SerializeField] private bool setStartPositionAfterMissArrow;
        [SerializeField] private int arrowMoveSpeedMultiply;
        [SerializeField] private SingleScoreController scoreController;
        [SerializeField] private float startTimeBeforeShoot;
        [SerializeField] private float minimalTimeBeforeShoot;
        [SerializeField] private int timeBeforeShootMultiply;
        [SerializeField] private SingleArrowView arrowView;
        [SerializeField] private SinglePlayerController playerControllers;
        [SerializeField] private BotController botController;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentVelocity;
        
        private float _arrowMoveSpeed;
        private float _timeBeforeShoot;

        private void Start()
        {
            SetStartArrowMoveSpeed();
            SetStartTimeBeforeShoot();
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            arrowView.OnReflect += ArrowReflected;
            arrowView.OnCatch += ArrowCaught;
            arrowView.OnMiss += PlayerMissedArrow;
            scoreController.OnStartNextRound += SetStartArrowMoveSpeed;
            playerControllers.OnShoot += Shooted;
            ArrowDisable();
        }

        private void SetStartArrowMoveSpeed()
        {
            _arrowMoveSpeed = startArrowMoveSpeed;
        }

        private void SetStartTimeBeforeShoot()
        {
            _timeBeforeShoot = startTimeBeforeShoot;
        }
        
        private void ArrowCaught()
        {
            RemoveTimeBeforeShoot();
            ArrowTake(true);
            AddArrowSpeed();
        }

        private void AddArrowSpeed()
        {
            if (_arrowMoveSpeed > maxArrowMoveSpeed)return;
            _arrowMoveSpeed += _arrowMoveSpeed / 100 * arrowMoveSpeedMultiply;
        }

        private void RemoveTimeBeforeShoot()
        {
            if (_timeBeforeShoot > minimalTimeBeforeShoot)
            {
                _timeBeforeShoot -= _timeBeforeShoot / 100 * timeBeforeShootMultiply;
            }
        }
        
        private void ArrowTake(bool withTimer)
        {
            playerControllers.TakeArrow(true, withTimer ? (true, _timeBeforeShoot) : (false, _timeBeforeShoot));
            ArrowDisable();
        }
        
        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            botController.StopMove = true;
        }
        
        private void ArrowReflected(Vector2 normal, bool isBot)
        {
            if (isBot)
            {
                AddArrowSpeed();
                RemoveTimeBeforeShoot();
            }
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _rigidbody.velocity = direction * _arrowMoveSpeed;
            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) * _transform.rotation;
        }

        private void PlayerMissedArrow(bool isFirstPlayer)
        {
            if (setStartPositionAfterMissArrow)
            {
                SetStartPosition(); 
            }
            OnPlayerMiss?.Invoke(isFirstPlayer);
            SetStartArrowMoveSpeed();
            SetStartTimeBeforeShoot();
            ArrowTake(false); 
        }

        private void SetStartPosition()
        {
            playerControllers.SetStartPosition();
            botController.SetStartPosition();
        }

        private void Shooted()
        {
            PrepareShootArrow();
            ArrowEnable();
        }

        private void PrepareShootArrow()
        {
            var shootPosition = playerControllers.GetShootPosition;
            _transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
            playerControllers.TakeArrow(false, (false, 0f));
        }
        
        private void ArrowEnable()
        {
            _spriteRenderer.enabled = true;
            _rigidbody.AddForce(_transform.up * _arrowMoveSpeed * _rigidbody.mass, ForceMode2D.Impulse);
            botController.StopMove = false;
        }

        private void FixedUpdate()
        {
            _currentVelocity = _rigidbody.velocity;
        }

        private void OnDestroy()
        {
            playerControllers.OnShoot -= Shooted;
            Destroy(gameObject);
        }    
}
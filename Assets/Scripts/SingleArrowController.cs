using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;
using Views;

public class SingleArrowController : MonoBehaviour
{
        public Action<bool> OnPlayerMiss;
        [SerializeField] private float startArrowMoveSpeed;
        [SerializeField] private float maxArrowMoveSpeed;
        [SerializeField] private bool setStartPositionAfterMissArrow;
        [SerializeField] private int arrowMoveSpeedMultiply;
        [SerializeField] private ScoreController scoreController;
        [SerializeField] private float startTimeBeforeShoot;
        [SerializeField] private float minimalTimeBeforeShoot;
        [SerializeField] private int timeBeforeShootMultiply;
        private ArrowView _arrowView;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentVelocity;
        private SinglePlayerController _playerControllers;
        private BotController _botController;
        private float _arrowMoveSpeed;
        private float _timeBeforeShoot;

        private void Awake()
        {
            _playerControllers = FindObjectOfType<SinglePlayerController>();
            _botController = FindObjectOfType<BotController>(); 
        }

        private void Start()
        {
            SetStartArrowMoveSpeed();
            SetStartTimeBeforeShoot();
            _arrowView = GetComponent<ArrowView>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _arrowView.OnReflect += ArrowReflected;
            _arrowView.OnCatch += ArrowCaught;
            _arrowView.OnMiss += PlayerMissedArrow;
            scoreController.OnStartNextRound += SetStartArrowMoveSpeed;
            _playerControllers.OnShoot += Shooted;
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
        
        private void ArrowCaught(bool isFirstPlayer)
        {
            if (_timeBeforeShoot > minimalTimeBeforeShoot)
            {
                _timeBeforeShoot -= _timeBeforeShoot / 100 * timeBeforeShootMultiply;
            }
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.ArrowCaught, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            ArrowTake(true);
            if (_arrowMoveSpeed > maxArrowMoveSpeed)return;
            _arrowMoveSpeed += _arrowMoveSpeed / 100 * arrowMoveSpeedMultiply;
        }

        private void ArrowTake(bool withTimer)
        {
            _playerControllers.TakeArrow(true, withTimer ? (true, _timeBeforeShoot) : (false, _timeBeforeShoot));
            ArrowDisable();
        }
        
        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }
        
        private void ArrowReflected(Vector2 normal)
        {
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
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.ArrowMissed, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            OnPlayerMiss?.Invoke(isFirstPlayer);
            SetStartArrowMoveSpeed();
            SetStartTimeBeforeShoot();
            ArrowTake(false);
        }

        private void SetStartPosition()
        {
            _playerControllers.SetStartPosition();
            _botController.SetStartPosition();
        }

        private void Shooted()
        {
            PrepareShootArrow();
            ArrowEnable();
        }

        private void PrepareShootArrow()
        {
            var shootPosition = _playerControllers.GetShootPosition;
            _transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
            _playerControllers.TakeArrow(false, (false, 0f));
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

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnDestroy()
        {
            _playerControllers.OnShoot -= Shooted;
            Destroy(gameObject);
        }    
}
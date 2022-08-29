using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;
using Views;

public class ArrowController : MonoBehaviour, IOnEventCallback
{
        [SerializeField] private float arrowMoveSpeed;
        private ArrowView _arrowView;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private PhotonView _photonView;
        private Vector2 _currentVelocity;
        private List<PlayerController> _playerControllers = new();

        private void Start()
        {
            _arrowView = GetComponent<ArrowView>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _photonView = GetComponent<PhotonView>();
            _arrowView.OnReflect += ArrowReflected;
            _arrowView.OnCatch += ArrowCaught;
            ArrowDisable();
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)PhotonEventCode.ArrowCaught:
                    if ((bool)photonEvent.CustomData)
                    {
                        _playerControllers[0].TakeArrow(true);
                    }
                    else
                    {
                        _playerControllers[1].TakeArrow(true);
                    }
                    ArrowDisable();
                   break; 
                case (int)PhotonEventCode.ArrowEnable:
                    _spriteRenderer.enabled = true;
                    break;
            }
        }
        
        private void ArrowCaught(bool isFirstPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.ArrowCaught, isFirstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            if (isFirstPlayer)
            {
               _playerControllers[0].TakeArrow(true);
            }
            else
            {
                _playerControllers[1].TakeArrow(true);
            }
            ArrowDisable();
        }

        private void ArrowReflected(Vector2 normal)
        {
            //if (!PhotonNetwork.IsMasterClient) return;
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _rigidbody.velocity = direction * arrowMoveSpeed;
            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) * _transform.rotation;
        }

        public void AddPlayerController(PlayerController playerController)
        {
            _playerControllers.Add(playerController);
            playerController.OnShoot += Shooted;
            
            if (_playerControllers.Count != 2) return;
            _playerControllers = _playerControllers.OrderBy(p => p.GetComponent<PhotonView>().Owner.ActorNumber).ToList();
        }

        private void Shooted(bool isFirstPlayer)
        {
            if (isFirstPlayer)
            {
                var shootPosition = _playerControllers[0].GetShootPosition;
                _transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
                _playerControllers[0].TakeArrow(false);
            }
            else
            {
                var shootPosition = _playerControllers[1].GetShootPosition;
                _transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
                _playerControllers[1].TakeArrow(false);
            }
            ArrowEnable();
        }

        private void ArrowEnable()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _spriteRenderer.enabled = true;
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.ArrowEnable, null, RaiseEventOptions.Default,
                SendOptions.SendReliable);
            _rigidbody.AddForce(_transform.up * arrowMoveSpeed * _rigidbody.mass, ForceMode2D.Impulse);
        }

        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            //if (!PhotonNetwork.IsMasterClient) return;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }

        private void FixedUpdate()
        {
            //if (!PhotonNetwork.IsMasterClient) return;
            _currentVelocity = _rigidbody.velocity;
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnDestroy()
        {
            foreach (var playerController in _playerControllers)
            {
                playerController.OnShoot -= Shooted;
            }
            Destroy(gameObject);
        }
}
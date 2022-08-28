using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Views;

public class ArrowController : MonoBehaviour
{
        [SerializeField] private float arrowMoveSpeed;
        private GameObject _arrow;
        private ArrowView _arrowView;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private Vector2 _currentVelocity;
        private List<PlayerController> _playerControllers = new();

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _arrow = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Arrow"), Vector3.zero, Quaternion.identity);
                _arrowView = _arrow.GetComponent<ArrowView>();
                _rigidbody = _arrow.GetComponent<Rigidbody2D>();
                _transform = _arrow.GetComponent<Transform>();
                _arrowView.OnReflect += ArrowReflected;
                _arrowView.OnCatch += ArrowCaught;
                _arrow.GetComponent<SpriteRenderer>().enabled = true;
                ArrowDisable();
            }
        }

        private void ArrowCaught(bool isFirstPlayer)
        {
            ArrowDisable();
            if (!PhotonNetwork.IsMasterClient) return;
            if (isFirstPlayer)
            {
               _playerControllers[0].TakeArrow(true);
            }
            else
            {
                _playerControllers[1].TakeArrow(true);
            }
        }

        private void ArrowReflected(Vector2 normal)
        {
            if (!PhotonNetwork.IsMasterClient) return;
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

        private void Shooted(Vector2 arrowPosition, Quaternion arrowRotation)
        {
            _transform.SetPositionAndRotation(arrowPosition,arrowRotation);
            foreach (var controller in _playerControllers)
            {
                controller.TakeArrow(false);
            }
            ArrowEnable();
        }

        private void ArrowEnable()
        {
            _arrow.SetActive(true);
            if (!PhotonNetwork.IsMasterClient) return;
            _rigidbody.AddForce(_transform.up * arrowMoveSpeed * _rigidbody.mass, ForceMode2D.Impulse); 
        }

        private void ArrowDisable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.angularVelocity = 0;
            }
            _arrow.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            _currentVelocity = _rigidbody.velocity;
        }

        private void OnDestroy()
        {
            foreach (var playerController in _playerControllers)
            {
                playerController.OnShoot -= Shooted;
            }
            Destroy(_arrow);
        }
}
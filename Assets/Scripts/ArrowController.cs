using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
        [SerializeField] private float arrowMoveSpeed;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentVelocity;
        private List<PlayerController> _playerControllers = new();

        private void Start()
        {
                _rigidbody = GetComponent<Rigidbody2D>();
                _transform = GetComponent<Transform>();
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakePlayerController(PlayerController playerController)
        {
            _playerControllers.Add(playerController);
            if (_playerControllers.Count != 2) return;
            foreach (var controller in _playerControllers)
            {
                controller.OnShoot += Shooted;
            }
        }
        
        private void Shooted(Vector2 arrowPosition, Quaternion arrowRotation)
        {
            _transform.SetPositionAndRotation(arrowPosition,arrowRotation);
            ArrowEnable();
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            var objectTag = col.gameObject.tag;

            switch (objectTag)
            {
                case "Wall":
                    var destroyPlayerWall = col.gameObject.GetComponent<DestroyPlayerWall>();
                    if (destroyPlayerWall)
                    {
                        //OnMissArrow?.Invoke(destroyPlayerWall.IsFirstPlayer);
                        if (PhotonNetwork.IsMasterClient)
                        {
                            var direction = Vector2.Reflect(_currentVelocity.normalized, col.contacts[0].normal);
                            _rigidbody.velocity = direction * arrowMoveSpeed;
                            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) *
                                                  _transform.rotation;
                        }
                    }
                    else
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            var direction = Vector2.Reflect(_currentVelocity.normalized, col.contacts[0].normal);
                            _rigidbody.velocity = direction * arrowMoveSpeed;
                            _transform.rotation = Quaternion.FromToRotation(_transform.up, direction) *
                                                  _transform.rotation;
                        }
                    }
                    break;
                case "Player":
                    col.gameObject.GetComponent<PlayerController>().TakeArrow(true);
                    ArrowDisable();
                    break;
            }
        }

        private void ArrowEnable()
        {
            _spriteRenderer.enabled = true;
            _rigidbody.AddForce(_transform.up * arrowMoveSpeed, ForceMode2D.Impulse); 
        }

        private void ArrowDisable()
        {
            _spriteRenderer.enabled = false;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _currentVelocity = _rigidbody.velocity;
            }
            
        }

        private void OnDestroy()
        {
            foreach (var playerController in _playerControllers)
            {
                playerController.OnShoot -= Shooted;
            }
        }
}
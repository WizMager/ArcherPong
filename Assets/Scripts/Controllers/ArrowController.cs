using System;
using System.Collections.Generic;
using Controllers.Interfaces;
using UnityEngine;
using Views;

namespace Controllers
{
    public class ArrowController : IStart, ILateExecute, ICleanup
    {
        public Action<bool> OnArrowCatch;
        public Action<bool> OnArrowMiss;
        private List<PlayerShootController> _playerShootController;
        private ArrowView _arrowView;
        private Rigidbody2D _arrowRigidbody;
        private Transform _arrowTransform;
        private SpriteRenderer _arrowSpriteRenderer;
        private Vector2 _currentVelocity;
        private float _arrowMoveSpeed;
        
        public ArrowController(List<PlayerShootController> playerShootControllers, GameObject arrow, float arrowMoveSpeed)
        {
            _playerShootController = playerShootControllers;
            _arrowView = arrow.GetComponent<ArrowView>();
            _arrowRigidbody = arrow.GetComponent<Rigidbody2D>();
            _arrowTransform = arrow.GetComponent<Transform>();
            _arrowSpriteRenderer = arrow.GetComponent<SpriteRenderer>();
            _arrowMoveSpeed = arrowMoveSpeed;
        }

        public void Start()
        {
            _arrowRigidbody = _arrowView.GetComponent<Rigidbody2D>();
            _arrowTransform = _arrowView.GetComponent<Transform>();
            _arrowSpriteRenderer = _arrowView.GetComponent<SpriteRenderer>();
            _arrowView.OnCatchArrow += ArrowCaught;
            _arrowView.OnMissArrow += ArrowMissed;
            _arrowView.OnReflect += ArrowReflected;
            foreach (var playerShootController in _playerShootController)
            {
                playerShootController.OnShoot += Shooted;
            }

            _arrowSpriteRenderer.enabled = false;
        }

        private void Shooted(Vector2 arrowPosition, Quaternion arrowRotation)
        {
            _arrowTransform.SetPositionAndRotation(arrowPosition,arrowRotation);
            ArrowEnable();
        }

        private void ArrowReflected(Vector2 normal)
        {
            var direction = Vector2.Reflect(_currentVelocity.normalized, normal);
            _arrowRigidbody.velocity = direction * _arrowMoveSpeed;
            _arrowTransform.rotation = Quaternion.FromToRotation(_arrowTransform.up, direction) *
                                 _arrowTransform.rotation;
        }

        private void ArrowMissed(bool isFirstPlayer)
        {
            ArrowDisable();
            OnArrowMiss?.Invoke(isFirstPlayer);
        }

        private void ArrowCaught(bool isFirstPlayer)
        {
            ArrowDisable();
            OnArrowCatch?.Invoke(isFirstPlayer);
        }

        public void LateExecute(float deltaTime)
        {
            _currentVelocity = _arrowRigidbody.velocity;
        }

        private void ArrowEnable()
        {
            _arrowSpriteRenderer.enabled = true;
            _arrowRigidbody.AddForce(_arrowTransform.up * _arrowMoveSpeed, ForceMode2D.Impulse); 
        }

        private void ArrowDisable()
        {
            _arrowSpriteRenderer.enabled = false;
            _arrowRigidbody.velocity = Vector2.zero;
            _arrowRigidbody.angularVelocity = 0;
        }

        public void Cleanup()
        {
            _arrowView.OnCatchArrow -= ArrowCaught;
            _arrowView.OnMissArrow -= ArrowMissed;
            _arrowView.OnReflect -= ArrowReflected;
            foreach (var playerShootController in _playerShootController)
            {
                playerShootController.OnShoot -= Shooted;
            }
        }
    }
}
using System;
using Controllers.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

namespace Controllers
{
    public class PlayerShootController : IAwake, IStart, IEnable, IDisable, ICleanup
    {
        public Action<Vector2, Quaternion> OnShoot;
        private ArrowController _arrowController;
        private GameObject _arrow;
        private Transform _shootPosition;
        private GameObject _bowArrow;
        private PlayerInput _playerInput;
        private bool _hasArrow = true;
        private bool _isFirstPlayer;
        
        public PlayerShootController(GameObject player)
        {
            _shootPosition = player.GetComponentInChildren<ArrowShootPosition>().transform;
            _bowArrow = player.GetComponentInChildren<BowArrow>().gameObject;
        }

        public void GetArrowController(ArrowController arrowController)
        {
            _arrowController = arrowController;
        }
        
        public void Awake()
        {
            _playerInput = new PlayerInput();
        }

        public void Start()
        {
            _arrowController.OnArrowCatch += ArrowTaken;
            _arrowController.OnArrowMiss += ArrowTaken;
        }
        
        private void ArrowTaken(bool isFirstPlayer)
        {
            if (_isFirstPlayer == isFirstPlayer) return;
            _hasArrow = true;
            _bowArrow.SetActive(true);
        }

        public void OnEnable()
        {
            _playerInput.Player.Shoot.Enable();
            _playerInput.Player.Shoot.performed += Shoot;
        }

        private void Shoot(InputAction.CallbackContext obj)
        {
            if (!_hasArrow) return;
            OnShoot?.Invoke(_shootPosition.position, _shootPosition.rotation);
            _hasArrow = false;
            _bowArrow.SetActive(false);
        }

        public void OnDisable()
        {
            _playerInput.Player.Shoot.Disable();
        }

        public void Cleanup()
        {
            _playerInput.Player.Shoot.performed -= Shoot;
        }
    }
}
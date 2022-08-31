using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils
{
       public class BowAim : MonoBehaviour
       {
              [SerializeField] private PhotonView photonView;
              private Camera _mainCamera;
              private PlayerInput _playerInput;

              private void Awake()
              {
                     _mainCamera = Camera.main;
                     _playerInput = new PlayerInput();
              }

              private void OnEnable()
              {
                     _playerInput.Player.Aiming.Enable();

                     _playerInput.Player.Aiming.performed += Aiming;
              }

              private void Aiming(InputAction.CallbackContext aiming)
              {
                     if (!photonView.IsMine) return;
                     var mousePosition = aiming.ReadValue<Vector2>();
                     var worldMousePosition = _mainCamera.ScreenToWorldPoint(mousePosition);
                     var mouseDirection = worldMousePosition - transform.position;
                     mouseDirection.Normalize();
                     var angleAxisZ = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90f;
                     transform.rotation = Quaternion.Euler(0f, 0f, angleAxisZ);
              }

              private void OnDisable()
              {
                     _playerInput.Player.Aiming.Disable();
              }
       }
}
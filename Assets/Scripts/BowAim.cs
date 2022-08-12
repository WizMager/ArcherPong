using UnityEngine;
using UnityEngine.InputSystem;

public class BowAim : MonoBehaviour
{
       [SerializeField] private Camera mainCamera;
       private PlayerInput _playerInput;

       private void Awake()
       {
              _playerInput = new PlayerInput();
       }

       private void OnEnable()
       {
              _playerInput.Player.Aiming.Enable();

              _playerInput.Player.Aiming.performed += Aiming;
       }

       private void Aiming(InputAction.CallbackContext aiming)
       {
              var mousePosition = aiming.ReadValue<Vector2>();
              var worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
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
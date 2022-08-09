using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowAim : MonoBehaviour
{
       [SerializeField] private Camera mainCamera;
       [SerializeField] private float rotationSpeed;
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
              var directionRay = new Ray2D(transform.position, transform.up - transform.position);
              var ang = Vector3.Angle(directionRay.direction, mouseDirection);
              Debug.Log(ang);
              var angle = Quaternion.Angle(transform.rotation, Quaternion.Euler(mouseDirection));
              Debug.DrawLine(transform.position, transform.up - transform.position);
              Debug.Log(angle);
              int rotationWay;
              if (ang > 90)
              {
                     rotationWay = 1;
              }
              else
              {
                     rotationWay = -1;
              }
              transform.Rotate(0, 0, rotationWay * rotationSpeed * Time.deltaTime);
       }

       private void OnDisable()
       {
              _playerInput.Player.Aiming.Disable();
       }
}
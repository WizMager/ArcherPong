using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
        public Action<bool> OnCatchArrow;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private Rigidbody2D arrowRigidbody;
        private Vector2 _currentVelocity;

        private void OnEnable()
        {
                arrowRigidbody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);  
        }

        private void OnDisable()
        {
                arrowRigidbody.velocity = Vector2.zero;
                arrowRigidbody.angularVelocity = 0;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
                tag = col.transform.tag;

                if (CompareTag("Wall"))
                {
                        var direction = Vector2.Reflect(_currentVelocity.normalized, col.contacts[0].normal);
                        arrowRigidbody.velocity = direction * moveSpeed;
                        transform.rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
                }

                if (CompareTag("Player"))
                {
                        var isFirstPlayer = col.transform.GetComponent<PlayerView>().IsFirstPlayer;
                        OnCatchArrow?.Invoke(isFirstPlayer);
                }
        }

        private void FixedUpdate()
        {
                _currentVelocity = arrowRigidbody.velocity;   
        }
}
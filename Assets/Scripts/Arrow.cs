using UnityEngine;

public class Arrow : MonoBehaviour
{
        [SerializeField] private float moveSpeed = 3f;
        private Rigidbody2D _rigidbody;
        private Vector2 _currentVelocity;

        private void Start()
        {
                _rigidbody = GetComponent<Rigidbody2D>();
                _rigidbody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
                tag = col.transform.tag;

                if (CompareTag("Wall"))
                {
                        var direction = Vector2.Reflect(_currentVelocity.normalized, col.contacts[0].normal);
                        _rigidbody.velocity = direction * moveSpeed;
                        transform.rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
                }

                if (CompareTag("Player"))
                {
                        var playerShoot = col.transform.GetComponent<PlayerShoot>();
                        playerShoot.CatchArrow();
                        Destroy(gameObject);
                }
        }

        private void FixedUpdate()
        {
                _currentVelocity = _rigidbody.velocity;   
        }
}
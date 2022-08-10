using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
        [SerializeField] private float moveSpeed = 3f;
        private Ray2D _ray;
        private Vector3 _normal;

        private void Start()
        {
                //Destroy(gameObject, 3f);
                CalculateReflection();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
                if (col.CompareTag("Wall"))
                {
                        var reflectionVector = Vector2.Reflect(_ray.direction.normalized, _normal.normalized);
                        var angle = Vector2.Angle(_ray.direction, _normal);
                        transform.RotateAround(transform.position, transform.forward, angle);
                        Debug.DrawLine(transform.position, reflectionVector, Color.red, 1000f);
                        CalculateReflection();
                } 
        }

        private void Update()
        {
                transform.Translate(transform.up * moveSpeed * Time.deltaTime);
        }

        private void CalculateReflection()
        {
                _ray = new Ray2D(transform.position, transform.up);
                var raycastHit = new RaycastHit2D[1];
                if (Physics2D.RaycastNonAlloc(_ray.origin, _ray.direction, raycastHit, 10f, LayerMask.GetMask("Reflect")) > 0)
                { 
                        _normal = raycastHit[0].normal;    
                }
        }
}
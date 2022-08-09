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
                        var reflectionVector = Vector2.Reflect(transform.forward, _normal);
                        reflectionVector.Normalize();
                        var angleAxisZ = Mathf.Atan2(reflectionVector.y, reflectionVector.x) * Mathf.Rad2Deg - 90f;
                        var angle = Vector2.Angle(-_ray.direction, _normal);
                        transform.RotateAround(transform.position, transform.forward, angle);
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
                if (Physics2D.RaycastNonAlloc(_ray.origin, _ray.direction, raycastHit) > 0)
                { 
                        _normal = raycastHit[0].normal;    
                }   
        }
}
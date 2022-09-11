using System;
using UnityEngine;
namespace Views
{
    public class ArrowView : MonoBehaviour
    {
        public Action<bool> OnMiss;
        public Action<Vector2> OnReflect;
        public Action<bool> OnCatch;
        [SerializeField] private Rigidbody2D arrowRigidbody;
        [SerializeField] private Transform arrowTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public Rigidbody2D GetRigidbody => arrowRigidbody;
        public Transform GetTransform => arrowTransform;
        public SpriteRenderer GetSpriteRenderer => spriteRenderer;

        private void OnCollisionEnter2D(Collision2D col)
        {
            var colliderTag = col.transform.tag;
            
            if (colliderTag == "Wall")
            {
                var destroyPlayerWall = col.gameObject.GetComponent<PlayerWallView>();
                if (destroyPlayerWall)
                {
                    OnMiss?.Invoke(destroyPlayerWall.IsFirstPlayer);
                }
                else
                {
                    OnReflect?.Invoke(col.contacts[0].normal);
                }
            }

            if (colliderTag == "Player")
            {
                OnCatch?.Invoke(col.gameObject.GetComponent<PlayerView>().IsFirstPlayer);
            }
        }
    }
}
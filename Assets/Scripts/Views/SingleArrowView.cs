using System;
using UnityEngine;

namespace Views
{
    public class SingleArrowView : MonoBehaviour
    {
        public Action<bool> OnMiss;
        public Action<Vector2, bool> OnReflect;
        public Action OnCatch;
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
                    OnReflect?.Invoke(col.contacts[0].normal, false);
                }
            }

            if (colliderTag == "Bot")
            {
                OnReflect?.Invoke(col.contacts[0].normal, true);
            }

            if (colliderTag == "Player")
            {
                OnCatch?.Invoke();
            }
        }
    }
}
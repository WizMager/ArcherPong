using System;
using UnityEngine;

namespace Views
{
    public class SingleArrowView : MonoBehaviour
    {
        public Action<bool> OnMiss;
        public Action<Vector2, bool> OnReflect;
        public Action OnCatch;

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
                    var isBot = col.gameObject.GetComponent<BotController>() != null;
                    OnReflect?.Invoke(col.contacts[0].normal, isBot);
                }
            }

            if (colliderTag == "Player")
            {
                OnCatch?.Invoke();
            }
        }
    }
}
using System;
using UnityEngine;
namespace Views
{
    public class ArrowView : MonoBehaviour
    {
        public Action<bool> OnMiss;
        public Action<Vector2> OnReflect;
        public Action<bool> OnCatch;

        private void OnCollisionEnter2D(Collision2D col)
        {
            tag = col.transform.tag;

            if (CompareTag("Wall"))
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

            if (CompareTag("Player"))
            {
                var isFirstPlayer = col.gameObject.GetComponent<PlayerView>().IsFirstPlayer;
                OnCatch?.Invoke(isFirstPlayer);
            }
        }
    }
}
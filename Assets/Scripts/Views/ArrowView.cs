using System;
using UnityEngine;

namespace Views
{
    public class ArrowView : MonoBehaviour
    {
        public Action<bool> OnMissArrow;
        public Action<Vector2> OnReflect;
        public Action<bool> OnCatchArrow;
        private void OnCollisionEnter2D(Collision2D col)
        {
            tag = col.transform.tag;

            if (CompareTag("Wall"))
            {
                var destroyPlayerWall = col.gameObject.GetComponent<DestroyPlayerWall>();
                if (destroyPlayerWall)
                {
                    OnMissArrow?.Invoke(destroyPlayerWall.IsFirstPlayer);
                }
                else
                {
                    OnReflect?.Invoke(col.contacts[0].normal);
                }
            }

            if (CompareTag("Player"))
            {
                var isFirstPlayer = col.transform.GetComponent<PlayerView>().IsFirstPlayer;
                OnCatchArrow?.Invoke(isFirstPlayer);
            }
        }
    }
}
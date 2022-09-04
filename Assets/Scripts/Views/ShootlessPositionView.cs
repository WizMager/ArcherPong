using System;
using UnityEngine;

namespace Views
{
    public class ShootlessPositionView : MonoBehaviour
    {
        public Action<bool> OnShootActivator;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                OnShootActivator?.Invoke(false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnShootActivator?.Invoke(true);
            }
        }
    }
}
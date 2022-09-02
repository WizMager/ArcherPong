using System;
using UnityEngine;

namespace Views
{
     public class PlayerView : MonoBehaviour
     {
          public Action<Vector3> OnWallEnter;
          [SerializeField] private bool isFirstPlayer;
          
          public bool IsFirstPlayer => isFirstPlayer;

          private void OnCollisionEnter2D(Collision2D col)
          {
               var colliderTag = col.gameObject.tag;
        
               if (colliderTag == "Wall")
               {
                    OnWallEnter?.Invoke(col.contacts[0].normal);
               }
          }
     }
}
using System;
using UnityEngine;

namespace Views
{
     public class PlayerView : MonoBehaviour
     {
          public Action<Vector3> OnWallEnter;
          [SerializeField] private bool isFirstPlayer;
          [SerializeField] private Transform bowBody;
          [SerializeField] private SpriteRenderer bowArrow;
          [SerializeField] private Transform shootPosition;
          
          public bool IsFirstPlayer => isFirstPlayer;
          public Transform GetBowBody => bowBody;
          public SpriteRenderer GetBowArrow => bowArrow;
          public Transform GetShootPosition => shootPosition;

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
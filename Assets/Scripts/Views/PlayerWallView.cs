using UnityEngine;

namespace Views
{
     public class PlayerWallView : MonoBehaviour
     {
          [SerializeField] private bool isFirstPlayer;

          public bool IsFirstPlayer => isFirstPlayer;
     }
}
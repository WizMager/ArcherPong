using UnityEngine;

public class PlayerView : MonoBehaviour
{
     [SerializeField] private bool isFirstPlayer;

     public bool IsFirstPlayer => isFirstPlayer;
}
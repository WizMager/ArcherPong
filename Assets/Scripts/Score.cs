using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
        [SerializeField] private Arrow arrow;
        [SerializeField] private TMP_Text firstPlayer;
        [SerializeField] private TMP_Text secondPlayer;
        private int _firstPlayerScore;
        private int _secondPlayerScore;

        private void Start()
        {
                arrow.OnMissArrow += MissedArrow;   
        }

        private void MissedArrow(bool isFirstPlayer)
        {
                if (isFirstPlayer)
                {
                        _secondPlayerScore++;
                        secondPlayer.text = _secondPlayerScore.ToString();
                }
                else
                {
                        _firstPlayerScore++;
                        firstPlayer.text = _firstPlayerScore.ToString();
                }
        }
}
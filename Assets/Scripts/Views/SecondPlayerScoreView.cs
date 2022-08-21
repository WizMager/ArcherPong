using TMPro;
using UnityEngine;

namespace Views
{
    public class SecondPlayerScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        public void SetScoreText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
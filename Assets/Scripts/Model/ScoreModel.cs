using Views;

namespace Model
{
    public class ScoreModel
    {
        private FirstPlayerScoreView _firstPlayer;
        private SecondPlayerScoreView _secondPlayer;
        private int _firstPlayerScore;
        private int _secondPlayerScore;

        public ScoreModel(FirstPlayerScoreView firstPlayerScoreView, SecondPlayerScoreView secondPlayerScoreView)
        {
            _firstPlayer = firstPlayerScoreView;
            _secondPlayer = secondPlayerScoreView;
        }

        public void ChangeScore(bool isFirstPlayer)
        {
            if (isFirstPlayer)
            {
                _secondPlayerScore++;
                _secondPlayer.SetScoreText(_secondPlayerScore);
            }
            else
            {
                _firstPlayerScore++;
                _firstPlayer.SetScoreText(_firstPlayerScore);
            }
        }
    }
}
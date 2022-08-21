using Controllers.Interfaces;
using Model;

namespace Controllers
{
    public class ScoreController : IStart, ICleanup
    {
        private ScoreModel _scoreModel;
        private ArrowController _arrowController;

        public ScoreController(ScoreModel scoreModel, ArrowController arrowController)
        {
            _scoreModel = scoreModel;
            _arrowController = arrowController;
        }
        
        public void Start()
        {
            _arrowController.OnArrowMiss += ArrowMissed;
        }

        private void ArrowMissed(bool isFirstPlayer)
        {
            _scoreModel.ChangeScore(isFirstPlayer);
        }

        public void Cleanup()
        {
            _arrowController.OnArrowMiss -= ArrowMissed;
        }
    }
}
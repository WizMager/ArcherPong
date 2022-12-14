using Controllers.Interfaces;
using Model;

namespace Controllers
{
    public class ScoreControllerMVC : IStart, ICleanup
    {
        private ScoreModel _scoreModel;
        private ArrowControllerOld _arrowControllerOld;

        public ScoreControllerMVC(ScoreModel scoreModel, ArrowControllerOld arrowControllerOld)
        {
            _scoreModel = scoreModel;
            _arrowControllerOld = arrowControllerOld;
        }
        
        public void Start()
        {
            _arrowControllerOld.OnArrowMiss += ArrowMissed;
        }

        private void ArrowMissed(bool isFirstPlayer)
        {
            _scoreModel.ChangeScore(isFirstPlayer);
        }

        public void Cleanup()
        {
            _arrowControllerOld.OnArrowMiss -= ArrowMissed;
        }
    }
}
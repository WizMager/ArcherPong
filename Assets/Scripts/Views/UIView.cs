using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class UIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text firstPlayerScore;
        [SerializeField] private TMP_Text secondPlayerScore;
        [SerializeField] private TMP_Text winLabel;
        [SerializeField] private Button leave;

        public Button GetLeaveButton => leave;

        public void WinTextActivation(bool isActivate)
        {
            winLabel.enabled = isActivate;
        }
        
        public void SetFirstPlayerScore(string scoreString)
        {
            
        }

        public void SetSecondPlayerScore(string scoreString)
        {
            
        }

        public void SetWinText(string winText)
        {
            
        }
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Views
{
    public class UIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text firstPlayerScore;
        [SerializeField] private TMP_Text secondPlayerScore;
        [SerializeField] private TMP_Text winLabel;
        [SerializeField] private Button leave;

        private void Start()
        {
            leave.onClick.AddListener(LeftRoom);
        }

        private void LeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void WinTextActivation(bool isActivate)
        {
            winLabel.enabled = isActivate;
        }
        
        public void SetFirstPlayerScore(string scoreString)
        {
            firstPlayerScore.text = scoreString;
        }

        public void SetSecondPlayerScore(string scoreString)
        {
            secondPlayerScore.text = scoreString;
        }

        public void SetWinText(string winText)
        {
            winLabel.text = winText;
        }

        private void OnDestroy()
        {
            leave.onClick.RemoveListener(LeftRoom);
        }
    }
}
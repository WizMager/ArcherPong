using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Utils;

public class ScoreController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private TMP_Text firstPlayerScore;
    [SerializeField] private TMP_Text secondPlayerScore;
    [SerializeField] private ArrowController arrowController;
    private int _firstPlayer;
    private int _secondPlayer;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        arrowController.OnPlayerMiss += ArrowMissed;
    }

    private void ArrowMissed(bool isFirstPlayer)
    {
        if (isFirstPlayer)
        {
            _secondPlayer++;
            secondPlayerScore.text = _secondPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.SecondPlayerScore, _secondPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
        else
        {
            _firstPlayer++;
            firstPlayerScore.text = _firstPlayer.ToString();
            PhotonNetwork.RaiseEvent((int)PhotonEventCode.FirstPlayerScore, _firstPlayer, RaiseEventOptions.Default,
                SendOptions.SendReliable);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (int)PhotonEventCode.FirstPlayerScore:
                firstPlayerScore.text = photonEvent.CustomData.ToString();
                break;
            case (int)PhotonEventCode.SecondPlayerScore:
                secondPlayerScore.text = photonEvent.CustomData.ToString();
                break;
        }
    }

    private void OnDestroy()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        arrowController.OnPlayerMiss -= ArrowMissed;
    }
}
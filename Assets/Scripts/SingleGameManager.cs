using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleGameManager : MonoBehaviour
{
    [SerializeField] private Button leave;
    [SerializeField] private Transform firstPlayerSpawn;
    [SerializeField] private Transform secondPlayerSpawn;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bot;

    private void Awake()
    {
        player.transform.SetPositionAndRotation(firstPlayerSpawn.position, firstPlayerSpawn.rotation);
        bot.transform.SetPositionAndRotation(secondPlayerSpawn.position, secondPlayerSpawn.rotation);
    }

    private void Start()
    {
        leave.onClick.AddListener(LeftRoom);
    }

    private void LeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        leave.onClick.RemoveListener(LeftRoom);
    }
}
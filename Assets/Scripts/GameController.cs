using UnityEngine;
using Views;

public class GameController : MonoBehaviour
{
    [SerializeField] private Data.Data data;
    private Controllers.Controllers _controllers;

    private void Awake()
    {
        var players = FindObjectsOfType<PlayerView>();
        //_controllers = new Controllers.Controllers();
        //new GameInitialization(_controllers, data, players);
        _controllers.Awake();
    }

    private void Start()
    {
        _controllers.Start();
    }

    private void OnEnable()
    {
        _controllers.OnEnable();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        _controllers.Execute(deltaTime);
    }

    private void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        _controllers.LateExecute(deltaTime);
    }

    private void OnDisable()
    {
        _controllers.OnDisable();
    }

    private void OnDestroy()
    {
        _controllers.Cleanup();
    }
}
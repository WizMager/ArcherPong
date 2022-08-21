using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Data.Data data;
    private Controllers.Controllers _controllers;

    private void Awake()
    {
        _controllers = new Controllers.Controllers();
        new GameInitialization(_controllers, data);
        _controllers.Awake();
    }

    private void Start()
    {
        _controllers.Start();
    }

    private void Update()
    {
        _controllers.Execute();
    }

    private void LateUpdate()
    {
        _controllers.LateExecute();
    }

    private void OnDestroy()
    {
        _controllers.Cleanup();
    }
}
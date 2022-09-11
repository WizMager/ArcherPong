using Photon.Pun;
using UnityEngine;

namespace Controllers.MultiPlayer
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Data.Data data;
        [SerializeField] private GameObject arrow;
        private Controllers _controllers;
        
        private void Awake()
        {
            _controllers = new Controllers();
            new GameInitialization(_controllers, data, arrow);
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
}
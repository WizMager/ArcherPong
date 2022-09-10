﻿using UnityEngine;

namespace Controllers.SinglePlayer
{
    public class SingleGameController : MonoBehaviour
    {
        [SerializeField] private Data.Data data;
        private Controllers _controllers;

        private void Awake()
        {
            _controllers = new Controllers();
            new SingleGameInitialization(_controllers, data);
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
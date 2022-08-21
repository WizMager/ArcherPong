using System.Collections.Generic;
using Controllers.Interfaces;
using UnityEngine;

namespace Controllers
{
    public class Controllers: IAwake, IStart, IEnable, IExecute, ILateExecute, IDisable, ICleanup
    {
        private readonly List<IAwake> _awakes;
        private readonly List<IStart> _starts;
        private readonly List<IEnable> _enables;
        private readonly List<IExecute> _executes;
        private readonly List<ILateExecute> _lateExecutes;
        private readonly List<IDisable> _disables;
        private readonly List<ICleanup> _cleanups;

        public Controllers()
        {
            _awakes = new List<IAwake>();
            _starts = new List<IStart>();
            _enables = new List<IEnable>();
            _executes = new List<IExecute>();
            _lateExecutes = new List<ILateExecute>();
            _disables = new List<IDisable>();
            _cleanups = new List<ICleanup>();
        }

        internal Controllers Add(IController controller)
        {
            if (controller is IAwake awakeController)
            {
                _awakes.Add(awakeController);
            }

            if (controller is IStart startController)
            {
                _starts.Add(startController);
            }

            if (controller is IEnable enableController)
            {
                _enables.Add(enableController);
            }
            
            if (controller is IExecute executeController)
            {
                _executes.Add(executeController);
            }

            if (controller is ILateExecute lateExecuteController)
            {
               _lateExecutes.Add(lateExecuteController); 
            }

            if (controller is IDisable disableController)
            {
                _disables.Add(disableController);
            }
            
            if (controller is ICleanup cleanupController)
            {
                _cleanups.Add(cleanupController);
            }

            return this;
        }
        
        internal void Remove(IController controller)
        {
            if (controller is IAwake awakeController)
            {
                _awakes.Remove(awakeController);
            }

            if (controller is IStart startController)
            {
                _starts.Remove(startController);
            }

            if (controller is IEnable enableController)
            {
                _enables.Remove(enableController);
            }
            
            if (controller is IExecute executeController)
            {
                _executes.Remove(executeController);
            }

            if (controller is ILateExecute lateExecuteController)
            {
                _lateExecutes.Remove(lateExecuteController); 
            }

            if (controller is IDisable disableController)
            {
                _disables.Remove(disableController);
            }
            
            if (controller is ICleanup cleanupController)
            {
                _cleanups.Remove(cleanupController);
            }
        }
        
        public void Awake()
        {
            _awakes.ForEach(value => value.Awake());
        }

        public void Start()
        {
            _starts.ForEach(value => value.Start());
        }

        public void OnEnable()
        {
            _enables.ForEach(value => value.OnEnable());
        }
        
        public void Execute(float deltaTime)
        {
            _executes.ForEach(value => value.Execute(deltaTime));
        }

        public void LateExecute(float deltaTime)
        {
            _lateExecutes.ForEach(value => value.LateExecute(deltaTime));
        }

        public void OnDisable()
        {
            _disables.ForEach(value => value.OnDisable());
        }
        
        public void Cleanup()
        {
            _cleanups.ForEach(value => value.Cleanup());
        }
    }
}
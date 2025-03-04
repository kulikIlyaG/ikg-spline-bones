using System;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainer;
using UnityEngine;

namespace IKGTools.Editor.EasyStateMachine
{
    public interface IStateMachine
    {
        void GoToState(Type state);
    }

    public class EasyStateMachine<T> : IStateMachine where T : EasyState
    {
        private sealed class StateMachineInstaller : BaseInstaller
        {
            private readonly StatesInstaller _statesInstaller;
            private readonly IStateMachine _stateMachine;

            public StateMachineInstaller(IStateMachine stateMachine, StatesInstaller statesInstaller)
            {
                _stateMachine = stateMachine;
                _statesInstaller = statesInstaller;
            }

            public override void Install(DIContainer container)
            {
                container.Register(_stateMachine);
                _statesInstaller.Install(container);
            }
        }

        private readonly Dictionary<Type, T> _states;
        private readonly DIContainer _diContainer;

        public EasyStateMachine(DIContainer rootContainer, StatesInstaller installer)
        {
            _diContainer = rootContainer.CreateSubContext(new[] {new StateMachineInstaller(this, installer)});

            var instances = installer.GetStateInstances();
            _states = new Dictionary<Type, T>(instances.Count);
            foreach (var stateInstancePair in instances)
                _states.Add(stateInstancePair.Key, stateInstancePair.Value as T);
        }

        public T CurrentState { get; private set; }

        public void Start(Type startState)
        {
            var startStateInstance = GetStateInstance(startState);
            CurrentState = startStateInstance;
            startStateInstance.Enter(null);
        }


        void IStateMachine.GoToState(Type type)
        {
            if (CurrentState == null)
                throw new Exception($"Transition not possible. {nameof(CurrentState)} is null!");

            var newStateInstance = GetStateInstance(type);
            var previousStateInstance = CurrentState;
            CurrentState.Exit(newStateInstance);
            CurrentState = newStateInstance;
            CurrentState.Enter(previousStateInstance);
            
#if EASY_STATE_MACHINE_DEBUG
            Debug.Log($"Transition state {previousStateInstance.GetType().Name} -> {type.Name}");
#endif
        }


        private T GetStateInstance(Type type)
        {
            if (_states.TryGetValue(type, out var state))
                return state;

            throw new Exception($"Not found state by type: {type.Name}");
        }

        public void Close()
        {
            CurrentState.Exit(null);
            CurrentState = null;
        }
    }
}
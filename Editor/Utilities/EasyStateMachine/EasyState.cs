using System;
using IKGTools.Editor.EasyContainerEditor;

namespace IKGTools.SplineBones.Editor.EasyStateMachine
{
    public abstract class EasyState : ITickable
    {
        private readonly IStateMachine _stateMachine;
        protected bool _isActive;
        
        protected EasyState(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public bool IsInitialized { get; private set; }
        
        protected abstract void OnEnter(EasyState enterFrom);
        protected abstract void OnExit(EasyState exitTo);
        protected abstract void OnCloseMachine();
        protected virtual void InitializeProcess(){}
        public void Tick()
        {
            if (_isActive)
                OnTick();
        }

        protected virtual void OnTick(){}

        protected void GoToState(Type stateType)
        {
            _stateMachine.GoToState(stateType);
        }
        
        public void Enter(EasyState enterFrom)
        {
            if (!IsInitialized)
                Initialize();

            OnEnter(enterFrom);
            _isActive = true;
        }

        private void Initialize()
        {
            InitializeProcess();
            IsInitialized = true;
        }

        public void Exit(EasyState exitTo)
        {
            OnExit(exitTo);
            if (exitTo == null)
                OnCloseMachine();

            _isActive = false;
        }

    }
}
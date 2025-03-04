using System;
using System.Collections.Generic;
using Editor.Services.State.States;
using IKGTools.Editor.EasyContainerEditor;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor.EasyStateMachine;

namespace IKGTools.Editor.Services
{
    internal sealed class EditorStateMachineService
    {
        private sealed class EditorStatesInstaller : StatesInstaller
        {
            private IReadOnlyCollection<Type> _stateTypes = new  List<Type>
            {
                typeof(DisabledEditorState),
                typeof(EmptyEditorState),
                typeof(SplineEditorState),
                typeof(RigEditorState)
            };

            private Dictionary<Type, EasyState> _stateInstances;
        
            public override void Install(DIContainer container)
            {
                _stateInstances = new Dictionary<Type, EasyState>(_stateTypes.Count);
            
                foreach (var statePair in _stateTypes)
                    _stateInstances.Add(statePair, container.CreateAndRegister(statePair) as EasyState);
            }

            public override Dictionary<Type, EasyState> GetStateInstances()
            {
                return _stateInstances;
            }
        }
        
        private readonly EditorDataService _data;

        private readonly EasyStateMachine<EditorEasyState> _stateMachine;
        private readonly EditorLifeCycleEventsService _lifeCycleEvents;

        public EditorStateMachineService(EditorDataService data, EditorLifeCycleEventsService lifeCycleEvents, DIContainer rootContainer)
        {
            _data = data;
            _lifeCycleEvents = lifeCycleEvents;

            _stateMachine = new EasyStateMachine<EditorEasyState>(rootContainer, new EditorStatesInstaller());
            _lifeCycleEvents.OnDisabledEditor += OnDisabledEditor;
        }

        private void OnDisabledEditor()
        {
            (_stateMachine as IStateMachine).GoToState(typeof(DisabledEditorState));
        }

        public void Initialize()
        {
            Type startStateType = _lifeCycleEvents.IsEditorEnabled
                ? typeof(EmptyEditorState)
                : typeof(DisabledEditorState);
            _stateMachine.Start(startStateType);
        }
    }
}
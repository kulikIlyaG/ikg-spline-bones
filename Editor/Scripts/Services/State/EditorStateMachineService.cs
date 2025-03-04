using System;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor.Services.States;

namespace IKGTools.SplineBones.Editor.Services
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
        private readonly EditorActivitiCycleEventsService _activitiCycleEvents;

        public EditorStateMachineService(EditorDataService data, EditorActivitiCycleEventsService activitiCycleEvents, DIContainer rootContainer)
        {
            _data = data;
            _activitiCycleEvents = activitiCycleEvents;

            _stateMachine = new EasyStateMachine<EditorEasyState>(rootContainer, new EditorStatesInstaller());
            _activitiCycleEvents.OnDisabledEditor += OnDisabledEditor;
        }

        private void OnDisabledEditor()
        {
            (_stateMachine as IStateMachine).GoToState(typeof(DisabledEditorState));
        }

        public void Initialize()
        {
            Type startStateType = _activitiCycleEvents.IsEditorEnabled
                ? typeof(EmptyEditorState)
                : typeof(DisabledEditorState);
            _stateMachine.Start(startStateType);
        }
    }
}
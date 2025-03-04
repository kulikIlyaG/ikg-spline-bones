using System;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services.States;

namespace IKGTools.SplineBones.Editor.Services.States
{
    internal sealed class RigEditorSubStateMachine
    {
        private sealed class RigEditorSubStatesInstaller : StatesInstaller
        {
            private IReadOnlyCollection<Type> _stateTypes = new[]
            {
                typeof(EditorRigSubStateEmpty),
                typeof(EditorRigSubStateAddBone),
                typeof(EditorRigSubStateRemoveBone)
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

        private readonly EasyStateMachine<EditorEasyState> _stateMachine;
        
        public RigEditorSubStateMachine(DIContainer rootContainer)
        {
            _stateMachine = new EasyStateMachine<EditorEasyState>(rootContainer, new RigEditorSubStatesInstaller());
        }

        public void Start()
        {
            _stateMachine.Start(typeof(EditorRigSubStateEmpty));
        }
        
        public void Close()
        {
            _stateMachine.Close();
        }
    }
}
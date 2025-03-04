using System;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainerEditor;

namespace IKGTools.SplineBones.Editor.EasyStateMachine
{
    public abstract class StatesInstaller : BaseInstaller
    {
        public abstract Dictionary<Type, EasyState> GetStateInstances();
    }
}
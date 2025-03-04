using System;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainer;

namespace IKGTools.Editor.EasyStateMachine
{
    public abstract class StatesInstaller : BaseInstaller
    {
        public abstract Dictionary<Type, EasyState> GetStateInstances();
    }
}
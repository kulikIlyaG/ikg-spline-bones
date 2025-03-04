using System;
using IKGTools.SplineBones;

namespace IKGTools.Editor.Services
{
    internal sealed class EditorDataService
    {
        private SplineBonesComponent _component;
        private bool _beforeEnterEditModeExecuteComponentValue;
        
        public event Action<SplineBonesComponent> OnSetData;
        public event Action OnReleaseComponent;
        
        public bool IsHasComponent => _component != null;
        public SplineBonesComponent Component => _component;

        public void SetData(SplineBonesComponent component)
        {
            if(_component != null && _component == component)
                return;
            
            _component = component;
            OnSetData?.Invoke(_component);
        }

        public void ReleaseData()
        {
            if(_component == null)
                return;
            
            _component = null;
            OnReleaseComponent?.Invoke();
        }

        public void TryStartExecute()
        {
            if (_beforeEnterEditModeExecuteComponentValue)
            {
                _component.RestartExecute();
                _component.Execute = true;
            }
        }

        public void TryStopExecuteComponent()
        {
            _beforeEnterEditModeExecuteComponentValue = _component.Execute;
            
            if(_beforeEnterEditModeExecuteComponentValue)
                _component.Execute = false;
        }
    }
}
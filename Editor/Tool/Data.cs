using System;
using UnityEngine.U2D;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class Data
    {
        public enum EditMode
        {
            None,
            Spline,
            Rig
        }

        private readonly SplineBonesComponent _splineBonesComponent;
        private SplineBindingsData _bindingsData;

        private EditMode _currentMode = EditMode.None;

        public event Action OnEnabledSplineEditMode;
        public event Action OnEnabledRigEditMode;
        public event Action OnResetEditMode;
        
        public Data(SplineBonesComponent splineBonesComponent)
        {
            _splineBonesComponent = splineBonesComponent;
        }

        public EditMode CurrentMode => _currentMode;
        public SkeletonComponent Skeleton => _splineBonesComponent.Skeleton;
        public SplineBindingsData BindingsData => _splineBonesComponent.BindingsData;
        
        
        private Spline Spline => _splineBonesComponent.Spline;

        
        private bool _beforeEnterEditModeExecuteComponentValue;

        public void SetEditSplineMode()
        {
            if(_currentMode == EditMode.Spline)
                return;
            
            _currentMode = EditMode.Spline;
            
            TryStopExecuteComponent();
            
            OnEnabledSplineEditMode?.Invoke();
        }

        public void SetEditRigMode()
        {
            if(_currentMode == EditMode.Rig)
                return;

            _currentMode = EditMode.Rig;
            
            TryStopExecuteComponent();
            
            OnEnabledRigEditMode?.Invoke();
        }

        public void ResetEditMode()
        {
            if(_currentMode == EditMode.None)
                return;
            
            _currentMode = EditMode.None;

            if (_beforeEnterEditModeExecuteComponentValue)
            {
                _splineBonesComponent.RestartExecute();
                _splineBonesComponent.Execute = true;
            }
            
            OnResetEditMode?.Invoke();
        }

        private void TryStopExecuteComponent()
        {
            _beforeEnterEditModeExecuteComponentValue = _splineBonesComponent.Execute;
            
            if(_beforeEnterEditModeExecuteComponentValue)
                _splineBonesComponent.Execute = false;
        }
    }
}
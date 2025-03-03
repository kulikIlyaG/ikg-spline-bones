using System;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    [InitializeOnLoad]
    internal static class MainEditor
    {
        private static Data _data;

        private static RigEditor _rigEditor;
        private static SplineEditor _splineEditor;
        private static BoneAndSplineInfoDrawer _boneAndSplineInfoDrawer;
        
        internal static Data Data => _data;
        internal static Data.EditMode CurrentMode => _data.CurrentMode;

        public static IRigEditor RigEditor => _rigEditor;

        
        internal static Action<Data> OnStartEdit;
        internal static Action OnStopEdit;
        internal static Action OnSelectedEditMode;
        internal static Action OnDeselectedEditMode;

        static MainEditor()
        {
            Selection.selectionChanged += OnChangedSelections;
        }

        private static void OnChangedSelections()
        {
            if (IsEdit)
            {
                if (Selection.activeGameObject == null || !IsRiggedSpline(Selection.activeGameObject, out var component))
                    StopEdit();
                else if (component.Skeleton != _data.Skeleton)
                {
                    StopEdit();
                    StartRigEdit(component);
                }
            }
            else
            {
                TryStartEdit();
            }
        }

        public static void TryStartEdit()
        {
            if (Selection.activeGameObject != null && IsRiggedSpline(Selection.activeGameObject, out var component))
                StartRigEdit(component);
        }

        private static bool IsRiggedSpline(GameObject obj, out SplineBonesComponent component)
        {
            var result = ComponentChecker.HasComponentInHierarchy(obj, typeof(SplineBonesComponent), out var founded);
            
            if (result)
                component = founded as SplineBonesComponent;
            else
                component = null;
            
            return result;
        }

        public static void StartRigEdit(SplineBonesComponent component)
        {
            ResetEditorData();
            _data = new Data(component);
            _data.OnResetEditMode += OnResetEditMode;
            _data.OnEnabledRigEditMode += OnEnabledRigEditModeInvoke;
            _data.OnEnabledSplineEditMode += OnEnabledSplineEditMode;

            if(_data.BindingsData != null)
                _boneAndSplineInfoDrawer = new BoneAndSplineInfoDrawer(component.Skeleton, component.BindingsData, component.Spline);
            OnStartEdit?.Invoke(_data);
        }
        
        public static bool IsEdit => _data != null && _data.BindingsData != null && !Application.isPlaying;

        public static void StopEdit()
        {
            TryToDisposeRigEditor();
            
            ResetEditorData();
            _boneAndSplineInfoDrawer.Dispose();
            OnStopEdit?.Invoke();
        }
        
        private static void ResetEditorData()
        {
            if (_data != null)
            {
                _data.ResetEditMode();
                
                _data.OnResetEditMode -= OnResetEditMode;
                _data.OnEnabledRigEditMode -= OnEnabledRigEditModeInvoke;
                _data.OnEnabledSplineEditMode -= OnEnabledSplineEditMode;
                _data = null;
            }
        }

        private static void OnEnabledRigEditModeInvoke()
        {
            TryToDisposeSplineEditor();
            
            _rigEditor = new RigEditor(_data);
            OnSelectedEditMode?.Invoke();
        }
        
        private static void OnEnabledSplineEditMode()
        {
            TryToDisposeRigEditor();
            
            _splineEditor = new SplineEditor();
            OnSelectedEditMode?.Invoke();
        }

        private static void OnResetEditMode()
        {
            TryToDisposeRigEditor();
            TryToDisposeSplineEditor();
            OnDeselectedEditMode?.Invoke();
        }

        private static void TryToDisposeSplineEditor()
        {
            if (_splineEditor != null)
            {
                _splineEditor.Dispose();
                _splineEditor = null;
            }
        }

        private static void TryToDisposeRigEditor()
        {
            if (_rigEditor != null)
            {
                _rigEditor.Dispose();
                _rigEditor = null;
            }
        }
    }
}
using System;
using IKGTools.Editor.EasyContainerEditor;
using IKGTools.SplineBones;
using IKGTools.SplineBones.Editor;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Services
{
    internal sealed class EditorLifeCycleEventsService
    {
        private readonly EditorDataService _dataService;
        
        internal event Action OnStartEdit;
        internal event Action OnStopEdit;
        
        internal event Action OnSelectedEditMode;
        internal event Action OnDeselectedEditMode;

        [EasyInject]
        public EditorLifeCycleEventsService(EditorDataService dataService)
        {
            _dataService = dataService;
            
            Selection.selectionChanged += OnChangedSelections;
        }
        
        private void OnChangedSelections()
        {
            if (_dataService.IsHasComponent)
            {
                if (Selection.activeGameObject == null ||
                    !IsRiggedSpline(Selection.activeGameObject, out var component))
                {
                    BreakEdit();
                }
                else if(component.Skeleton != _dataService.Component.Skeleton)
                {
                    BreakEdit();
                    StartEdit(component);
                }
            }
            else
            {
                TryStartEdit();
            }
        }

        private void StartEdit(SplineBonesComponent component)
        {
            _dataService.SetData(component);
            OnStartEdit?.Invoke();
        }

        private void BreakEdit()
        {
            _dataService.ReleaseData();
            OnStopEdit?.Invoke();
        }

        private void TryStartEdit()
        {
            if (Selection.activeGameObject != null && IsRiggedSpline(Selection.activeGameObject, out var component))
                StartEdit(component);
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
    }
}
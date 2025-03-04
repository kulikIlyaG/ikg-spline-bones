using System;
using IKGTools.Editor.EasyContainerEditor;
using IKGTools.SplineBones;
using IKGTools.SplineBones.Editor;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Services
{
    internal sealed class EditorLifeCycleEventsService : IInitializable
    {
        private readonly EditorDataService _dataService;
        
        internal event Action OnEnabledEditor;
        internal event Action OnDisabledEditor;
        
        internal event Action OnSelectedEditMode;
        internal event Action OnDeselectedEditMode;

        public EditorLifeCycleEventsService(EditorDataService dataService)
        {
            _dataService = dataService;
            
            Selection.selectionChanged += OnChangedSelections;
        }
        
        public bool IsEditorEnabled { get; private set; }
        
        private void OnChangedSelections()
        {
            CheckSelectedObjects();
        }

        public void Initialize()
        {
            CheckSelectedObjects();
        }

        private void CheckSelectedObjects()
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
            IsEditorEnabled = true;
            OnEnabledEditor?.Invoke();
        }

        private void BreakEdit()
        {
            _dataService.ReleaseData();
            IsEditorEnabled = false;
            OnDisabledEditor?.Invoke();
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
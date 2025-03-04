using IKGTools.Editor;
using IKGTools.SplineBones.Editor.Utilities.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    [CustomEditor(typeof(SplineBonesComponent))]
    internal class SplineBonesComponentEditor : UnityEditor.Editor
    {
        private const string ENABLED_TOGGLE_EXECUTE_LABEL = "Stop execute";
        private const string DISABLED_TOGGLE_EXECUTE_LABEL = "Start execute";
        
        
        private SplineBonesComponent _splineBones;
        private SkeletonComponent _skeleton;
        private Transform _selectedBone;

        private PointBindingsEditor _pointBindingsEditor;

        private ToggleButton _executeToggle;
        
        private void OnEnable()
        {
            _splineBones = (SplineBonesComponent) target;
            _skeleton = _splineBones.GetComponentInChildren<SkeletonComponent>();
            
            if(_splineBones.BindingsData != null)
                _pointBindingsEditor = new PointBindingsEditor(_splineBones.BindingsData);

            if (_pointBindingsEditor != null)
            {
                TryValidateBonesInstance();
            }
            
            EditorApplication.update += UpdateGUIItems;
            SplineBonesEditorRoot.OnSelectedEditMode += OnEnterToEditMode;
            SplineBonesEditorRoot.OnDeselectedEditMode += OnExitFromEditMode;
        }
        

        private void OnExitFromEditMode()
        {
            _executeToggle.SetValue(_splineBones.Execute, true);
            _executeToggle.SetInteractable(true);
        }

        private void OnEnterToEditMode()
        {
            _executeToggle.SetValue(_splineBones.Execute, true);
            _executeToggle.SetInteractable(false);
        }

        private void TryValidateBonesInstance()
        {
            SplineBonesEditorRoot.TryUpdateBoneInstances(_skeleton,
                _splineBones.BindingsData.Bindings.SkeletonDefinition.GetBonesTransform());
            //RigEditor.TryUpdateBoneInstances(_skeleton, _splineBones.BindingsData.Bindings.SkeletonDefinition.GetBonesTransform());
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateGUIItems;
            SplineBonesEditorRoot.OnSelectedEditMode -= OnEnterToEditMode;
            SplineBonesEditorRoot.OnDeselectedEditMode -= OnExitFromEditMode;
        }


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            CreateExecutionToggle(root);

            if(_splineBones.BindingsData == null)
            {
                ObjectField objectField = new ObjectField("Bindings data")
                {
                    objectType = typeof(SplineBindingsData),
                    value = null
                };

                objectField.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue is SplineBindingsData data)
                    {
                        _splineBones.SetBindingsData(data);
                        EditorUtility.SetDirty(_splineBones); 
                        root.Clear();
                        CreateExecutionToggle(root);
                        CreateBindingsDataUI(root, data);
                        if (_pointBindingsEditor != null)
                            TryValidateBonesInstance();
                        MainEditor.TryStartEdit();
                    }
                });

                root.Add(objectField);
            }
            else
            {
                CreateBindingsDataUI(root, _splineBones.BindingsData);
            }

            return root;
        }

        private void CreateExecutionToggle(VisualElement root)
        {
            _executeToggle = new ToggleButton(_splineBones.Execute ? ENABLED_TOGGLE_EXECUTE_LABEL : DISABLED_TOGGLE_EXECUTE_LABEL, _splineBones.Execute);
            _executeToggle.OnChangedValue += OnChangedExecuteToggleValue;
            _executeToggle.SetInteractable(_splineBones.BindingsData != null);
            root.Add(_executeToggle);
        }

        private void OnChangedExecuteToggleValue(bool value)
        {
            if (value)
                _splineBones.RestartExecute();
            
            _splineBones.Execute = value;
            
            _executeToggle.SetLabel(value ? ENABLED_TOGGLE_EXECUTE_LABEL : DISABLED_TOGGLE_EXECUTE_LABEL);
        }

        private void CreateBindingsDataUI(VisualElement root, SplineBindingsData data)
        {
            if (_pointBindingsEditor == null)
                _pointBindingsEditor = new PointBindingsEditor(data);
            
            root.Add(_pointBindingsEditor.CreateVisualElements());
        }

        private void UpdateGUIItems()
        {
            _pointBindingsEditor?.OnGUI();
        }
    }
}


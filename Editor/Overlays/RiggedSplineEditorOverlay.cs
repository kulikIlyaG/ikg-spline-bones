using IKGTools.SplineBones.Editor.Utilities.UIToolkit;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    [Overlay(typeof(SceneView), "Spline Bones Binding")]
    internal class RiggedSplineEditorOverlay : BaseTransientOverlay
    {
        private Button _buttonSplineEdit;
        private Button _buttonRigEdit;
        
        private Button _buttonAddBone;
        private Button _buttonRemoveBone;

        private RigEditModeOverlayExecutor _rigEditModeOverlayExecutor;

        protected override bool IsVisible() => MainEditor.IsEdit;

        protected override VisualElement CreateUIElements()
        {
            string path = PathHelper.GetPathFor("Layouts/edit-bones-overlay-main.uxml");
            var container = EditorAssetsHelper.Get<VisualTreeAsset>(path);

            if (container == null)
            {
                Debug.Log($"Not found UXML for Spline Bones Overlay({path})");
                return new VisualElement();
            }
            
            var result = container.Instantiate().contentContainer;

            _buttonSplineEdit = result.Q<Button>("spline_edit");
            _buttonRigEdit = result.Q<Button>("skeleton_edit");
            _buttonAddBone = result.Q<Button>("add_bone");
            _buttonRemoveBone = result.Q<Button>("remove_bone");

            _buttonSplineEdit.clicked += OnClickSplineEdit;
            _buttonRigEdit.clicked += OnClickRigEdit;
            
            return result;
        }

        protected override void OnBeforeShow()
        {
            ResetView();
        }

        protected override void InitializeProcess()
        {
            base.InitializeProcess();
            
            MainEditor.OnStartEdit += OnStartEdit;
            MainEditor.OnStopEdit += OnStopEdit;
        }


        private void ResetView()
        {
            if (_rigEditModeOverlayExecutor != null)
            {
                _rigEditModeOverlayExecutor.Dispose();
                _rigEditModeOverlayExecutor = null;
            }
            
            if(_buttonAddBone == null)
                return;
            
            _buttonAddBone!.TryAddClass(USSClassKeys.BUTTON_DISABLED);
            _buttonRemoveBone!.TryAddClass(USSClassKeys.BUTTON_DISABLED);
        }

        private void OnStartEdit(Data data)
        {
            SubscribeToEvents();
            ResetView();
        }
        
        private void OnStopEdit()
        {
            if (_rigEditModeOverlayExecutor != null)
            {
                _rigEditModeOverlayExecutor!.Dispose();
                _rigEditModeOverlayExecutor = null;
            }

            UnsubscribeFromEvents();
            ResetView();
        }

        private void OnClickRigEdit()
        {
            if(MainEditor.CurrentMode != Data.EditMode.Rig)
                MainEditor.Data.SetEditRigMode();
            else 
                MainEditor.Data.ResetEditMode();
        }

        private void OnClickSplineEdit()
        {
            if(MainEditor.CurrentMode != Data.EditMode.Spline)
                MainEditor.Data.SetEditSplineMode();
            else 
                MainEditor.Data.ResetEditMode();
        }

        private void OnResetEditMode()
        {
            _buttonSplineEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            
            if (_rigEditModeOverlayExecutor != null)
            {
                _rigEditModeOverlayExecutor.Dispose();
                _rigEditModeOverlayExecutor = null;
                _buttonAddBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
                _buttonRemoveBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
            }
        }

        private void OnEnabledSplineEditMode()
        {
            _buttonSplineEdit.TryAddClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);

            if (_rigEditModeOverlayExecutor != null)
            {
                _rigEditModeOverlayExecutor.Dispose();
                _rigEditModeOverlayExecutor = null;
                _buttonAddBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
                _buttonRemoveBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
            }
        }

        private void OnEnabledRigEditMode()
        {
            _buttonSplineEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryAddClass(USSClassKeys.BUTTON_ENABLED);

            if (_rigEditModeOverlayExecutor != null)
            {
                _rigEditModeOverlayExecutor.Dispose();
                _rigEditModeOverlayExecutor = null;
            }
            _rigEditModeOverlayExecutor = new RigEditModeOverlayExecutor(MainEditor.RigEditor, _buttonAddBone, _buttonRemoveBone);
            
            _buttonAddBone.TryRemoveClass(USSClassKeys.BUTTON_DISABLED);
            _buttonRemoveBone.TryRemoveClass(USSClassKeys.BUTTON_DISABLED);
        }
        

        protected override void SubscribeToEvents()
        {
            var data = MainEditor.Data;
            
            if(data == null)
                return;
            
            data.OnEnabledRigEditMode += OnEnabledRigEditMode;
            data.OnEnabledSplineEditMode += OnEnabledSplineEditMode;
            data.OnResetEditMode += OnResetEditMode;
        }

        protected override void UnsubscribeFromEvents()
        {
            var data = MainEditor.Data;
            if(data == null)
                return;
            data.OnEnabledRigEditMode -= OnEnabledRigEditMode;
            data.OnEnabledSplineEditMode -= OnEnabledSplineEditMode;
            data.OnResetEditMode -= OnResetEditMode;
        }
    }
}
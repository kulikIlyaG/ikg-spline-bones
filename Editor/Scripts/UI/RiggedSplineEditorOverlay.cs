using System;
using IKGTools.Editor;
using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.Services;
using IKGTools.Editor.Utilities;
using IKGTools.Editor.Utilities.UI;
using IKGTools.Editor.Utilities.UI.UIToolkit;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    [Overlay(typeof(SceneView), "Spline Bones Binding")]
    internal class RiggedSplineEditorOverlay : BaseTransientOverlay, IEditorOverlay
    {
        private Button _buttonSplineEdit;
        private Button _buttonRigEdit;

        private Button _addBone;
        private Button _removeBone;
        
        public event Action OnClickEditSpline;
        public event Action OnClickEditRig;
        public event Action OnClickAddBone;
        public event Action OnClickRemoveBone;

        void IEditorOverlay.SetActiveSplineButton()
        {
            _buttonSplineEdit.TryAddClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
        }

        void IEditorOverlay.SetActiveRigButton()
        {
            _buttonSplineEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryAddClass(USSClassKeys.BUTTON_ENABLED);
        }

        void IEditorOverlay.SetInteractableRigEditButtons(bool interactable)
        {
            if (interactable)
            {
                _addBone.TryRemoveClass(USSClassKeys.BUTTON_DISABLED);
                _removeBone.TryRemoveClass(USSClassKeys.BUTTON_DISABLED);
            }
            else
            {
                _addBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
                _removeBone.TryAddClass(USSClassKeys.BUTTON_DISABLED);
            }
        }

        void IEditorOverlay.SetActiveAddBoneButton()
        {
            _addBone.TryAddClass(USSClassKeys.BUTTON_ENABLED);
            _removeBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
        }

        void IEditorOverlay.SetActiveRemoveBoneButton()
        {
            _addBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _removeBone.TryAddClass(USSClassKeys.BUTTON_ENABLED);
        }

        void IEditorOverlay.ResetButtons()
        {
            _addBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _removeBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);

            _buttonSplineEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _buttonRigEdit.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
        }

        public override void OnCreated()
        {
            EasyContainerRoot.Resolve<EditorOverlayService>().SetOverlayInstance(this);
            base.OnCreated();
        }

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
            _addBone = result.Q<Button>("add_bone");
            _removeBone = result.Q<Button>("remove_bone");

            _buttonSplineEdit.clicked += OnClickSplineEdit;
            _buttonRigEdit.clicked += OnClickRigEdit;

            _addBone.clicked += OnClickAddBoneEdit;
            _removeBone.clicked += OnClickRemoveBoneEdit;
            return result;
        }


        private void OnClickRigEdit()
        {
            OnClickEditRig?.Invoke();
        }

        private void OnClickSplineEdit()
        {
            OnClickEditSpline?.Invoke();
        }


        private void OnClickRemoveBoneEdit()
        {
            OnClickRemoveBone?.Invoke();
        }

        private void OnClickAddBoneEdit()
        {
            OnClickAddBone?.Invoke();
        }

    }
}
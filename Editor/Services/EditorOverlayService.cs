using System;

namespace IKGTools.Editor.Services
{
    public sealed class EditorOverlayService
    {
        private IEditorOverlay _overlay;

        public event Action OnSetOverlay;

        public event Action OnClickEditSpline;
        public event Action OnClickEditRig;
        public event Action OnClickAddBone;
        public event Action OnClickRemoveBone;

        public void SetOverlayInstance(IEditorOverlay overlay)
        {
            if (_overlay != null)
                throw new Exception("Override overlay! Something wrong!");

            _overlay = overlay;
            _overlay.OnClickEditSpline += OnClickEditSplineInvoke;
            _overlay.OnClickEditRig += OnClickEditRigInvoke;
            _overlay.OnClickAddBone += OnClickAddBoneInvoke;
            _overlay.OnClickRemoveBone += OnClickRemoveBoneInvoke;

            OnSetOverlay?.Invoke();
        }

        private void OnClickEditSplineInvoke()
        {
            OnClickEditSpline?.Invoke();
        }

        private void OnClickEditRigInvoke()
        {
            OnClickEditRig?.Invoke();
        }

        private void OnClickAddBoneInvoke()
        {
            OnClickAddBone?.Invoke();
        }

        private void OnClickRemoveBoneInvoke()
        {
            OnClickRemoveBone?.Invoke();
        }

        public void ShowOverlay()
        {
            _overlay.SetVisible(true);
        }

        public void HideOverlay()
        {
            _overlay.SetVisible(false);
        }

        public void ResetActiveButtons()
        {
            if (_overlay.IsVisible())
                _overlay.ResetButtons();
        }

        public void SetActiveSplineButton()
        {
            if (_overlay.IsVisible())
                _overlay.SetActiveSplineButton();
        }

        public void SetActiveRigButton()
        {
            if (_overlay.IsVisible())
                _overlay.SetActiveRigButton();
        }

        public void SetActiveRigEditButtons()
        {
            if (_overlay.IsVisible())
                _overlay.SetInteractableRigEditButtons(true);
        }

        public void SetInactiveRigEditButtons()
        {
            if (_overlay.IsVisible())
                _overlay.SetInteractableRigEditButtons(false);
        }

        public void SetActiveAddBoneButton()
        {
            if (_overlay.IsVisible())
                _overlay.SetActiveAddBoneButton();
        }

        public void SetActiveRemoveBoneButton()
        {
            if (_overlay.IsVisible())
                _overlay.SetActiveRemoveBoneButton();
        }
    }

    public interface IEditorOverlay
    {
        bool IsVisible();
        void SetVisible(bool visible);
        event Action OnClickEditSpline;
        event Action OnClickEditRig;
        event Action OnClickAddBone;
        event Action OnClickRemoveBone;

        void SetActiveSplineButton();
        void SetActiveRigButton();
        void SetInteractableRigEditButtons(bool interactable);
        void SetActiveAddBoneButton();
        void SetActiveRemoveBoneButton();
        void ResetButtons();
    }
}
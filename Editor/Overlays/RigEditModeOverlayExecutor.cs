using IKGTools.SplineBones.Editor.Utilities.UIToolkit;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class RigEditModeOverlayExecutor
    {
        private readonly IRigEditor _rigEditor;
        private readonly Button _addBone;
        private readonly Button _removeBone;

        public RigEditModeOverlayExecutor(IRigEditor rigEditor, Button addBone, Button removeBone)
        {
            _rigEditor = rigEditor;
            
            _addBone = addBone;
            _removeBone = removeBone;
            
            _addBone.clicked += OnClickAddBoneModeSwitcher;
            _removeBone.clicked += OnClickRemoveBoneSwitcher;

            _rigEditor.OnChangedMode += OnChangedRigEditMode;
        }

        private void OnChangedRigEditMode(RigEditor.Mode mode)
        {
            switch (mode)
            {
                case RigEditor.Mode.AddBone:
                    _addBone.TryAddClass(USSClassKeys.BUTTON_ENABLED);
                    _removeBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
                    break;
                case RigEditor.Mode.RemoveBone:
                    _addBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
                    _removeBone.TryAddClass(USSClassKeys.BUTTON_ENABLED);
                    break;
                default:
                    ResetButtonsView();
                    break;
            }
        }

        public void Dispose()
        {
            _addBone.clicked -= OnClickAddBoneModeSwitcher;
            _removeBone.clicked -= OnClickRemoveBoneSwitcher;
            
            _rigEditor.OnChangedMode -= OnChangedRigEditMode;
            
            ResetButtonsView();
        }

        private void ResetButtonsView()
        {
            _addBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
            _removeBone.TryRemoveClass(USSClassKeys.BUTTON_ENABLED);
        }

        private void OnClickRemoveBoneSwitcher()
        {
            if (_rigEditor!.CurrentMode == RigEditor.Mode.RemoveBone)
                _rigEditor.ResetMode();
            else
                _rigEditor.SetRemoveBoneMode();
        }

        private void OnClickAddBoneModeSwitcher()
        {
            if (_rigEditor!.CurrentMode == RigEditor.Mode.AddBone)
                _rigEditor.ResetMode();
            else
                _rigEditor.SetAddBoneMode();
        }
    }
}
using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor;
using UnityEditor;

namespace IKGTools.SplineBones.Editor.Services.States
{
    internal sealed class EditorRigSubStateRemoveBone : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorDataService _dataService;
        private readonly ManageBonesService _manageBonesService;
        
        private BoneRemoveGUIDrawer _boneRemoveGUIDrawer;

        
        public EditorRigSubStateRemoveBone(EditorOverlayService overlayService,
            EditorDataService editorDataService,
            ManageBonesService manageBonesService,
            IStateMachine stateMachine) : base(stateMachine)
        {
            _dataService = editorDataService;
            _manageBonesService = manageBonesService;
            _overlayService = overlayService;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _overlayService.OnClickAddBone += GoToAddBoneState;
            _overlayService.OnClickRemoveBone += GoToEmptyState;
            
            _boneRemoveGUIDrawer = new BoneRemoveGUIDrawer(_dataService.Component.Skeleton);
            _boneRemoveGUIDrawer.OnClickRemoveBone += OnClickRemoveBone;
            
            _overlayService.SetActiveRemoveBoneButton();
        }

        protected override void OnTickSceneGUI(SceneView sceneView)
        {
            _boneRemoveGUIDrawer.Draw();
        }

        private void OnClickRemoveBone(int boneIndex)
        {
            _manageBonesService.RemoveBone(boneIndex);
        }

        private void GoToAddBoneState()
        {
            GoToState(typeof(EditorRigSubStateAddBone));
        }

        private void GoToEmptyState()
        {
            GoToState(typeof(EditorRigSubStateEmpty));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _overlayService.OnClickAddBone -= GoToAddBoneState;
            _overlayService.OnClickRemoveBone -= GoToEmptyState;
        }

        protected override void OnCloseMachine()
        {
            _overlayService.SetInactiveRigEditButtons();
        }
    }
}
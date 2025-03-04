using Editor.Services.State.States;
using IKGTools.SplineBones.Editor;
using IKGTools.SplineBones.Editor.EasyStateMachine;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Services.States
{
    internal class EditorRigSubStateEmpty : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorDataService _dataService;
        
        private BoneTransformControllersDrawer _boneTransformControllersDrawer;
        
        public EditorRigSubStateEmpty(EditorOverlayService overlayService,
            EditorDataService dataService,
            IStateMachine stateMachine) : base(stateMachine)
        {
            _dataService = dataService;
            _overlayService = overlayService;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _overlayService.OnClickAddBone += GoToAddBone;
            _overlayService.OnClickRemoveBone += GoToRemoveBone;
            
            _boneTransformControllersDrawer = new BoneTransformControllersDrawer(_dataService.Component.Skeleton);
            _boneTransformControllersDrawer.OnUpdatedBoneTransforms += BakeNewBoneTransforms;

            _overlayService.SetActiveRigEditButtons();
        }
        
        protected override void OnTickSceneGUI(SceneView sceneView)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            _boneTransformControllersDrawer.Draw();
        }
        
        private void BakeNewBoneTransforms(int index, Vector2 position, Quaternion rotation)
        {
            _dataService.Component.BindingsData.Bindings.SkeletonDefinition.SetNewTransforms(index, position, rotation);
            EditorUtility.SetDirty(_dataService.Component.BindingsData.Bindings.SkeletonDefinition);
        }

        private void GoToAddBone()
        {
            GoToState(typeof(EditorRigSubStateAddBone));
        }

        private void GoToRemoveBone()
        {
            GoToState(typeof(EditorRigSubStateRemoveBone));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _overlayService.OnClickAddBone -= GoToAddBone;
            _overlayService.OnClickRemoveBone -= GoToRemoveBone;
            
            _boneTransformControllersDrawer = null;
        }

        protected override void OnCloseMachine()
        {
            _overlayService.SetInactiveRigEditButtons();
        }
    }
}
using Editor.Scripts.Services;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.SplineBones.Editor;
using IKGTools.SplineBones.Editor.Services;
using IKGTools.SplineBones.Editor.Services.States;
using UnityEditor;

namespace IKGTools.Editor.Services.States
{
    internal sealed class EmptyEditorState : EditorEasyState
    {
        private readonly EditorDataService _dataService;
        private readonly EditorOverlayService _overlayService;
        private readonly EditorActivityCycleEventsService _activityCycleEvents;

        private readonly DrawObjectsInfoService _drawObjectsInfoService;
        
        private BoneTransformControllersDrawer _boneTransformControllersDrawer;
        private BoneLengthDrawer _boneLengthDrawer;
        
        public EmptyEditorState(EditorOverlayService overlayService
            , EditorActivityCycleEventsService activityCycleEvents,
            DrawObjectsInfoService drawObjectsInfoService
            , IStateMachine stateMachine
            ,EditorDataService editorDataService) : base(stateMachine)
        {
            _dataService = editorDataService;
            _overlayService = overlayService;
            _activityCycleEvents = activityCycleEvents;
            _drawObjectsInfoService = drawObjectsInfoService;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _activityCycleEvents.OnDisabledEditor += GoToDisabledState;
            
            _overlayService.OnClickEditRig += GoToEditRigState;
            _overlayService.OnClickEditSpline += GoToEditSplineState;
            
            _boneTransformControllersDrawer = new BoneTransformControllersDrawer(_dataService.Component.Skeleton);
            _boneLengthDrawer = new BoneLengthDrawer(_dataService.Component.Skeleton);

            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.ResetActiveButtons();
            _overlayService.ShowOverlay();
            _drawObjectsInfoService.Show();
        }

        protected override void OnTickSceneGUI(SceneView sceneView)
        {
            _boneTransformControllersDrawer.Draw();
            _boneLengthDrawer.Draw();
        }

        private void GoToDisabledState()
        {
            GoToState(typeof(DisabledEditorState));
        }

        private void GoToEditRigState()
        {
            GoToState(typeof(RigEditorState));
        }

        private void GoToEditSplineState()
        {
            GoToState(typeof(SplineEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _activityCycleEvents.OnDisabledEditor -= GoToDisabledState;
            
            _overlayService.OnClickEditRig -= GoToEditRigState;
            _overlayService.OnClickEditSpline -= GoToEditSplineState;
        }

        protected override void OnCloseMachine()
        {
            
        }
    }
}
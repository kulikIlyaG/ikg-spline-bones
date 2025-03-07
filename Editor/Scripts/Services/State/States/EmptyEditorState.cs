using Editor.Scripts.Services;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.SplineBones.Editor.Services;
using IKGTools.SplineBones.Editor.Services.States;

namespace IKGTools.Editor.Services.States
{
    internal sealed class EmptyEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorActivityCycleEventsService _activityCycleEvents;

        private readonly DrawObjectsInfoService _drawObjectsInfoService;
        
        public EmptyEditorState(EditorOverlayService overlayService
            , EditorActivityCycleEventsService activityCycleEvents,
            DrawObjectsInfoService drawObjectsInfoService
            , IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _activityCycleEvents = activityCycleEvents;
            _drawObjectsInfoService = drawObjectsInfoService;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _activityCycleEvents.OnDisabledEditor += GoToDisabledState;
            
            _overlayService.OnClickEditRig += GoToEditRigState;
            _overlayService.OnClickEditSpline += GoToEditSplineState;

            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.ResetActiveButtons();
            _overlayService.ShowOverlay();
            _drawObjectsInfoService.Show();
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
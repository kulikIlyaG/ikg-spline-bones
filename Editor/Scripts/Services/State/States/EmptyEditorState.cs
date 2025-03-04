using IKGTools.Editor.EasyStateMachine;
using IKGTools.SplineBones.Editor.Services;
using IKGTools.SplineBones.Editor.Services.States;

namespace IKGTools.Editor.Services.States
{
    internal sealed class EmptyEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorActivitiCycleEventsService _activitiCycleEvents;
        
        public EmptyEditorState(EditorOverlayService overlayService
            , EditorActivitiCycleEventsService activitiCycleEvents
            , IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _activitiCycleEvents = activitiCycleEvents;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _activitiCycleEvents.OnDisabledEditor += GoToDisabledState;
            
            _overlayService.OnClickEditRig += GoToEditRigState;
            _overlayService.OnClickEditSpline += GoToEditSplineState;

            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.ResetActiveButtons();
            _overlayService.ShowOverlay();
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
            _activitiCycleEvents.OnDisabledEditor -= GoToDisabledState;
            
            _overlayService.OnClickEditRig -= GoToEditRigState;
            _overlayService.OnClickEditSpline -= GoToEditSplineState;
        }

        protected override void OnCloseMachine()
        {
            
        }
    }
}
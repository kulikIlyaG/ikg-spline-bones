using Editor.Services.State.States;
using IKGTools.SplineBones.Editor.EasyStateMachine;

namespace IKGTools.Editor.Services.States
{
    internal sealed class EmptyEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorLifeCycleEventsService _lifeCycleEvents;
        
        public EmptyEditorState(EditorOverlayService overlayService
            , EditorLifeCycleEventsService lifeCycleEvents
            , IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _lifeCycleEvents = lifeCycleEvents;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _lifeCycleEvents.OnDisabledEditor += GoToDisabledState;
            
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
            _lifeCycleEvents.OnDisabledEditor -= GoToDisabledState;
            
            _overlayService.OnClickEditRig -= GoToEditRigState;
            _overlayService.OnClickEditSpline -= GoToEditSplineState;
        }

        protected override void OnCloseMachine()
        {
            
        }
    }
}
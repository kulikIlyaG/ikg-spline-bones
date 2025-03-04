using Editor.Services.State.States;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor.EasyStateMachine;

namespace IKGTools.Editor.Services
{
    internal sealed class DisabledEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorLifeCycleEventsService _lifeCycleEvents;
        
        public DisabledEditorState(EditorOverlayService overlayService
            , EditorLifeCycleEventsService lifeCycleEvents
            , IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _lifeCycleEvents = lifeCycleEvents;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _lifeCycleEvents.OnEnabledEditor += OnEnabledEditor;
            
            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.HideOverlay();
        }


        private void OnEnabledEditor()
        {
            GoToState(typeof(EmptyEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _lifeCycleEvents.OnEnabledEditor -= OnEnabledEditor;
        }

        protected override void OnCloseMachine()
        {
        }
    }
}
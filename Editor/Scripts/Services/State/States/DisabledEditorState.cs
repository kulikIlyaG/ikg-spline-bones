using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor.Services;
using IKGTools.SplineBones.Editor.Services.States;

namespace IKGTools.Editor.Services
{
    internal sealed class DisabledEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorActivitiCycleEventsService _activitiCycleEvents;
        
        public DisabledEditorState(EditorOverlayService overlayService
            , EditorActivitiCycleEventsService activitiCycleEvents
            , IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _activitiCycleEvents = activitiCycleEvents;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _activitiCycleEvents.OnEnabledEditor += OnEnabledEditor;
            
            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.HideOverlay();
        }


        private void OnEnabledEditor()
        {
            GoToState(typeof(EmptyEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _activitiCycleEvents.OnEnabledEditor -= OnEnabledEditor;
        }

        protected override void OnCloseMachine()
        {
        }
    }
}
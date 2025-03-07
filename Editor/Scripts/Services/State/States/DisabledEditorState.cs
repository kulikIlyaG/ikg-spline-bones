using Editor.Scripts.Services;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services.States;
using IKGTools.SplineBones.Editor.Services;
using IKGTools.SplineBones.Editor.Services.States;

namespace IKGTools.Editor.Services
{
    internal sealed class DisabledEditorState : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly EditorActivityCycleEventsService _activityCycleEvents;

        private readonly DrawObjectsInfoService _drawObjectsInfoService;
        
        public DisabledEditorState(EditorOverlayService overlayService
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
            _activityCycleEvents.OnEnabledEditor += OnEnabledEditor;
            
            SplineBonesEditorRoot.SetEnabledEditMode(false);
            
            _overlayService.HideOverlay();
            
            _drawObjectsInfoService.Hide();
        }


        private void OnEnabledEditor()
        {
            GoToState(typeof(EmptyEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _activityCycleEvents.OnEnabledEditor -= OnEnabledEditor;
        }

        protected override void OnCloseMachine()
        {
        }
    }
}
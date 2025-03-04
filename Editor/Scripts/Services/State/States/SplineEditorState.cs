using IKGTools.Editor;
using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services;
using IKGTools.Editor.Services.States;
using UnityEngine;

namespace IKGTools.SplineBones.Editor.Services.States
{
    internal sealed class SplineEditorState : BaseEditEditorState
    {
        private const string SCENE_BANNER_MESSAGE = "EDIT SPLINE";
        private static readonly Color SCENE_BANNER_BACKGROUND_COLOR = new Color32(223, 61, 44, 255);
        private static readonly Color SCENE_BANNER_LABEL_COLOR = new Color32(240, 174, 153, 255);
        
        private readonly SceneBannerService _sceneBannerService;
        private readonly SceneBanner _sceneBanner;
        
        private readonly EditorOverlayService _overlayService;
        
        public SplineEditorState(EditorOverlayService overlayService,
            SceneBannerService sceneBannerService,
            IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _sceneBannerService = sceneBannerService;
            
            GUIStyle bannerLabelStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = SCENE_BANNER_LABEL_COLOR
                }
            };

            _sceneBanner = new SceneBanner(SCENE_BANNER_BACKGROUND_COLOR, 24f, bannerLabelStyle, SCENE_BANNER_MESSAGE);
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _overlayService.OnClickEditSpline += GoToEmptyState;
            _overlayService.OnClickEditRig += GoToEditRigState;
            
            SplineBonesEditorRoot.SetEnabledEditMode(true);
            
            _sceneBannerService.AddBanner(SCENE_BANNER_MESSAGE, _sceneBanner);
            
            _overlayService.SetActiveSplineButton();
        }

        private void GoToEditRigState()
        {
            GoToState(typeof(RigEditorState));
        }

        private void GoToEmptyState()
        {
            GoToState(typeof(EmptyEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _overlayService.OnClickEditSpline -= GoToEmptyState;
            _overlayService.OnClickEditRig -= GoToEditRigState;
            
            _sceneBannerService.RemoveBanner(SCENE_BANNER_MESSAGE);
        }

        protected override void OnCloseMachine()
        {
        }
    }
}
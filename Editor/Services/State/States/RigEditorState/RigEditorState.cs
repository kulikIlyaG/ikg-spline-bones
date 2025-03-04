using IKGTools.Editor.EasyContainerEditor;
using IKGTools.SplineBones.Editor;
using IKGTools.SplineBones.Editor.EasyStateMachine;
using UnityEngine;

namespace IKGTools.Editor.Services.States
{
    internal sealed class RigEditorState : BaseEditEditorState
    {
        private const string SCENE_BANNER_MESSAGE = "EDIT SKELETON";
        private static readonly Color SCENE_BANNER_BACKGROUND_COLOR = new Color32(96, 137, 223, 255);
        private static readonly Color SCENE_BANNER_LABEL_COLOR = new Color32(202, 214, 240, 255);
        
        private readonly EditorOverlayService _overlayService;
        private readonly RigEditorSubStateMachine _subStateMachine;
        
        private readonly RigEditorContextMenuService _contextMenuService;
        private readonly SceneBannerService _sceneBannerService;
        private readonly SceneBanner _sceneBanner;
        
        
        public RigEditorState(EditorOverlayService overlayService,
            RigEditorContextMenuService contextMenuService,
            SceneBannerService sceneBannerService,
            DIContainer container,
            IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _contextMenuService = contextMenuService;
            _sceneBannerService = sceneBannerService;
            
            _subStateMachine = new RigEditorSubStateMachine(container);
            
            GUIStyle bannerLabelStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = SCENE_BANNER_LABEL_COLOR
                }
            };
            _sceneBanner = new SceneBanner(SCENE_BANNER_BACKGROUND_COLOR, 24f, bannerLabelStyle, SCENE_BANNER_MESSAGE);

        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _overlayService.OnClickEditRig += GoToEmptyState;
            _overlayService.OnClickEditSpline += GoToEditSplineState;
            
            SplineBonesEditorRoot.SetEnabledEditMode(true);
            
            _overlayService.SetActiveRigButton();
            
            _sceneBannerService.AddBanner(SCENE_BANNER_MESSAGE, _sceneBanner);
            _contextMenuService.SetEnabled();
            _subStateMachine.Start();
        }
        

        private void GoToEditSplineState()
        {
            GoToState(typeof(SplineEditorState));
        }

        private void GoToEmptyState()
        {
            GoToState(typeof(EmptyEditorState));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _overlayService.OnClickEditRig -= GoToEmptyState;
            _overlayService.OnClickEditSpline -= GoToEditSplineState;

            _sceneBannerService.RemoveBanner(SCENE_BANNER_MESSAGE);
            
            _contextMenuService.SetDisabled();
            _subStateMachine.Close();
        }

        protected override void OnCloseMachine()
        {
        }
    }
}
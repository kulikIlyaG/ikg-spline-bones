using System;
using IKGTools.Editor.EasyContainerEditor;
using IKGTools.Editor.Services;
using IKGTools.SplineBones;
using IKGTools.SplineBones.Editor.Services;
using UnityEditor;

namespace IKGTools.Editor
{
    [InitializeOnLoad]
    public static class SplineBonesEditorRoot
    {
        private static readonly BaseInstaller[] INSTALLERS = {
            new MainInstaller()
        };

        internal static EditorStateMachineService _stateMachine;
        internal static EditorOverlayService _overlayService;

        private static EditorDataService _dataService;

        private static bool _isEditModeSelected;
        
        public static event Action OnSelectedEditMode;
        public static event Action OnDeselectedEditMode;
        
        static SplineBonesEditorRoot()
        {
            EnterPoint();
        }
        
        internal static EditorDataService EditorDataService { get; private set; }

        private static void EnterPoint()
        {
            EasyContainer.Initialize(INSTALLERS);

            ResolveReferences();

            _overlayService.OnSetOverlay += InitializeEditor;
        }

        private static void InitializeEditor()
        {
            _stateMachine.Initialize();
        }

        private static void ResolveReferences()
        {
            EditorDataService = EasyContainer.Resolve<EditorDataService>();
            _stateMachine = EasyContainer.Resolve<EditorStateMachineService>();
            _overlayService = EasyContainer.Resolve<EditorOverlayService>();
            _dataService = EasyContainer.Resolve<EditorDataService>();
        }

        private sealed class MainInstaller : BaseInstaller
        {
            public override void Install(DIContainer container)
            {
                container.CreateAndRegister<EditorApplicationModeObserver>();
                
                container.CreateAndRegister<EditorDataService>();
                container.CreateAndRegister<ManageBonesService>();
                
                container.CreateAndRegister<EditorLifeCycleEventsService>();
                container.CreateAndRegister<EditorOverlayService>();

                container.CreateAndRegister<SceneBannerService>();
                container.CreateAndRegister<RigEditorContextMenuService>();
                
                container.CreateAndRegister<EditorStateMachineService>();
            }
        }

        public static void TryUpdateBoneInstances(SkeletonComponent skeleton, SkeletonDefinition.BoneTransforms[] bonesTransforms)
        {
            EasyContainer.Resolve<ManageBonesService>().TryUpdateBoneInstances(skeleton, bonesTransforms);
        }

        public static void SetEnabledEditMode(bool enabled)
        {
            if(_isEditModeSelected == enabled)
                return;
            
            _isEditModeSelected = enabled;
            if (_isEditModeSelected)
            {
                _dataService.TryStopExecuteComponent();
                OnSelectedEditMode?.Invoke();
            }
            else
            {
                _dataService.TryStartExecute();
                OnDeselectedEditMode?.Invoke();
            }
        }
    }
}
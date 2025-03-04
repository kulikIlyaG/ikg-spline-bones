using System;
using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.Services;
using IKGTools.SplineBones;
using IKGTools.SplineBones.Editor.Services;
using UnityEditor;

namespace IKGTools.Editor
{
    [InitializeOnLoad]
    internal static class SplineBonesEditorRoot
    {
        private static readonly BaseInstaller[] INSTALLERS = {
            new MainInstaller()
        };

        private static EditorStateMachineService _stateMachine;
        private static EditorOverlayService _overlayService;

        private static EditorActivitiCycleEventsService _activitiCycleEvents;
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
            EasyContainerRoot.Initialize(INSTALLERS);

            ResolveReferences();

            _overlayService.OnSetOverlay += InitializeEditor;
        }

        private static void InitializeEditor()
        {
            _stateMachine.Initialize();
        }

        private static void ResolveReferences()
        {
            EditorDataService = EasyContainerRoot.Resolve<EditorDataService>();
            _stateMachine = EasyContainerRoot.Resolve<EditorStateMachineService>();
            _overlayService = EasyContainerRoot.Resolve<EditorOverlayService>();
            _dataService = EasyContainerRoot.Resolve<EditorDataService>();
            _activitiCycleEvents = EasyContainerRoot.Resolve<EditorActivitiCycleEventsService>();
        }

        private sealed class MainInstaller : BaseInstaller
        {
            public override void Install(DIContainer container)
            {
                container.CreateAndRegister<EditorApplicationModeObserver>();
                
                container.CreateAndRegister<EditorDataService>();
                container.CreateAndRegister<ManageBonesService>();
                
                container.CreateAndRegister<EditorActivitiCycleEventsService>();
                container.CreateAndRegister<EditorOverlayService>();

                container.CreateAndRegister<SceneBannerService>();
                container.CreateAndRegister<RigEditorContextMenuService>();
                
                container.CreateAndRegister<EditorStateMachineService>();
            }
        }

        public static void TryUpdateBoneInstances(SkeletonComponent skeleton, SkeletonDefinition.BoneTransforms[] bonesTransforms)
        {
            EasyContainerRoot.Resolve<ManageBonesService>().TryUpdateBoneInstances(skeleton, bonesTransforms);
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

        public static void TryStartEdit()
        {
            _activitiCycleEvents.TryStartEdit();
        }
    }
}
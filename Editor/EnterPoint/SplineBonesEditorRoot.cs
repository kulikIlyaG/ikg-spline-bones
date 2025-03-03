using IKGTools.Editor.EasyContainerEditor;
using IKGTools.Editor.Services;
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
        
        static SplineBonesEditorRoot()
        {
            EnterPoint();
        }
        
        internal static EditorDataService EditorDataService { get; private set; }

        private static void EnterPoint()
        {
            EasyContainer.Initialize(INSTALLERS);
            
            EditorDataService = EasyContainer.Resolve<EditorDataService>();
        }
        
        private sealed class MainInstaller : BaseInstaller
        {
            public override void Install(DIContainer container)
            {
                container.CreateAndRegister<EditorApplicationModeObserver>();
                container.CreateAndRegister<EditorDataService>();
            }
        }
    }
}
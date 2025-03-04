using IKGTools.Editor.EasyContainerEditor;
using IKGTools.SplineBones.Editor.EasyStateMachine;
using UnityEditor;

namespace Editor.Services.State.States
{
    public abstract class EditorEasyState : EasyState, ITickableSceneGUI
    {
        protected EditorEasyState(IStateMachine stateMachine) : base(stateMachine)
        {
        }

        public void TickSceneGUI(SceneView sceneView)
        {
            if (_isActive)
                OnTickSceneGUI(sceneView);
        }

        protected virtual void OnTickSceneGUI(SceneView sceneView){}
    }
}
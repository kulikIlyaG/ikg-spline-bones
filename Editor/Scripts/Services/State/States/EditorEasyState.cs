using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.EasyStateMachine;
using UnityEditor;

namespace IKGTools.SplineBones.Editor.Services.States
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
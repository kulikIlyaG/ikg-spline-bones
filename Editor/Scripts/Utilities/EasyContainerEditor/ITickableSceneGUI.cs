using UnityEditor;

namespace IKGTools.Editor.EasyContainer
{
    public interface ITickableSceneGUI
    {
        void TickSceneGUI(SceneView sceneView);
    }
}
using UnityEditor;

namespace IKGTools.Editor.EasyContainerEditor
{
    public interface ITickableSceneGUI
    {
        void TickSceneGUI(SceneView sceneView);
    }
}
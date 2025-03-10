using IKGTools.Editor.EasyContainer;
using IKGTools.Editor.Services;
using IKGTools.SplineBones.Editor;
using UnityEditor;

namespace Editor.Scripts.Services
{
    internal sealed class DrawObjectsInfoService : ITickableSceneGUI
    {
        private readonly EditorDataService _data;

        private BoneAndSplineInfoDrawer _drawer;
        private bool _show;

        public DrawObjectsInfoService(EditorDataService data)
        {
            _data = data;
        }

        public void Show()
        {
            _drawer = new BoneAndSplineInfoDrawer(_data.Component.Skeleton, _data.Component.BindingsData,
                _data.Component.Spline);
        }

        public void Hide()
        {
            _drawer = null;
        }

        public void TickSceneGUI(SceneView sceneView)
        {
            if(_drawer != null)
                _drawer.Draw();
        }
    }
}
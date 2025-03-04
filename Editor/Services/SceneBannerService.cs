using System.Collections.Generic;
using IKGTools.Editor.EasyContainerEditor;
using IKGTools.SplineBones.Editor;
using UnityEditor;

namespace IKGTools.Editor.Services
{
    internal sealed class SceneBannerService : ITickableSceneGUI
    {
        private readonly Dictionary<string, SceneBanner> _banners = new(2);
        
        public void TickSceneGUI(SceneView sceneView)
        {
            foreach (var banner in _banners)
            {
                banner.Value.Draw(sceneView);
            }
        }

        public void AddBanner(string id, SceneBanner instance)
        {
            _banners.Add(id, instance);
        }

        public void RemoveBanner(string id)
        {
            _banners.Remove(id);
        }
    }
}
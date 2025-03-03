using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class SplineEditor
    {
        private const string SCENE_BANNER_MESSAGE = "EDIT SPLINE";
        private static readonly Color SCENE_BANNER_BACKGROUND_COLOR = new Color32(223, 61, 44, 255);
        private static readonly Color SCENE_BANNER_LABEL_COLOR = new Color32(240, 174, 153, 255);
        
        private readonly SceneBanner _sceneBanner;

        public SplineEditor()
        {
            GUIStyle bannerLabelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = SCENE_BANNER_LABEL_COLOR
                }
            };

            _sceneBanner = new SceneBanner(SCENE_BANNER_BACKGROUND_COLOR, 24f, bannerLabelStyle);
            
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSceneGUI(SceneView obj)
        {
            _sceneBanner.Draw(obj, SCENE_BANNER_MESSAGE);
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}
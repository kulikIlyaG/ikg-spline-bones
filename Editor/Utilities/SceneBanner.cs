using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class SceneBanner
    {
        private readonly GUIStyle _labelStyle;
        private readonly Color _backgroundColor;
        private readonly float _height;

        private readonly string _message;
        
        public SceneBanner(Color backgroundColor, float height, GUIStyle labelStyle, string message)
        {
            _labelStyle = labelStyle;

            _backgroundColor = backgroundColor;
            _height = height;
            _message = message;
        }

        public void Draw(SceneView sceneView)
        {
            var screenRect = sceneView.position;
            var bannerHeight = _height;

            Handles.BeginGUI();

            var bannerRect = new Rect(
                0, 
                screenRect.height - (bannerHeight + 26f), 
                screenRect.width, 
                bannerHeight
            );
            
            GUI.BeginGroup(bannerRect);

            EditorGUI.DrawRect(new Rect(0, 0, bannerRect.width, bannerRect.height), 
                _backgroundColor);

            EditorGUI.LabelField(new Rect(0, 0, bannerRect.width, bannerRect.height), _message, _labelStyle);
            Color oldColor = GUI.color;
            
        
            GUI.EndGroup();
            GUI.color = oldColor;
            Handles.EndGUI();
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class BoneRemoveGUIDrawer
    {
        private static readonly Color BUTTON_FONT_COLOR = new Color(0.9f, 0.3f, 0.23f, 1f);
        
        private readonly SkeletonComponent _skeleton;
        private readonly GUIStyle _removeBoneButtonStyle;
        
        public event Action<int> OnClickRemoveBone;

        public BoneRemoveGUIDrawer(SkeletonComponent skeleton)
        {
            _skeleton = skeleton;

            _removeBoneButtonStyle = new GUIStyle(EditorStyles.miniButton);
                
            _removeBoneButtonStyle.normal.textColor = BUTTON_FONT_COLOR;
            _removeBoneButtonStyle.active.textColor = BUTTON_FONT_COLOR - new Color(0.2f,0.2f,0.2f, 0f);
            _removeBoneButtonStyle.fontStyle = FontStyle.Bold;
        }

        public void Draw()
        {
            var bones = _skeleton.Bones;
            var size = GetSizeByDistance(0.02f);

            for (int index = 0; index < bones.Length; index++)
            {
                if(bones[index] != null)
                    DrawBoneGUI(bones[index].transform.position, index, size);
            }
        }

        private void DrawBoneGUI(Vector3 worldPosition, int index, float size)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            
            if(sceneCamera == null) return;

            Vector2 viewportPoint = sceneCamera.WorldToViewportPoint(worldPosition);
            if (viewportPoint.x < 0f || viewportPoint.x > 1f ||
                viewportPoint.y < 0f || viewportPoint.y > 1f)
                return;

            Vector2 guiPoint = HandleUtility.WorldToGUIPoint(worldPosition);

            float halfSize = 10f;
            Rect buttonRect = new Rect(
                guiPoint.x - halfSize,
                guiPoint.y - halfSize,
                halfSize * 2f,
                halfSize * 2f
            );
            buttonRect.position += new Vector2(0f, 25f);

            Handles.BeginGUI();
            if (GUI.Button(buttonRect, "X", _removeBoneButtonStyle))
            {
                OnClickRemoveBone?.Invoke(index);
                Event.current.Use();
            }
            Handles.EndGUI();
        }

        private float GetSizeByDistance(float value)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            float distance = Vector3.Distance(sceneCamera.transform.position, _skeleton.transform.position);
            float scale = distance * value;
            return scale;
        }
    }
}
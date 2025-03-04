using IKGTools.Editor.EasyContainer;
using UnityEditor;
using UnityEngine;

namespace IKGTools.Editor.Services
{
    internal sealed class RigEditorContextMenuService : ITickableSceneGUI
    {
        private bool _isEnabled = false;

        private Vector2 _clickPosition;

        public void SetEnabled()
        {
            _isEnabled = true;
        }

        public void SetDisabled()
        {
            _isEnabled = false;
        }

        public void TickSceneGUI(SceneView sceneView)
        {
            if (!_isEnabled)
            {
                return;
            }

            Event evnt = Event.current;
            if (evnt != null && evnt.type == EventType.ContextClick)
            {
                Vector2 mousePosition = evnt.mousePosition;
                _clickPosition = GetWorldPosition(sceneView, mousePosition);
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create bone"), false, CreateBoneContextCommand);
                menu.ShowAsContext();
                evnt.Use();
            }
        }

        private static void CreateBoneContextCommand()
        {
            Debug.Log("Context menu add button pressed");
        }

        private static Vector2 GetWorldPosition(SceneView sceneView, Vector2 screenPosition)
        {
            screenPosition.y = sceneView.camera.pixelHeight - screenPosition.y;

            Ray ray = sceneView.camera.ScreenPointToRay(screenPosition);

            Plane plane = new Plane(-Vector3.forward, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);

            return Vector3.zero;
        }
    }
}
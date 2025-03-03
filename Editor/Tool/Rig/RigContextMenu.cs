using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    [InitializeOnLoad]
    internal static class RigContextMenu
    {
        private static Vector2 _clickPosition;
        private static RigEditor _rigEditor;
        
        static RigContextMenu()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void SetRigEditor(RigEditor rigEditor)
        {
            _rigEditor = rigEditor;
        }

        public static void RemoveRigEditor()
        {
            _rigEditor = null;
        }
        
        private static void OnSceneGUI(SceneView sceneView)
        {
            if(_rigEditor == null)
                return;
            
            Event evnt = Event.current;
            if (evnt != null && evnt.type == EventType.ContextClick)
            {
                Vector2 mousePosition = evnt.mousePosition;
                _clickPosition = GetWorldPosition(sceneView, mousePosition);
                EditorUtility.DisplayPopupMenu(new Rect(mousePosition, new Vector2(100, 100)), "RiggedSplineContextMenu", null);
                evnt.Use();
            }
        }
        
        [MenuItem("RiggedSplineContextMenu/Create bone")]
        private static void CreateBoneContextCommand()
        {
            _rigEditor.AddBone(_clickPosition);
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
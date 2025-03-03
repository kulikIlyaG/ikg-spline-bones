using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class StaticCommands
    {
        [MenuItem("GameObject/Rigged Spline", false, 0)]
        private static void CreateRiggedSplineBlankObject()
        {
            var parent = Selection.activeGameObject;
            var created = RiggedSplineCreateHelper.CreateBlank(GetSceneCameraPosition(), parent != null ? parent.transform : null);
            Selection.activeGameObject = created.gameObject;
        }

        public static Vector2 GetSceneCameraPosition()
        {
            var view = SceneView.lastActiveSceneView;
            
            if (view != null && view.camera != null)
                return view.camera.transform.position;
        
            return Vector2.zero;
        }
    }
}
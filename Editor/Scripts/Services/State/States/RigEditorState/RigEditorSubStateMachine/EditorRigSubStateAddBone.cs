using IKGTools.Editor.EasyStateMachine;
using IKGTools.Editor.Services;
using IKGTools.Editor.Services.States;
using IKGTools.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor.Services.States
{
    internal sealed class EditorRigSubStateAddBone : EditorEasyState
    {
        private readonly EditorOverlayService _overlayService;
        private readonly ManageBonesService _manageBonesService;
        
        
        public EditorRigSubStateAddBone(EditorOverlayService overlayService,
            ManageBonesService manageBonesService,
            IStateMachine stateMachine) : base(stateMachine)
        {
            _overlayService = overlayService;
            _manageBonesService = manageBonesService;
        }

        protected override void OnEnter(EasyState enterFrom)
        {
            _overlayService.OnClickAddBone += GoToEmptyState;
            _overlayService.OnClickRemoveBone += GoToRemoveBoneState;

            _overlayService.SetActiveAddBoneButton();
        }

        protected override void OnTickSceneGUI(SceneView sceneView)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.CustomCursor);
            Cursor.SetCursor(EditorAssetsHelper.Get<Texture2D>(PathHelper.GetPathForEditorTexture(PathHelper.Keys.CursorAddBone)), Vector2.zero, CursorMode.Auto);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(-Vector3.forward, Vector3.zero);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector2 worldPosition = ray.GetPoint(enter);

                    _manageBonesService.AddBone(worldPosition);
                }

                e.Use();
            }
        }
        
        private void GoToEmptyState()
        {
            GoToState(typeof(EditorRigSubStateEmpty));
        }

        private void GoToRemoveBoneState()
        {
            GoToState(typeof(EditorRigSubStateRemoveBone));
        }

        protected override void OnExit(EasyState exitTo)
        {
            _overlayService.OnClickAddBone -= GoToEmptyState;
            _overlayService.OnClickRemoveBone -= GoToRemoveBoneState;
        }

        protected override void OnCloseMachine()
        {
            _overlayService.SetInactiveRigEditButtons();
        }
    }
}
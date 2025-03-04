using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IKGTools.SplineBones.Editor
{
    internal interface IRigEditor
    {
        RigEditor.Mode CurrentMode { get; }
        event Action<RigEditor.Mode> OnChangedMode;
        void SetAddBoneMode();
        void SetRemoveBoneMode();
        void ResetMode();
    }
    internal sealed class RigEditor : IRigEditor
    {
        public enum Mode
        {
            None,
            AddBone,
            RemoveBone
        }
        private readonly Data _editorData;

        private bool _isDisposed = false;
        private Mode _mode;
        
        public event Action<Mode> OnChangedMode;

        internal RigEditor(Data editorData)
        {
            _editorData = editorData;

        }


        private SkeletonComponent Skeleton => _editorData.Skeleton;
        public Mode CurrentMode => _mode;

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            
            _isDisposed = true;
        }
        
        private void OnSceneGUI(SceneView obj)
        {
            
            if (Skeleton == null && !_isDisposed)
                Dispose();
            
        }

        private void OnSceneGUIForNoneMode(SceneView sceneView)
        {
            // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //
            // if (Skeleton.Bones == null || Skeleton.Bones.Length == 0)
            //     return;
            //
            // _boneTransformControllersDrawer.Draw();
        }

        private void OnSceneGUIForRemoveBoneMode(SceneView sceneView)
        {
            
        }

        private void OnSceneGUIForAddBoneMode(SceneView sceneView)
        {
           
        }


        
        private void OnClickRemoveBone(int boneIndex)
        {
            
            if(Skeleton.Bones.Length <= 0)
                ResetMode();
        }
        
        public void SetAddBoneMode()
        {
            SetMode(Mode.AddBone);
        }

        public void SetRemoveBoneMode()
        {
            SetMode(Mode.RemoveBone);
        }

        public void ResetMode()
        {
            SetMode(Mode.None);
        }

        private void SetMode(Mode mode)
        {
            if (_mode == mode)
            {
                Debug.LogWarning($"You try set mode: {mode}; second times");
                return;
            }
            
            _mode = mode;
            OnChangedMode?.Invoke(_mode);
        }
    }
}
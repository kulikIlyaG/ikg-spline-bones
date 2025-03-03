using System;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class BoneTransformControllersDrawer
    {
        private static readonly Color _rotationDiscColor = new Color32(255, 102, 60, 255);

        private readonly SkeletonComponent _skeleton;

        public event Action<int, Vector2, Quaternion> OnUpdatedBoneTransforms;

        public BoneTransformControllersDrawer(SkeletonComponent skeleton)
        {
            _skeleton = skeleton;
        }


        public Bone SelectedBone { get; private set; }

        public void Draw()
        {
            var bones = _skeleton.Bones;
            var size = GetSizeByDistance(0.05f);

            for (int index = 0; index < bones.Length; index++)
            {
                if (bones[index] == null) continue;

                Transform boneTransform = bones[index].transform;
                
                DrawBone(boneTransform, size, bones, index);
            }
        }


        private void DrawBone(Transform boneTransform, float size, Bone[] bones, int index)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(boneTransform.position, boneTransform.rotation);
                
            Handles.color = _rotationDiscColor;
            Quaternion newRot = Handles.Disc(boneTransform.rotation, boneTransform.position, Vector3.forward, size, false, 15f);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(boneTransform, "Move/Rotate Bone");
                boneTransform.position = newPos;
                boneTransform.rotation = newRot;
                OnUpdatedBoneTransforms?.Invoke(index, newPos, newRot);
                SelectedBone = bones[index];
            }
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
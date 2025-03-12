using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    internal sealed class BoneLengthDrawer
    {
        private readonly SkeletonComponent _skeleton;

        public BoneLengthDrawer(SkeletonComponent skeleton)
        {
            _skeleton = skeleton;
        }

        public void Draw()
        {
            if(_skeleton == null)
                return;
            
            var bones = _skeleton.Bones;
            var selection = Selection.objects;
            
            if(bones == null)
                return;

            foreach (var bone in bones)
            {
                if (bone == null) continue;
                
                if(selection.Contains(bone.gameObject))
                    continue;
                
                DrawBoneLength(bone);
            }
        }

        public static void DrawBoneLength(Bone bone)
        {
            Vector3 originPosition = bone.Origin.position;
            Vector3 localDirection = new Vector3(1, 1, 0).normalized;
            Vector3 worldDirection = bone.Origin.rotation * localDirection;

            Vector3 sliderPosition = originPosition;

            EditorGUI.BeginChangeCheck();
            Handles.color = Color.magenta;

            float newLength = Handles.ScaleSlider(
                bone.Length, sliderPosition, worldDirection, Quaternion.identity,
                HandleUtility.GetHandleSize(sliderPosition), 0.1f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bone, "Change Bone Length");
                bone.SetLength(newLength);
                EditorUtility.SetDirty(bone);
            }
        }
    }
}
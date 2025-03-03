using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor
{
    [CustomEditor(typeof(Bone), true)]
    [CanEditMultipleObjects] 
    internal class BoneEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is Bone bone)
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
    }
}
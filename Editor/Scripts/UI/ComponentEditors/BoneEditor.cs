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
                if(obj == null)
                    continue;

                var gameObject = obj as GameObject;
                
                if(gameObject == null)
                    continue;
                
                var bone = gameObject.GetComponent<Bone>();
                
                if (bone != null)
                    BoneLengthDrawer.DrawBoneLength(bone);
            }
        }
    }
}
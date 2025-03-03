using IKGTools.SplineBones;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones.Editor
{
    internal static class RiggedSplineCreateHelper
    {
        private static Texture2D _boneIcon;
        
        public static SplineBonesComponent CreateBlank(Vector2 position, Transform parent)
        {
            GameObject rootInstance = new GameObject("RiggedSpline");
            rootInstance.transform.position = position;

            GameObject skeletonInstance = new GameObject("Skeleton");
            skeletonInstance.AddComponent<SkeletonComponent>();

            GameObject viewInstance = new GameObject("Spline");
            viewInstance.AddComponent<SpriteShapeRenderer>();
            viewInstance.AddComponent<SpriteShapeController>();
            viewInstance.AddComponent<SpriteShapeSplineProviderComponent>();
            
            if(parent != null)
                rootInstance.transform.SetParent(parent, false);
            
            skeletonInstance.transform.SetParent(rootInstance.transform, false);
            viewInstance.transform.SetParent(rootInstance.transform, false);
            
            var resultComponent = rootInstance.AddComponent<SplineBonesComponent>();
            return resultComponent;
        }

        public static Bone CreateBlankBone(Vector2 position, int currentBonesCount)
        {
            GameObject boneInstance = new GameObject($"bone_{currentBonesCount:000}");
            var bone = boneInstance.AddComponent<Bone>();
            boneInstance.transform.position = position;

            var boneIcon = GetBoneIcon();
            EditorGUIUtility.SetIconForObject(boneInstance, boneIcon);
            EditorUtility.SetDirty(boneInstance);
            
            return bone;
        }

        public static Bone[] CreateBonesByDefinition(SkeletonDefinition.BoneTransforms[] transforms)
        {
            Bone[] result = new Bone[transforms.Length];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = CreateBlankBone(transforms[index].WorldPosition, index);
                result[index].transform.rotation = transforms[index].Rotation;
            }
            
            return result;
        }

        private static Texture2D GetBoneIcon()
        {
            if (_boneIcon == null)
            {
                _boneIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    PathHelper.GetPathForEditorTexture(PathHelper.Keys.BoneColoredIcon00));
            }

            return _boneIcon;
        }
    }
}
using System;
using System.Linq;
using UnityEngine;

namespace IKGTools.SplineBones
{
    [CreateAssetMenu(fileName = "SkeletonDefinition", menuName = "IKGTools/Spline Bones/Skeleton")]
    public sealed class SkeletonDefinition : ScriptableObject
    {
        [Serializable]
        private class BoneDefinition
        {
            [SerializeField] private Vector2 _worldPosition;
            public Vector2 WorldPosition => _worldPosition;

            [SerializeField] private Quaternion _rotation;
            public Quaternion Rotation => _rotation;

#if UNITY_EDITOR
            [SerializeField] private Color _guiColor;
            public Color GUIColor => _guiColor;

            public BoneDefinition(Vector2 worldPosition, Color guiColor)
            {
                _worldPosition = worldPosition;
                _guiColor = guiColor;
            }
            
            public void SetNewTransforms(Vector2 position, Quaternion rotation)
            {
                _worldPosition = position;
                _rotation = rotation;
            }
#endif
        }
        
        public struct BoneTransforms
        {
            public readonly Vector2 WorldPosition;
            public readonly Quaternion Rotation;

            public BoneTransforms(Vector2 worldPosition, Quaternion rotation)
            {
                WorldPosition = worldPosition;
                Rotation = rotation;
            }
        }

        [SerializeField] private BoneDefinition[] _bones = Array.Empty<BoneDefinition>();

        public int BonesCount => _bones?.Length ?? 0;


        public BoneTransforms[] GetBonesTransform()
        {
            BoneTransforms[] result = new BoneTransforms[_bones.Length];
            for (int index = 0; index < result.Length; index++)
            {
                var bone = _bones[index];
                result[index] = new BoneTransforms(bone.WorldPosition, bone.Rotation);
            }
            return result;
        }
        
        
#if UNITY_EDITOR
        /// <summary>
        /// Only for Editor!
        /// </summary>
        public bool IsNotEmpty => _bones is {Length: > 0};
        public void AddBone(Vector2 worldPosition, Color color)
        {
            var list = _bones.ToList();
            list.Add(new BoneDefinition(worldPosition, color));
            _bones = list.ToArray();
        }


        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="index"></param>
        public void RemoveBone(int index)
        {
            var list = _bones.ToList();
            list.RemoveAt(index);
            _bones = list.ToArray();
        }
        
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetNewTransforms(int index, Vector2 position, Quaternion rotation)
        {
            _bones[index].SetNewTransforms(position, rotation);
        }
        
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <returns></returns>
        public Color[] GetBonesColors()
        {
            if(_bones is {Length: > 0})
                return _bones.Select(element => element.GUIColor).ToArray();

            return Array.Empty<Color>();
        }
#endif
    }
}
using System;
using UnityEngine;

namespace IKGTools.SplineBones
{
    [Serializable]
    public struct BoneWeight
    {
        [SerializeField] private int _boneIndex;
        [SerializeField] [Range(0f, 1f)] private float _weight;

        public int BoneIndex => _boneIndex;
        public float Weight => _weight;


        public BoneWeight(int boneIndex, float weight)
        {
            _boneIndex = boneIndex;
            _weight = weight;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only for editor!
        /// </summary>
        /// <param name="weight"></param>
        public void SetWeight(float weight)
        {
            _weight = weight;
        }

        /// <summary>
        /// Only for editor!
        /// </summary>
        /// <param name="newIndex"></param>
        public void SetNewBoneIndex(int newIndex)
        {
            _boneIndex = newIndex;
        }   
#endif
    }
}
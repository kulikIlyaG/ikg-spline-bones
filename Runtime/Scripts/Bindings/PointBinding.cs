using System;
using UnityEngine;

namespace IKGTools.SplineBones
{
    [Serializable]
    public class PointBinding
    {
        [SerializeField] private int _pointIndex;
        [SerializeField] private BoneWeight[] _bones;

        public PointBinding(int pointIndex, BoneWeight[] bones)
        {
            _pointIndex = pointIndex;
            _bones = bones;
        }

        public int PointIndex => _pointIndex;
        public BoneWeight[] Bones => _bones;

        #if UNITY_EDITOR
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="bones"></param>
        public void SetBones(BoneWeight[] bones)
        {
            _bones = bones;
        }
        #endif
    }
}
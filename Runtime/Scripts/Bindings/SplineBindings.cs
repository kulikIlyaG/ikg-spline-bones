using System;
using UnityEngine;

namespace IKGTools.SplineBones
{
    [Serializable]
    public sealed class SplineBindings
    {
        [SerializeField] private SkeletonDefinition _skeletonDefinition;
        [SerializeField] private PointBinding[] _points = Array.Empty<PointBinding>();
        public PointBinding[] Points => _points;
        public SkeletonDefinition SkeletonDefinition => _skeletonDefinition;

        

#if UNITY_EDITOR
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="points"></param>
        public void SetPointBindings(PointBinding[] points)
        {
            _points = points;
        }
        
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="obj"></param>
        public void SetSkeletonDefinition(SkeletonDefinition obj)
        {
            _skeletonDefinition = obj;
        }
#endif
    }
}
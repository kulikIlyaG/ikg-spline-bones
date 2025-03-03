using UnityEngine;

namespace IKGTools.SplineBones
{
    public sealed class SkeletonComponent : MonoBehaviour
    {
        [SerializeField] private Bone[] _bones;

        public Bone[] Bones => _bones;

#if UNITY_EDITOR
        /// <summary>
        /// Only for Editor!
        /// </summary>
        /// <param name="skeletonBones"></param>
        public void SetBones(Bone[] skeletonBones)
        {
            _bones = skeletonBones;
        }
#endif
    }
}
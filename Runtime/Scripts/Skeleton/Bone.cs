using UnityEngine;

namespace IKGTools.SplineBones
{
    public sealed class Bone : MonoBehaviour
    {
        [SerializeField] private float _length = 1f;

        public float Length => _length;
        public Transform Origin => transform;

        public Vector3 GetPosition()
        {
            return Origin.transform.localPosition;
        }

        public Quaternion GetRotation()
        {
            return Origin.transform.localRotation;
        }

        public void SetLength(float length)
        {
            _length = length;
        }
    }
}
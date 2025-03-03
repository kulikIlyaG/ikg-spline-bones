using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones
{
    internal sealed class BonesSplineExecutor
    {
        private readonly Spline _spline;
        private readonly SplineBindings _bindings;
        private readonly Bone[] _bones;

        private Vector3[] _initialPositionOffsets;
        private Quaternion[] _initialRotations;
        private Vector3[] _initialLeftTangents;
        private Vector3[] _initialRightTangents;
        
        private bool _isInitialized;
        
        public BonesSplineExecutor(Spline spline, SplineBindings bindings, Bone[] bones)
        {
            _spline = spline;
            _bindings = bindings;
            _bones = bones;
        }
        
        public void Initialize()
        {
            int pointCount = _bindings.Points.Length;
            _initialPositionOffsets = new Vector3[pointCount];
            _initialRotations = new Quaternion[pointCount];
            _initialLeftTangents = new Vector3[pointCount];
            _initialRightTangents = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 targetPosition = CalculatePointPosition(_bindings.Points[i]);
                _initialPositionOffsets[i] = _spline.GetPosition(i) - targetPosition;
                
                _initialLeftTangents[i] = _spline.GetLeftTangent(i);
                _initialRightTangents[i] = _spline.GetRightTangent(i);
                
                Quaternion targetRotation = CalculatePointRotation(_bindings.Points[i]);
                _initialRotations[i] = Quaternion.Inverse(targetRotation);
            }

            _isInitialized = true;
        }

        public void Update()
        {
            if (!_isInitialized)
                return;

            for (int i = 0; i < _bindings.Points.Length; i++)
            {
                Vector3 targetPosition = CalculatePointPosition(_bindings.Points[i]);
                
                Quaternion targetRotation = CalculatePointRotation(_bindings.Points[i]);
                Quaternion rotationDelta = targetRotation * _initialRotations[i];
                
                Vector3 newLeftTangent = rotationDelta * _initialLeftTangents[i];
                Vector3 newRightTangent = rotationDelta * _initialRightTangents[i];

                float length = CalculateBoneLengthByWeight(_bindings.Points[i]);
                newLeftTangent *= length;
                newRightTangent *= length;
                
                _spline.SetPosition(i, targetPosition + _initialPositionOffsets[i]);
                _spline.SetLeftTangent(i, newLeftTangent);
                _spline.SetRightTangent(i, newRightTangent);
            }
        }
        
        private float CalculateBoneLengthByWeight(PointBinding pointBinding)
        {
            if (pointBinding.Bones == null || pointBinding.Bones.Length == 0)
                return 1f;

            float weightedLength = 0f;
            float totalWeight = 0f;

            foreach (var boneWeight in pointBinding.Bones)
            {
                if (boneWeight.BoneIndex >= 0 && boneWeight.BoneIndex < _bones.Length)
                {
                    Bone bone = _bones[boneWeight.BoneIndex];
                    weightedLength += bone.Length * boneWeight.Weight; 
                    totalWeight += boneWeight.Weight;
                }
            }

            return totalWeight > 0 ? weightedLength / totalWeight : 1f;
        }

        private Vector3 CalculatePointPosition(PointBinding pointBinding)
        {
            if (pointBinding.Bones == null || pointBinding.Bones.Length == 0)
                return Vector3.zero;

            Vector3 weightedPosition = Vector3.zero;
            float totalWeight = 0f;

            foreach (var boneWeight in pointBinding.Bones)
            {
                if (boneWeight.BoneIndex >= 0 && boneWeight.BoneIndex < _bones.Length)
                {
                    Bone bone = _bones[boneWeight.BoneIndex];
                    weightedPosition += bone.GetPosition() * boneWeight.Weight;
                    totalWeight += boneWeight.Weight;
                }
            }
            return totalWeight > 0 ? weightedPosition / totalWeight : Vector3.zero;
        }
        
        private Quaternion CalculatePointRotation(PointBinding pointBinding)
        {
            if (pointBinding.Bones == null || pointBinding.Bones.Length == 0)
                return Quaternion.identity;

            Quaternion weightedRotation = Quaternion.identity;
            float totalWeight = 0f;

            foreach (var boneWeight in pointBinding.Bones)
            {
                if (boneWeight.BoneIndex >= 0 && boneWeight.BoneIndex < _bones.Length)
                {
                    Bone bone = _bones[boneWeight.BoneIndex];
                    Quaternion boneRotation = bone.GetRotation();
                    
                    if (totalWeight == 0)
                    {
                        weightedRotation = boneRotation;
                    }
                    else
                    {
                        weightedRotation = Quaternion.Slerp(weightedRotation, boneRotation, boneWeight.Weight / (totalWeight + boneWeight.Weight));
                    }
                    totalWeight += boneWeight.Weight;
                }
            }
            return weightedRotation;
        }
    }
}

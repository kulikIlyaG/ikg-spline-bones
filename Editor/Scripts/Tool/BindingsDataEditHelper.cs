using System;
using System.Linq;

namespace IKGTools.SplineBones.Editor
{
    internal static class BindingsDataEditHelper
    {
        public static PointBinding[] AddPoint(PointBinding[] currentBindings, int bonesCount)
        {
            var list = currentBindings.ToList();
            
            int pointIndex = 0;

            if (list.Count > 0)
                pointIndex = list.Last().PointIndex + 1;
            
            BoneWeight[] boneWeights = new BoneWeight[bonesCount];

            for (int index = 0; index < boneWeights.Length; index++)
                boneWeights[index] = new BoneWeight(index, 0f);
            
            list.Add(new PointBinding(pointIndex, boneWeights));
            
            return list.ToArray();
        }

        public static PointBinding[] RemovePoint(PointBinding[] currentBindings, int index)
        {
            var list = currentBindings.ToList();
            list.RemoveAt(index);
            return list.ToArray();
        }
        
        public static PointBinding[] OnAddedBone(PointBinding[] currentBindings, Bone[] bones)
        {
            PointBinding[] result = new PointBinding[currentBindings.Length];

            for (var index = 0; index < result.Length; index++)
            {
                var pointBinding = result[index];
                
                var pointWeights = currentBindings[index].Bones;
                Array.Resize(ref pointWeights, pointWeights.Length + 1);
                pointWeights[^1] = new BoneWeight(bones.Length - 1, 0f);
                pointBinding = new PointBinding(index, pointWeights);
                
                result[index] = pointBinding;
            }

            return result;
        }

        public static PointBinding[] OnRemovedBone(PointBinding[] currentBindings, int boneIndex)
        {
            PointBinding[] result = new PointBinding[currentBindings.Length];

            for (var index = 0; index < result.Length; index++)
            {
                var pointBinding = result[index];
                
                var pointWeights = currentBindings[index].Bones.ToList();
                pointWeights.Remove(pointWeights.FirstOrDefault(element => element.BoneIndex == boneIndex));
                int newBoneIndex = 0;
                foreach (BoneWeight weight in pointWeights)
                {
                    weight.SetNewBoneIndex(newBoneIndex);
                    newBoneIndex++;
                }
                pointBinding = new PointBinding(index, pointWeights.ToArray());
                
                result[index] = pointBinding;
            }

            return result;
        }

        public static PointBinding[] ValidatePointWeightsToSkeleton(PointBinding[] currentBindings, int bonesCount)
        {
            foreach (var binding in currentBindings)
            {
                BoneWeight[] bones = binding.Bones;

                if (bones.Length > bonesCount)
                {
                    Array.Resize(ref bones, bonesCount);
                }
                else if (bones.Length < bonesCount)
                {
                    BoneWeight[] newBones = new BoneWeight[bonesCount];

                    Array.Copy(bones, newBones, bones.Length);

                    for (int index = bones.Length; index < bonesCount; index++)
                    {
                        newBones[index] = new BoneWeight(index, 0f);
                    }

                    bones = newBones;
                }

                binding.SetBones(bones);
            }

            return currentBindings;
        }
    }
}
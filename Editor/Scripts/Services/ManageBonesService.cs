using IKGTools.Editor.Services;
using IKGTools.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace IKGTools.SplineBones.Editor.Services
{
    internal sealed class ManageBonesService
    {
        private readonly EditorDataService _dataService;

        private SkeletonDefinition SkeletonDefinition =>
            _dataService.Component.BindingsData.Bindings.SkeletonDefinition;

        private SplineBindingsData BindingsData => _dataService.Component.BindingsData;
        private SkeletonComponent Skeleton => _dataService.Component.Skeleton;
        
        public ManageBonesService(EditorDataService dataService)
        {
            _dataService = dataService;
        }
        
        public void TryUpdateBoneInstances(SkeletonComponent skeleton, SkeletonDefinition.BoneTransforms[] bonesPoints)
        {
            int bonesCount = skeleton.Bones?.Length ?? 0;
            if(bonesCount == bonesPoints.Length)
                return;
            
            Bone[] bones =
                RiggedSplineCreateHelper.CreateBonesByDefinition(bonesPoints);
            
            UpdateSkeletonBonesInstances(bones, skeleton);
        }

        public void AddBone(Vector2 worldPosition)
        {
            SkeletonDefinition.AddBone(worldPosition, PastelColorGenerator.GenerateUniquePastelColor(SkeletonDefinition.GetBonesColors(), 0.3f, 0.8f));
            EditorUtility.SetDirty(SkeletonDefinition);
            
            Bone[] bones =
                RiggedSplineCreateHelper.CreateBonesByDefinition(SkeletonDefinition.GetBonesTransform());

            UpdateSkeletonBonesInstances(bones, Skeleton);

            if (BindingsData != null)
            {
                BindingsData.Bindings.SetPointBindings(
                    BindingsDataEditHelper.OnAddedBone(BindingsData.Bindings.Points, Skeleton.Bones)
                );
                
                BindingsData.UpdateData();
                EditorUtility.SetDirty(BindingsData);
            }
        }
        
        public void RemoveBone(int index)
        {
            BindingsData.Bindings.SkeletonDefinition.RemoveBone(index);
            EditorUtility.SetDirty(BindingsData.Bindings.SkeletonDefinition);
            Bone[] bones =
                RiggedSplineCreateHelper.CreateBonesByDefinition(BindingsData.Bindings.SkeletonDefinition
                    .GetBonesTransform());
            
            UpdateSkeletonBonesInstances(bones, Skeleton);
            
            if (BindingsData != null)
            {
                BindingsData.Bindings.SetPointBindings(
                    BindingsDataEditHelper.OnRemovedBone(BindingsData.Bindings.Points, index)
                );
                
                BindingsData.UpdateData();
                EditorUtility.SetDirty(BindingsData);
            }
        }
        
        private void UpdateSkeletonBonesInstances(Bone[] bones, SkeletonComponent skeleton)
        {
            if (skeleton.Bones is {Length: > 0})
            {
                var previousBoneInstances = skeleton.Bones;
                for (int index = 0; index < previousBoneInstances.Length; index++)
                    Object.DestroyImmediate(previousBoneInstances[index].gameObject);
            }
            
            skeleton.SetBones(bones);
            for (int index = 0; index < bones.Length; index++)
                bones[index].transform.SetParent(skeleton.transform, true);
            EditorUtility.SetDirty(skeleton);
        }
    }
}
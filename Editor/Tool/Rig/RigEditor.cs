using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IKGTools.SplineBones.Editor
{
    internal interface IRigEditor
    {
        RigEditor.Mode CurrentMode { get; }
        event Action<RigEditor.Mode> OnChangedMode;
        void SetAddBoneMode();
        void SetRemoveBoneMode();
        void ResetMode();
    }
    internal sealed class RigEditor : IRigEditor
    {
        public enum Mode
        {
            None,
            AddBone,
            RemoveBone
        }

        private const string SCENE_BANNER_MESSAGE = "EDIT SKELETON";
        private static readonly Color SCENE_BANNER_BACKGROUND_COLOR = new Color32(96, 137, 223, 255);
        private static readonly Color SCENE_BANNER_LABEL_COLOR = new Color32(202, 214, 240, 255);
        
        private readonly BoneTransformControllersDrawer _boneTransformControllersDrawer;
        private readonly BoneRemoveGUIDrawer _boneRemoveGUIDrawer;
        
        private readonly Data _editorData;
        private readonly SceneBanner _sceneBanner;

        private bool _isDisposed = false;
        private Mode _mode;
        
        public event Action<Mode> OnChangedMode;

        internal RigEditor(Data editorData)
        {
            _editorData = editorData;

            _boneTransformControllersDrawer = new BoneTransformControllersDrawer(Skeleton);
            _boneTransformControllersDrawer.OnUpdatedBoneTransforms += BakeNewBoneTransforms;
            _boneRemoveGUIDrawer = new BoneRemoveGUIDrawer(Skeleton);
            _boneRemoveGUIDrawer.OnClickRemoveBone += OnClickRemoveBone;

            GUIStyle bannerLabelStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = SCENE_BANNER_LABEL_COLOR
                }
            };
            
            _sceneBanner = new SceneBanner(SCENE_BANNER_BACKGROUND_COLOR, 24f, bannerLabelStyle);
            
            RigContextMenu.SetRigEditor(this);
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void BakeNewBoneTransforms(int index, Vector2 position, Quaternion rotation)
        {
            _editorData.BindingsData.Bindings.SkeletonDefinition.SetNewTransforms(index, position, rotation);
            EditorUtility.SetDirty(_editorData.BindingsData.Bindings.SkeletonDefinition);
        }


        private SkeletonComponent Skeleton => _editorData.Skeleton;
        private SplineBindingsData BindingsData => _editorData.BindingsData;
        public Mode CurrentMode => _mode;

        public void Dispose()
        {
            RigContextMenu.RemoveRigEditor();
            SceneView.duringSceneGui -= OnSceneGUI;
            
            _isDisposed = true;
        }
        
        private void OnSceneGUI(SceneView obj)
        {
            switch (_mode)
            {
                case Mode.AddBone:
                    OnSceneGUIForAddBoneMode(obj);
                    break;
                case Mode.RemoveBone:
                    OnSceneGUIForRemoveBoneMode(obj);
                    break;
                default:
                    OnSceneGUIForNoneMode(obj);
                    break;
            }

            if (Skeleton == null && !_isDisposed)
                Dispose();
            
            _sceneBanner.Draw(obj,SCENE_BANNER_MESSAGE);
        }

        private void OnSceneGUIForNoneMode(SceneView sceneView)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            if (Skeleton.Bones == null || Skeleton.Bones.Length == 0)
                return;

            _boneTransformControllersDrawer.Draw();
        }

        private void OnSceneGUIForRemoveBoneMode(SceneView sceneView)
        {
            if (Skeleton.Bones == null || Skeleton.Bones.Length == 0)
                return;
            
            _boneRemoveGUIDrawer.Draw();
        }

        private void OnSceneGUIForAddBoneMode(SceneView sceneView)
        {
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.CustomCursor);
            Cursor.SetCursor(EditorAssetsHelper.Get<Texture2D>(PathHelper.GetPathForEditorTexture(PathHelper.Keys.CursorAddBone)), Vector2.zero, CursorMode.Auto);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(-Vector3.forward, Vector3.zero);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector2 worldPosition = ray.GetPoint(enter);

                    AddBone(worldPosition);
                }

                e.Use();
            }
        }

        public void AddBone(Vector2 worldPosition)
        {
            BindingsData.Bindings.SkeletonDefinition.AddBone(worldPosition, PastelColorGenerator.GenerateUniquePastelColor(BindingsData.Bindings.SkeletonDefinition.GetBonesColors(), 0.3f, 0.8f));
            EditorUtility.SetDirty(BindingsData.Bindings.SkeletonDefinition);
            
            Bone[] bones =
                RiggedSplineCreateHelper.CreateBonesByDefinition(BindingsData.Bindings.SkeletonDefinition
                    .GetBonesTransform());

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
        

        private void RemoveBone(int index)
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
        

        public static void TryUpdateBoneInstances(SkeletonComponent skeleton, SkeletonDefinition.BoneTransforms[] bonesPoints)
        {
            int bonesCount = skeleton.Bones?.Length ?? 0;
            if(bonesCount == bonesPoints.Length)
                return;
            
            Bone[] bones =
                RiggedSplineCreateHelper.CreateBonesByDefinition(bonesPoints);
            
            UpdateSkeletonBonesInstances(bones, skeleton);
        }

        private static void UpdateSkeletonBonesInstances(Bone[] bones, SkeletonComponent skeleton)
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

        
        private void OnClickRemoveBone(int boneIndex)
        {
            RemoveBone(boneIndex);
            
            if(Skeleton.Bones.Length <= 0)
                ResetMode();
        }
        
        public void SetAddBoneMode()
        {
            SetMode(Mode.AddBone);
        }

        public void SetRemoveBoneMode()
        {
            SetMode(Mode.RemoveBone);
        }

        public void ResetMode()
        {
            SetMode(Mode.None);
        }

        private void SetMode(Mode mode)
        {
            if (_mode == mode)
            {
                Debug.LogWarning($"You try set mode: {mode}; second times");
                return;
            }
            
            _mode = mode;
            OnChangedMode?.Invoke(_mode);
        }
    }
}
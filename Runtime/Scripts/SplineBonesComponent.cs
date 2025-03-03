using IKGTools.SplineBones;
using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones
{
    [ExecuteInEditMode]
    public sealed class SplineBonesComponent : MonoBehaviour
    {
        [SerializeField] private SplineProviderComponent _splineProvider;

        [SerializeField] private SkeletonComponent _skeleton;
        [SerializeField] private SplineBindingsData _bindingsData;

        public bool Execute;

        private BonesSplineExecutor _bonesSplineExecutor;

        public SkeletonComponent Skeleton => _skeleton;

        public Spline Spline => _splineProvider.Spline;
        public SplineBindingsData BindingsData => _bindingsData;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            Tick();
        }

        [ContextMenu("Initialize")]
        private void Initialize()
        {
            if (_bindingsData == null)
                return;

            _bonesSplineExecutor =
                new BonesSplineExecutor(_splineProvider.Spline, _bindingsData.Bindings, _skeleton.Bones);
            _bonesSplineExecutor.Initialize();
        }

        private void Tick()
        {
            if (Execute)
            {
                if (_bonesSplineExecutor == null)
                    Initialize();

                _bonesSplineExecutor.Update();
            }
        }


        public void RestartExecute()
        {
            Initialize();
        }


        private void OnValidate()
        {
            if (_splineProvider == null)
                _splineProvider = gameObject.GetComponentInChildren<SplineProviderComponent>();

            if (_skeleton == null)
                _skeleton = gameObject.GetComponentInChildren<SkeletonComponent>();
        }

#if UNITY_EDITOR
        public void SetBindingsData(SplineBindingsData data)
        {
            _bindingsData = data;
        }
#endif
    }
}
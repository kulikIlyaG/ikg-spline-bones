using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones
{
    [RequireComponent(typeof(SpriteShapeController))]
    public sealed class SpriteShapeSplineProviderComponent : SplineProviderComponent
    {
        [SerializeField] private SpriteShapeController _shapeController;

        private void OnValidate()
        {
            if (_shapeController == null)
                _shapeController = GetComponent<SpriteShapeController>();
        }

        public override Spline Spline => _shapeController.spline;
    }
}
using UnityEngine;
using UnityEngine.U2D;

namespace IKGTools.SplineBones
{
    public abstract class SplineProviderComponent : MonoBehaviour
    {
        public abstract Spline Spline { get; }
    }
}
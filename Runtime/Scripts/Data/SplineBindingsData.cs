using System;
using UnityEngine;

namespace IKGTools.SplineBones
{
    [CreateAssetMenu(fileName = "SplineBindingsData", menuName = "IKGTools/Spline Bones/Binding Data")]
    public sealed class SplineBindingsData : ScriptableObject
    {
        [SerializeField] private SplineBindings _bindings;

        public SplineBindings Bindings => _bindings;

#if UNITY_EDITOR
        /// <summary>
        ///  Only for Editor!
        /// </summary>
        public event Action OnUpdatedData;
        
        /// <summary>
        ///  Only for Editor!
        /// </summary>
        public void UpdateData()
        {
            OnUpdatedData?.Invoke();
        }
#endif
    }
}
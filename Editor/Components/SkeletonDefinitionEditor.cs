using System.Linq;
using IKGTools.SplineBones.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace IKGTools.SplineBones.Editor
{
    [CustomEditor(typeof(SkeletonDefinition))]
    internal class SkeletonDefinitionEditor : UnityEditor.Editor
    {
        private SkeletonDefinition _component;

        private void OnEnable()
        {
             _component = target as SkeletonDefinition;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            if (_component.IsNotEmpty)
            {
                BonesDrawerElement element = new BonesDrawerElement(
                    _component.GetBonesTransform().Select(element => element.WorldPosition).ToArray(),
                    _component.GetBonesColors());
                root.Add(element);
            }

            return root;
        }
    }
}